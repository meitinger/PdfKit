/* Copyright (C) 2016-2021, Manuel Meitinger
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
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Aufbauwerk.Tools.PdfKit.Properties;

namespace Aufbauwerk.Tools.PdfKit
{
    public class ConvertFormat
    {
        private static readonly List<ConvertFormat> _formats = new List<ConvertFormat>();

        public static readonly ConvertFormat Bmp;
        public static readonly ConvertFormat Eps;
        public static readonly ConvertFormat Jpeg;
        public static readonly ConvertFormat OptimizedPdf;
        public static readonly ConvertFormat Pdf;
        public static readonly ConvertFormat Png;
        public static readonly ConvertFormat Ps;
        public static readonly ConvertFormat Tiff;

        static ConvertFormat()
        {
            _formats.Add(Pdf = new ConvertFormat(Resources.Converter_FormatPdf, "pdf", true, DocumentType.Any & ~DocumentType.PortableDocumentFormat, "-sDEVICE=pdfwrite"));
            _formats.Add(OptimizedPdf = new ConvertFormat(Resources.Converter_FormatOptimizedPdf, "optimized.pdf", true, DocumentType.PortableDocumentFormat,
                "-dColorConversionStrategy=/RGB",
                "-dColorImageDownsampleThreshold=1.5", "-dColorImageDownsampleType=/Bicubic", "-dColorImageFilter=/DCTEncode", "-dColorImageResolution=96",
                "-dCompatibilityLevel=1.4",
                "-dCompressPages=true",
                "-dConvertCMYKImagesToRGB=true",
                "-dDetectDuplicateImages=true",
                "-dDownsampleColorImages=true", "-dDownsampleGrayImages=true", "-dDownsampleMonoImages=true",
                "-dEmbedAllFonts=true",
                "-dFastWebView=true",
                "-dGrayImageDownsampleThreshold=1.5", "-dGrayImageDownsampleType=/Bicubic", "-dGrayImageFilter=/DCTEncode", "-dGrayImageResolution=96",
                "-dMaxInlineImageSize=0",
                "-dMaxShadingBitmapSize=16000",
                "-dMonoImageDownsampleThreshold=1.5", "-dMonoImageDownsampleType=/Bicubic", "-dMonoImageFilter=/CCITTFaxEncode", "-dMonoImageResolution=192",
                "-dOptimize=true",
                "-dUseFlateCompression=true",
                "-r192",
                "-sDEVICE=pdfwrite"
            ));
            _formats.Add(Ps = new ConvertFormat(Resources.Converter_FormatPs, "ps", true, DocumentType.PortableDocumentFormat | DocumentType.EncapsulatedPostScript, "-sDEVICE=ps2write"));
            _formats.Add(Eps = new ConvertFormat(Resources.Converter_FormatEps, "eps", false, DocumentType.PortableDocumentFormat | DocumentType.PostScript, "-sDEVICE=eps2write"));
            _formats.Add(Png = new ConvertFormat(Resources.Converter_FormatPng, "png", false, new PngFormatDialog()));
            _formats.Add(Jpeg = new ConvertFormat(Resources.Converter_FormatJpeg, "jpg", false, new JpegFormatDialog()));
            _formats.Add(Tiff = new ConvertFormat(Resources.Converter_FormatTiff, "tif", true, new TiffFormatDialog()));
            _formats.Add(Bmp = new ConvertFormat(Resources.Converter_FormatBmp, "bmp", false, new BmpFormatDialog()));
        }

        public static IEnumerable<ConvertFormat> GetApplicable(DocumentType type)
        {
            return _formats.Where(f => (f.AllowedTypes & type) != 0);
        }

        private readonly string[] _args;
        private readonly FormatDialog _dialog;

        private ConvertFormat(string name, string fileExtension, bool supportsSingleFile, ImageFormatDialog dialog)
        {
            _args = null;
            _dialog = dialog;
            Program.PrepareForm(dialog);
            Name = name;
            FileExtension = fileExtension;
            SupportsSingleFile = supportsSingleFile;
            AllowedTypes = DocumentType.Any & ~DocumentType.Image;
        }

        private ConvertFormat(string name, string fileExtension, bool supportsSingleFile, DocumentType allowedTypes, params string[] args)
        {
            _args = args;
            _dialog = null;
            Name = name;
            FileExtension = fileExtension;
            SupportsSingleFile = supportsSingleFile;
            AllowedTypes = allowedTypes;
        }

        public DocumentType AllowedTypes { get; private set; }

        public string FileExtension { get; private set; }

        public string Name { get; private set; }

        public bool SupportsSingleFile { get; private set; }

        public string[] GetArguments(string outputFile)
        {
            // check the arguments
            if (outputFile == null)
            {
                throw new ArgumentNullException(nameof(outputFile));
            }

            // build the Ghostscript command line
            var list = new List<string>() { "PdfKit", "-dBATCH", "-dNOPAUSE", "-dEPSCrop", "-dAutoRotatePages=/None" };
            if (_args != null)
            {
                list.AddRange(_args);
            }
            if (_dialog != null)
            {
                _dialog.FillArguments(list);
            }
            list.Add("-sOutputFile=" + outputFile);
            return list.ToArray();
        }

        public bool ShowDialog(IWin32Window parent = null)
        {
            // show the dialog (if any)
            return _dialog == null || _dialog.ShowDialog(parent) == DialogResult.OK;
        }
    }

    public class Converter
    {
        private class Win32Window : IWin32Window
        {
            public static Win32Window FromHandle(IntPtr handle)
            {
                return handle == IntPtr.Zero ? null : new Win32Window(handle);
            }

            private readonly IntPtr _handle;

            private Win32Window(IntPtr handle)
            {
                _handle = handle;
            }

            public IntPtr Handle
            {
                get { return _handle; }
            }
        }

        public static void Run(IEnumerable<string> files, ConvertFormat format)
        {
            // check the arguments
            if (files == null)
            {
                throw new ArgumentNullException(nameof(files));
            }
            if (format == null)
            {
                throw new ArgumentNullException(nameof(format));
            }

            // show the dialog and run the converter
            if (format.ShowDialog())
            {
                new Converter(format, files).Run();
            }
        }

        private volatile bool _aborted = false;
        private int _completedPageCount = 0;
        private Document _currentFile = null;
        private int _dequeuedPageCount = 0;
        private readonly Queue<Document> _files = new Queue<Document>();
        private readonly ConvertFormat _format;
        private bool _initializationDone = false;
        private Win32Window _parent = null;
        private readonly object _parentLock = new object();
        private readonly Native.IProgressDialog _progressDialog;
        private readonly string _title;
        private int _totalPageCount = 0;

        private Converter(ConvertFormat format, IEnumerable<string> files)
        {
            // store the format and title
            _format = format;
            _title = string.Format(Resources.Converter_DialogTitle, Application.ProductName, _format.Name);

            // create the process dialog
            _progressDialog = (Native.IProgressDialog)Activator.CreateInstance(Type.GetTypeFromCLSID(Native.CLSID_ProgressDialog, true));
            _progressDialog.SetTitle(_title);
            _progressDialog.SetCancelMsg(Resources.Converter_CancelMessage, IntPtr.Zero);

            // start looking for files
            ThreadPool.QueueUserWorkItem(AsyncInitialize, files);
        }

        private void AsyncInitialize(object files)
        {
            // enqueue all selected files
            try
            {
                foreach (var filePath in (IEnumerable<string>)files)
                {
                    // try to load the file
                    Document file;
                Retry:
                    if (_aborted)
                    {
                        return;
                    }
                    try
                    {
                        file = Document.FromFile(filePath, _format.AllowedTypes);
                    }
                    catch (Exception e)
                    {
                        // show the error dialog and let the user pick
                        switch (QueryUser(string.Format(Resources.Converter_LoadFileError, filePath, e.Message)))
                        {
                            case DialogResult.Abort:
                            {
                                return;
                            }
                            case DialogResult.Retry:
                            {
                                goto Retry;
                            }
                            case DialogResult.Ignore:
                            {
                                continue;
                            }
                            default:
                            {
                                throw;
                            }
                        }
                    }

                    // enqueue the file and notify the main thread
                    lock (_files)
                    {
                        _files.Enqueue(file);
                        _totalPageCount += file.PageCount;
                        Monitor.Pulse(_files);
                    }
                }
            }
            finally
            {
                // notify the main thread that initialization is complete
                lock (_files)
                {
                    _initializationDone = true;
                    Monitor.Pulse(_files);
                }
            }
        }

        private DialogResult QueryUser(string text)
        {
            lock (_parentLock)
            {
                // wait for the progress dialog to show
                while (true)
                {
                    if (_aborted)
                    {
                        return DialogResult.Abort;
                    }
                    if (_parent != null)
                    {
                        break;
                    }
                    Monitor.Wait(_parentLock);
                }

                // show the message
                var result = MessageBox.Show(_parent, text, _title, MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Warning);
                if (result == DialogResult.Abort)
                {
                    _aborted = true;
                }
                return result;
            }
        }

        private void Run()
        {
            // show the process dialog
            _progressDialog.StartProgressDialog(IntPtr.Zero, IntPtr.Zero, Native.PROGDLG_AUTOTIME, IntPtr.Zero);
            try
            {
                // set the parent
                lock (_parentLock)
                {
                    _parent = Win32Window.FromHandle(((Native.IOleWindow)_progressDialog).GetWindow());
                    while (!Native.IsWindowVisible(_parent.Handle))
                    {
                        Thread.Sleep(0);
                    }
                    Native.SetForegroundWindow(_parent.Handle);
                    Monitor.Pulse(_parentLock);
                }

                // convert each file
                while (true)
                {
                    // reset the dialog
                    _currentFile = null;
                    _completedPageCount = 0;
                    UpdateDialog();

                    // dequeue the next file
                    lock (_files)
                    {
                        // get or wait for the next file
                        while (_files.Count == 0)
                        {
                            if (_initializationDone || _aborted)
                            {
                                return;
                            }
                            Monitor.Wait(_files);
                        }
                        _currentFile = _files.Dequeue();
                        _dequeuedPageCount += _currentFile.PageCount;
                    }

                // convert the file
                Retry:
                    if (_aborted)
                    {
                        return;
                    }
                    UpdateDialog();
                    try
                    {
                        _currentFile.Convert(_format, () => { _completedPageCount++; UpdateDialog(); }, () => _aborted || _progressDialog.HasUserCancelled());
                    }
                    catch (OperationCanceledException)
                    {
                        _aborted = true;
                        return;
                    }
                    catch (Exception e)
                    {
                        // disable the timer while showing the error dialog
                        _progressDialog.Timer(Native.PDTIMER_PAUSE, IntPtr.Zero);
                        try
                        {
                            switch (QueryUser(string.Format(Resources.Converter_ConvertFileError, _currentFile.FilePath, e.Message)))
                            {
                                case DialogResult.Abort:
                                {
                                    return;
                                }
                                case DialogResult.Retry:
                                {
                                    _completedPageCount = 0;
                                    goto Retry;
                                }
                                case DialogResult.Ignore:
                                {
                                    continue;
                                }
                                default:
                                {
                                    throw;
                                }
                            }
                        }
                        finally
                        {
                            _progressDialog.Timer(Native.PDTIMER_RESUME, IntPtr.Zero);
                        }
                    }
                }
            }
            finally
            {
                // clear the parent
                lock (_parentLock)
                {
                    _parent = null;
                    Monitor.Pulse(_parentLock);
                }

                // hide the dialog
                _progressDialog.StopProgressDialog();
            }
        }

        private void UpdateDialog()
        {
            // update the progress dialog
            if (_currentFile == null)
            {
                _progressDialog.SetLine(1, Resources.Converter_LoadingFiles, true, IntPtr.Zero);
                _progressDialog.SetLine(2, null, false, IntPtr.Zero);
                if (_initializationDone)
                {
                    _progressDialog.SetProgress(_dequeuedPageCount, _totalPageCount);
                }
            }
            else
            {
                _progressDialog.SetLine(1, _currentFile.FilePath, true, IntPtr.Zero);
                _progressDialog.SetLine(2, string.Format(Resources.Converter_CompletedPages, _completedPageCount, _currentFile.PageCount), false, IntPtr.Zero);
                if (_initializationDone)
                {
                    _progressDialog.SetProgress(_dequeuedPageCount - _currentFile.PageCount + Math.Min(_currentFile.PageCount, _completedPageCount), _totalPageCount);
                }
            }
        }
    }
}
