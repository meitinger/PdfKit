/* Copyright (C) 2016-2017, Manuel Meitinger
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 2 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace Aufbauwerk.Tools.PdfKit
{
    internal static class Ghostscript
    {
        private class GhostscriptException : Exception
        {
            internal static bool CheckResult(string api, int result, Tuple<StringBuilder, StringBuilder> stdio = null)
            {
                // return true if the operation was successful
                if (result >= 0)
                {
                    return true;
                }

                // return false if a cancellation in the main phase occured
                if (stdio != null && (result == Native.gs_error_Quit || result == Native.gs_error_interrupt))
                {
                    return false;
                }

                // throw standard error if quit was called outside of main phase
                if (result == Native.gs_error_Quit)
                {
                    throw new OperationCanceledException();
                }

                // on error throw an exception
                var message = new StringBuilder();
                message.Append(api).Append(": ").Append(result.ToString(CultureInfo.InvariantCulture));
                if (stdio != null)
                {
                    message.Append("\n\nOutput:\n").Append(stdio.Item1).Append("\n\nError:\n").Append(stdio.Item2);
                }
                throw new GhostscriptException(message.ToString());
            }

            private GhostscriptException(string message) : base(message) { }
        }

        private static readonly object instanceLock = new object();

        private static int AppendString(StringBuilder stdBuffer, IntPtr strBuffer, int len)
        {
            var buffer = new byte[len];
            Marshal.Copy(strBuffer, buffer, 0, len);
            stdBuffer.Append(Encoding.UTF8.GetString(buffer));
            return len;
        }

        private static IntPtr[] BuildArgs(string[] args)
        {
            // convert every arg to utf8
            var argsBuffer = new IntPtr[args.Length];
            try
            {
                for (var i = 0; i < args.Length; i++)
                {
                    var utf8 = Encoding.UTF8.GetBytes(args[i] + "\0");
                    argsBuffer[i] = Marshal.AllocHGlobal(utf8.Length);
                    Marshal.Copy(utf8, 0, argsBuffer[i], utf8.Length);
                }
            }
            catch
            {
                FreeArgs(argsBuffer);
                throw;
            }
            return argsBuffer;
        }

        private static void FreeArgs(IntPtr[] args)
        {
            // free every arg
            for (var i = 0; i < args.Length; i++)
            {
                if (args[i] != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(args[i]);
                }
            }
        }

        public static Image RenderPage(string filePath, int pageNumber, float dpiX = 96, float dpiY = 96, double scaleFactor = 1, Action<Image, Rectangle> progressiveUpdate = null, Func<bool> cancellationCallback = null)
        {
            var result = RenderPages(filePath, pageNumber, pageNumber, dpiX, dpiY, scaleFactor, progressiveUpdate == null ? null : new Action<int, Image, Rectangle>((_, image, rect) => progressiveUpdate(image, rect)), cancellationCallback);
            return result.Length > 0 ? result[0] : null;
        }

        public static Image[] RenderPages(string filePath, int firstPageNumber, int lastPageNumber, float dpiX = 96, float dpiY = 96, double scaleFactor = 1, Action<int, Image, Rectangle> progressiveUpdate = null, Func<bool> cancellationCallback = null, Action<int, Image> pageCallback = null)
        {
            // check the input arguments
            if (filePath == null)
            {
                throw new ArgumentNullException("filePath");
            }
            if (firstPageNumber < 1)
            {
                throw new ArgumentOutOfRangeException("firstPageNumber");
            }
            if (lastPageNumber < firstPageNumber)
            {
                throw new ArgumentOutOfRangeException("lastPageNumber");
            }
            if (dpiX <= 0)
            {
                throw new ArgumentOutOfRangeException("dpiX");
            }
            if (dpiY <= 0)
            {
                throw new ArgumentOutOfRangeException("dpiY");
            }
            if (scaleFactor <= 0)
            {
                throw new ArgumentOutOfRangeException("scaleFactor");
            }

            // create the display device callbacks
            var nextImageIndex = 0;
            var progressThreshold = 0;
            var dirtyRect = Rectangle.Empty;
            var images = new Image[lastPageNumber - firstPageNumber + 1];
            var gsImage = (Bitmap)null;
            var deleteGsImage = new Action(() =>
            {
                // delete any old source image
                if (gsImage != null)
                {
                    lock (gsImage)
                    {
                        gsImage.Tag = false;
                        gsImage.Dispose();
                    }
                    gsImage = null;
                }
            });
            var displayCallback = new Native.display_callback_v1(false)
            {
                display_preclose = (h, d) =>
                {
                    deleteGsImage();
                    return 0;
                },
                display_presize = (h, d, width, height, raster, format) =>
                {
                    deleteGsImage();
                    return 0;
                },
                display_size = (h, d, width, height, raster, format, pimage) =>
                {
                    // ensure the operation isn't completed
                    if (nextImageIndex < images.Length)
                    {
                        // create the new source image
                        gsImage = new Bitmap(width, height, raster, PixelFormat.Format32bppRgb, pimage);
                        gsImage.Tag = true;

                        // notify the caller
                        if (progressiveUpdate != null)
                        {
                            progressThreshold = (width * height) / 100;
                            dirtyRect = new Rectangle(0, 0, width, height);
                            progressiveUpdate(firstPageNumber + nextImageIndex, gsImage, dirtyRect);
                            dirtyRect = Rectangle.Empty;
                        }
                    }
                    return 0;
                },
                display_page = (h, d, c, f) =>
                {
                    // ensure the operation isn't completed
                    if (nextImageIndex < images.Length)
                    {
                        // store null and return an error if display_size has not been called yet
                        if (gsImage == null)
                        {
                            images[nextImageIndex++] = null;
                            return Native.gs_error_undefinedresult;
                        }

                        // notify the caller if not everything is drawn yet
                        if (progressiveUpdate != null && !dirtyRect.IsEmpty)
                        {
                            progressiveUpdate(firstPageNumber + nextImageIndex, gsImage, dirtyRect);
                            dirtyRect = Rectangle.Empty;
                        }

                        // store the result image and increment the index
                        Bitmap image;
                        lock (gsImage)
                        {
                            image = new Bitmap(gsImage);
                        }
                        image.SetResolution(dpiX, dpiY);
                        images[nextImageIndex++] = image;

                        // notify the caller
                        if (pageCallback != null)
                        {
                            pageCallback(firstPageNumber + nextImageIndex - 1, image);
                        }
                    }
                    return 0;
                },
                display_update = progressiveUpdate == null ? null : new Native.display_update((h, d, x, y, width, height) =>
                {
                    // ensure the operation isn't completed and has an image
                    if (nextImageIndex < images.Length && gsImage != null)
                    {
                        // enlarge the dirty area and notify the caller if it's sufficiently large
                        var newRect = new Rectangle(x, y, width, height);
                        dirtyRect = dirtyRect.IsEmpty ? newRect : Rectangle.Union(dirtyRect, newRect);
                        if (dirtyRect.Width * dirtyRect.Height > progressThreshold)
                        {
                            progressiveUpdate(firstPageNumber + nextImageIndex, gsImage, dirtyRect);
                            dirtyRect = Rectangle.Empty;
                        }
                    }
                    return 0;
                }),
            };

            // run ghostscript
            var cancelled = !Run(new string[]
            {
                "PdfKit",
                "-dNOPAUSE",
                "-dBATCH",
                "-dSAFER",
                "-sDEVICE=display",
                "-dTextAlphaBits=4",
                "-dGraphicsAlphaBits=4",
                string.Format(CultureInfo.InvariantCulture, "-r{0}x{1}", dpiX * scaleFactor, dpiY* scaleFactor),
                "-dDisplayFormat=" + (Native.DISPLAY_COLORS_RGB | Native.DISPLAY_UNUSED_LAST | Native.DISPLAY_DEPTH_8 | Native.DISPLAY_LITTLEENDIAN).ToString(CultureInfo.InvariantCulture),
                "-dFirstPage=" + firstPageNumber.ToString(CultureInfo.InvariantCulture),
                "-dLastPage=" + lastPageNumber.ToString(CultureInfo.InvariantCulture),
                "-dAutoRotatePages=/None",
                "-f",
                filePath,
            }, cancellationCallback, displayCallback);

            // check if some pages have not been rendered
            if (nextImageIndex < images.Length)
            {
                // throw an error if the operation wasn't cancelled by the caller
                if (!cancelled)
                {
                    throw new InvalidOperationException();
                }

                // resize the result array
                Array.Resize(ref images, nextImageIndex);
            }

            // return the images
            return images;
        }

        public static bool Run(string[] args, Func<bool> cancellationCallback = null, Native.display_callback_v1? displayCallback = null)
        {
            // acquire the instance lock
            lock (instanceLock)
            {
                // create a new instance
                var argumentsBuffer = (IntPtr[])null;
                var callbackBuffer = IntPtr.Zero;
                IntPtr instance;
                GhostscriptException.CheckResult("gsapi_new_instance", Native.gsapi_new_instance(out instance, IntPtr.Zero));
                try
                {
                    // ensure utf8 is selected
                    GhostscriptException.CheckResult("gsapi_set_arg_encoding", Native.gsapi_set_arg_encoding(instance, Native.GS_ARG_ENCODING_UTF8));

                    // set the stdio redirection
                    var stdoutBuffer = new StringBuilder();
                    var stderrBuffer = new StringBuilder();
                    var stdinCallback = new Native.stdin_fn((h, b, l) => 0);
                    var stdoutCallback = new Native.stdout_fn((h, b, l) => AppendString(stdoutBuffer, b, l));
                    var stderrCallback = new Native.stderr_fn((h, b, l) => AppendString(stderrBuffer, b, l));
                    GhostscriptException.CheckResult("gsapi_set_stdio", Native.gsapi_set_stdio(instance, stdinCallback, stdoutCallback, stderrCallback));

                    // set the poll callback
                    var cancelled = false;
                    if (cancellationCallback != null)
                    {
                        var pollCallback = new Native.poll_fn(_ =>
                        {
                            if (cancelled)
                            {
                                return Native.gs_error_interrupt;
                            }
                            else if (cancellationCallback())
                            {
                                cancelled = true;
                                return Native.gs_error_interrupt;
                            }
                            else
                            {
                                return 0;
                            }
                        });
                        GhostscriptException.CheckResult("gsapi_set_poll", Native.gsapi_set_poll(instance, pollCallback));
                    }

                    // set the display callback
                    if (displayCallback.HasValue)
                    {
                        callbackBuffer = Marshal.AllocHGlobal(displayCallback.Value.size);
                        Marshal.StructureToPtr(displayCallback.Value, callbackBuffer, false);
                        GhostscriptException.CheckResult("gsapi_set_display_callback", Native.gsapi_set_display_callback(instance, callbackBuffer));
                    }

                    // run ghostscript
                    argumentsBuffer = BuildArgs(args);
                    var interrupted = !GhostscriptException.CheckResult("gsapi_init_with_args", Native.gsapi_init_with_args(instance, argumentsBuffer.Length, argumentsBuffer), Tuple.Create(stdoutBuffer, stderrBuffer));
                    if (interrupted && !cancelled)
                    {
                        // throw an error if the operation was aborted elsewhere
                        throw new OperationCanceledException();
                    }
                    return !interrupted;
                }
                finally
                {
                    // exit and delete the instance
                    Native.gsapi_exit(instance);
                    Native.gsapi_delete_instance(instance);

                    // free the callback buffer
                    if (callbackBuffer != IntPtr.Zero)
                    {
                        Marshal.DestroyStructure(callbackBuffer, typeof(Native.display_callback_v1));
                        Marshal.FreeHGlobal(callbackBuffer);
                    }

                    // free the argument strings
                    if (argumentsBuffer != null)
                    {
                        FreeArgs(argumentsBuffer);
                    }
                }
            }
        }
    }
}
