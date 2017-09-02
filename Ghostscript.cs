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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using Aufbauwerk.Tools.PdfKit.Properties;

namespace Aufbauwerk.Tools.PdfKit
{
    public class GhostscriptException : Exception
    {
        public GhostscriptException(string message, int errorCode)
            : base(message)
        {
            ErrorCode = errorCode;
        }

        public int ErrorCode { get; private set; }
    }

    public class GhostscriptRendererEventArgs : EventArgs
    {
        internal GhostscriptRendererEventArgs(Bitmap image)
        {
            Image = image;
        }

        public Bitmap Image { get; private set; }
    }

    public class Ghostscript : IDisposable
    {
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

        private static int ReadStdBuffer(StringBuilder stdBuffer, IntPtr output, int len)
        {
            if (output == IntPtr.Zero)
            {
                return -1;
            }

            // read utf8 from buffer
            var buffer = new char[Math.Min(len, stdBuffer.Length)];
            stdBuffer.CopyTo(0, buffer, 0, buffer.Length);
            var charCount = len;
            while (charCount > 0 && Encoding.UTF8.GetByteCount(buffer, 0, charCount) > len)
            {
                charCount--;
            }
            var bytes = Encoding.UTF8.GetBytes(buffer, 0, charCount);
            Marshal.Copy(bytes, 0, output, bytes.Length);
            stdBuffer.Remove(0, charCount);
            return bytes.Length;
        }

        private static int WriteStdBuffer(StringBuilder stdBuffer, IntPtr input, int len)
        {
            if (input == IntPtr.Zero)
            {
                return -1;
            }

            // write utf8 to buffer
            var buffer = new byte[len];
            Marshal.Copy(input, buffer, 0, len);
            stdBuffer.Append(Encoding.UTF8.GetString(buffer));
            return len;
        }

        private bool _cancelled = false;
        private bool _initialized = false;
        private IntPtr _instance = IntPtr.Zero;
        private readonly Native.poll_fn _poll;
        private readonly Native.stderr_fn _stdErr;
        private readonly Native.stdin_fn _stdIn;
        private readonly Native.stdout_fn _stdOut;

        internal readonly StringBuilder StdIn = new StringBuilder();
        internal readonly StringBuilder StdErr = new StringBuilder();
        internal readonly StringBuilder StdOut = new StringBuilder();

        protected Ghostscript()
        {
            // store the delegates
            _stdIn = new Native.stdin_fn(StdInFunction);
            _stdOut = new Native.stdout_fn(StdOutFunction);
            _stdErr = new Native.stderr_fn(StdErrFunction);
            _poll = new Native.poll_fn(PollFunction);
        }


        public Ghostscript(string[] arguments)
            : this()
        {
            // check the arguments array
            if (arguments == null)
            {
                throw new ArgumentNullException("arguments");
            }

            // initialize ghostscript
            try
            {
                Prepare();
                Inialize(arguments);
            }
            catch
            {
                Dispose();
                throw;
            }
        }

        ~Ghostscript()
        {
            Dispose(false);
        }

        protected IntPtr Instance
        {
            get { return _instance; }
        }

        private void CheckDisposed()
        {
            // throw an exception if instance is null
            if (_instance == IntPtr.Zero)
            {
                throw new ObjectDisposedException(GetType().ToString());
            }
        }

        protected void CheckResult(string api, int result)
        {
            // on error throw an exception
            if (result < 0)
            {
                if (result == Native.gs_error_interrupt)
                {
                    // check the two causes for interrupt
                    if (_cancelled)
                    {
                        throw new OperationCanceledException();
                    }
                    CheckDisposed();
                }
                throw new GhostscriptException(string.Format(Resources.Ghostscript_Exception, api, result, StdOut, StdErr), result);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            // exit and delete the instance
            if (_instance != IntPtr.Zero)
            {
                var instance = _instance;
                _instance = IntPtr.Zero;
                if (_initialized)
                {
                    _initialized = false;
                    Native.gsapi_exit(instance);
                }
                Native.gsapi_delete_instance(instance);
            }
        }

        protected void Inialize(string[] arguments)
        {
            // ensure the instance is prepared and not initialized
            if (_instance == IntPtr.Zero || _initialized)
            {
                throw new InvalidOperationException();
            }
            _initialized = true;

            // initialize ghostscript
            var argumentsBuffer = BuildArgs(arguments);
            try
            {
                CheckResult("gsapi_init_with_args", Native.gsapi_init_with_args(_instance, argumentsBuffer.Length, argumentsBuffer));
            }
            finally
            {
                FreeArgs(argumentsBuffer);
            }
        }

        private int PollFunction(IntPtr caller_handle)
        {
            // if already cancelled or disposed then interrupt immediatelly
            if (_cancelled || _instance == IntPtr.Zero)
            {
                return Native.gs_error_interrupt;
            }

            // query the event handlers
            var poll = Poll;
            if (poll != null)
            {
                try
                {
                    var e = new CancelEventArgs();
                    poll(this, e);
                    if (e.Cancel)
                    {
                        // set the flag and interrupt
                        _cancelled = true;
                        return Native.gs_error_interrupt;
                    }
                }
                catch
                {
                    return Native.gs_error_Fatal;
                }
            }

            // everything is fine, continue
            return 0;
        }

        protected void Prepare()
        {
            // check the state
            if (_instance != IntPtr.Zero)
            {
                throw new InvalidOperationException();
            }

            // create a new instance
            CheckResult("gsapi_new_instance", Native.gsapi_new_instance(out _instance, IntPtr.Zero));

            // ensure utf8 is selected
            CheckResult("gsapi_set_arg_encoding", Native.gsapi_set_arg_encoding(_instance, Native.GS_ARG_ENCODING_UTF8));

            // set the stdio redirection
            CheckResult("gsapi_set_stdio", Native.gsapi_set_stdio(_instance, _stdIn, _stdOut, _stdErr));

            // set the poll callback
            CheckResult("gsapi_set_poll", Native.gsapi_set_poll(_instance, _poll));
        }

        public void Run(string commandString)
        {
            // check the arguments and state
            if (commandString == null)
            {
                throw new ArgumentNullException("commandString");
            }
            CheckDisposed();

            // convert the string to utf8 and call run
            Run(Encoding.UTF8.GetBytes(commandString));
        }

        public void Run(byte[] commandBuffer)
        {
            // check the arguments and state
            if (commandBuffer == null)
            {
                throw new ArgumentNullException("commandBuffer");
            }
            CheckDisposed();

            // pin the buffer
            var handle = GCHandle.Alloc(commandBuffer, GCHandleType.Pinned);
            try
            {
                // check if the string is too long for a single call
                int exitCode;
                const int maxSize = ushort.MaxValue - 5;
                if (commandBuffer.Length > maxSize)
                {
                    // run the string in parts
                    CheckResult("gsapi_run_string_begin", Native.gsapi_run_string_begin(_instance, 0, out exitCode));
                    for (var offset = 0; offset < commandBuffer.Length; offset += maxSize)
                    {
                        var result = Native.gsapi_run_string_continue(_instance, handle.AddrOfPinnedObject() + offset, Math.Min(commandBuffer.Length - offset, maxSize), 0, out exitCode);
                        if (result == Native.gs_error_NeedInput)
                        {
                            continue;
                        }
                        CheckResult("gsapi_run_string_continue", result);
                    }
                    CheckResult("gsapi_run_string_end", Native.gsapi_run_string_end(_instance, 0, out exitCode));
                }
                else
                {
                    CheckResult("gsapi_run_string_with_length", Native.gsapi_run_string_with_length(_instance, handle.AddrOfPinnedObject(), commandBuffer.Length, 0, out exitCode));
                }
            }
            finally
            {
                handle.Free();
            }
        }

        private int StdInFunction(IntPtr caller_handle, IntPtr buf, int len)
        {
            try
            {
                return ReadStdBuffer(StdIn, buf, len);
            }
            catch (Exception e)
            {
                StdErr.Append(e);
                return -1;
            }
        }

        private int StdErrFunction(IntPtr caller_handle, IntPtr buf, int len)
        {
            try
            {
                return WriteStdBuffer(StdErr, buf, len);
            }
            catch (Exception e)
            {
                StdErr.Append(e);
                return -1;
            }
        }

        private int StdOutFunction(IntPtr caller_handle, IntPtr buf, int len)
        {
            try
            {
                return WriteStdBuffer(StdOut, buf, len);
            }
            catch (Exception e)
            {
                StdErr.Append(e);
                return -1;
            }
        }

        public event EventHandler<CancelEventArgs> Poll;
    }

    internal class GhostscriptRenderer : Ghostscript
    {
        private static readonly string[] Arguments = new string[]
        {
            "PdfKit",
            "-dNOPAUSE",
            "-dQUIET",
            "-sDEVICE=display",
            "-dDisplayFormat=" + (Native.DISPLAY_COLORS_RGB | Native.DISPLAY_UNUSED_LAST | Native.DISPLAY_DEPTH_8 | Native.DISPLAY_LITTLEENDIAN).ToString(CultureInfo.InvariantCulture),
            "-dTextAlphaBits=4",
            "-dGraphicsAlphaBits=4",
            "-dAutoRotatePages=/None",
        };

        private IntPtr _callbackBuffer = IntPtr.Zero;
        private readonly Native.display_callback_v1 _displayCallback;
        private bool _filledStruct = false;
        private Bitmap _temporaryImage = null;

        public GhostscriptRenderer()
        {
            // set the callback buffer
            _displayCallback = new Native.display_callback_v1(false)
            {
                display_preclose = new Native.display_preclose(DisplayPreClose),
                display_presize = new Native.display_presize(DisplayPreSize),
                display_size = new Native.display_size(DisplaySize),
                display_page = new Native.display_page(DisplayPage),
                display_update = new Native.display_update(DisplayUpdate),
            };

            // initialize Ghostscript with the display callback
            try
            {
                _callbackBuffer = Marshal.AllocHGlobal(_displayCallback.size);
                Marshal.StructureToPtr(_displayCallback, _callbackBuffer, false);
                _filledStruct = true;
                Prepare();
                CheckResult("gsapi_set_display_callback", Native.gsapi_set_display_callback(Instance, _callbackBuffer));
                Inialize(Arguments);
            }
            catch
            {
                Dispose();
                throw;
            }
        }

        private void ClearProgressiveImage()
        {
            // delete any old source image
            if (_temporaryImage != null)
            {
                lock (_temporaryImage)
                {
                    _temporaryImage.Tag = false;
                    _temporaryImage.Dispose();
                }
                _temporaryImage = null;
            }
        }

        private int DisplayPage(IntPtr handle, IntPtr device, int copies, int flush)
        {
            // return an error if display_size has not been called yet
            if (_temporaryImage == null)
            {
                return Native.gs_error_undefinedresult;
            }

            // notify the page handler if any
            var page = Page;
            if (page != null)
            {
                try
                {
                    // store the resulting image
                    Bitmap image;
                    lock (_temporaryImage)
                    {
                        var rect = new Rectangle(Point.Empty, _temporaryImage.Size);
                        image = new Bitmap(rect.Width, rect.Height, PixelFormat.Format24bppRgb);
                        using (var gc = Graphics.FromImage(image))
                        {
                            gc.DrawImage(_temporaryImage, rect, rect, GraphicsUnit.Pixel);
                        }
                        image = new Bitmap(_temporaryImage);
                    }
                    page(this, new GhostscriptRendererEventArgs(image));
                }
                catch (Exception e)
                {
                    StdErr.Append(e);
                    return Native.gs_error_Fatal;
                }
            }
            return 0;
        }

        private int DisplayPreClose(IntPtr handle, IntPtr device)
        {
            try
            {
                ClearProgressiveImage();
            }
            catch (Exception e)
            {
                StdErr.Append(e);
                return Native.gs_error_Fatal;
            }
            return 0;
        }

        private int DisplayPreSize(IntPtr handle, IntPtr device, int width, int height, int raster, uint format)
        {
            try
            {
                ClearProgressiveImage();
            }
            catch (Exception e)
            {
                StdErr.Append(e);
                return Native.gs_error_Fatal;
            }
            return 0;
        }

        private int DisplaySize(IntPtr handle, IntPtr device, int width, int height, int raster, uint format, IntPtr pimage)
        {
            try
            {
                // create the new source image
                _temporaryImage = new Bitmap(width, height, raster, PixelFormat.Format32bppRgb, pimage);
                _temporaryImage.Tag = true;

                // notify the update listeners
                var update = Update;
                if (update != null)
                {
                    update(this, new GhostscriptRendererEventArgs(_temporaryImage));
                }
            }
            catch (Exception e)
            {
                StdErr.Append(e);
                return Native.gs_error_Fatal;
            }
            return 0;
        }

        private int DisplayUpdate(IntPtr handle, IntPtr device, int x, int y, int width, int height)
        {
            // ensure there are listeners and an image
            var update = Update;
            if (update != null && _temporaryImage != null)
            {
                try
                {
                    if (!(bool)_temporaryImage.Tag)
                    {
                        _temporaryImage.Tag = true;
                        update(this, new GhostscriptRendererEventArgs(_temporaryImage));
                    }
                }
                catch (Exception e)
                {
                    StdErr.Append(e);
                    return Native.gs_error_Fatal;
                }
            }
            return 0;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            // free the buffer
            if (_callbackBuffer != IntPtr.Zero)
            {
                if (_filledStruct)
                {
                    Marshal.DestroyStructure(_callbackBuffer, typeof(Native.display_callback_v1));
                    _filledStruct = false;
                }
                Marshal.FreeHGlobal(_callbackBuffer);
                _callbackBuffer = IntPtr.Zero;
            }
        }

        public event EventHandler<GhostscriptRendererEventArgs> Page;

        public event EventHandler<GhostscriptRendererEventArgs> Update;
    }
}
