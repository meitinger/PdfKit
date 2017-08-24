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
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Aufbauwerk.Tools.PdfKit.Properties;

namespace Aufbauwerk.Tools.PdfKit
{
    public abstract class Converter
    {
        private bool _aborted = false;
        private Document _currentFile = null;
        private int _completedPageCount = 0;
        private readonly object _errorLock = new object();
        private readonly Queue<Document> _files = new Queue<Document>();
        private bool _initializationDone = false;
        private int _dequeuedPageCount = 0;
        private readonly Native.IProgressDialog _progressDialog;
        private int _totalPageCount = 0;

        protected Converter(IEnumerable<string> files)
        {
            // create the process dialog
            _progressDialog = (Native.IProgressDialog)Activator.CreateInstance(Type.GetTypeFromCLSID(Native.CLSID_ProgressDialog, true));
            _progressDialog.SetTitle(Title);
            _progressDialog.SetCancelMsg(Resources.Converter_CancelMessage, IntPtr.Zero);

            // start looking for files
            ThreadPool.QueueUserWorkItem(Initialize, files);
        }

        protected abstract DocumentType AllowedTypes { get; }

        protected abstract string FormatName { get; }

        protected string Title
        {
            get { return string.Format(Resources.Converter_DialogTitle, Application.ProductName, FormatName); }
        }

        protected abstract void ConvertFile(Document file);

        private void Initialize(object files)
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
                        file = Document.FromFile(filePath, AllowedTypes);
                    }
                    catch (Exception e)
                    {
                        // show the error dialog and let the user pick
                        lock (_errorLock)
                        {
                            if (_aborted)
                            {
                                return;
                            }
                            switch (MessageBox.Show(string.Format(Resources.Converter_LoadFileError, filePath, e.Message), Title, MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Warning))
                            {
                                case DialogResult.Abort:
                                {
                                    _aborted = true;
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
                    }

                    // enqueue the file and notify the main thread
                    lock (_files)
                    {
                        if (_aborted)
                        {
                            return;
                        }
                        _files.Enqueue(file);
                        _totalPageCount += file.PageCount;
                        Monitor.Pulse(_files);
                    }
                }
            }
            finally
            {
                // notify the main thread that initialisation is complete
                lock (_files)
                {
                    _initializationDone = true;
                    Monitor.Pulse(_files);
                }
            }
        }

        protected void Run()
        {
            // show the process dialog
            _progressDialog.StartProgressDialog(IntPtr.Zero, IntPtr.Zero, Native.PROGDLG_AUTOTIME, IntPtr.Zero);
            try
            {
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
                        ConvertFile(_currentFile);
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
                            lock (_errorLock)
                            {
                                if (_aborted)
                                {
                                    return;
                                }
                                switch (MessageBox.Show(string.Format(Resources.Converter_ConvertFileError, _currentFile.FilePath, e.Message), Title, MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Warning))
                                {
                                    case DialogResult.Abort:
                                    {
                                        _aborted = true;
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
                // hide the dialog
                _progressDialog.StopProgressDialog();
            }
        }

        protected bool IsCancelled()
        {
            // check if the operation has been aborted
            return _aborted || _progressDialog.HasUserCancelled();
        }

        protected void PageCompleted()
        {
            // increment the counter and update the dialogs
            _completedPageCount++;
            UpdateDialog();
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

    public class GhostscriptConverter : Converter
    {
        public class Format
        {
            private static readonly List<Format> _formats = new List<Format>();

            public static readonly Format Bmp = new Format(6, Resources.Converter_FormatBmp, new BmpFormatDialog());
            //public static readonly Format Eps = new Format(2, Resources.Converter_FormatEps, new EpsFormatDialog(), DocumentType.PortableDocumentFormat | DocumentType.PostScript);
            public static readonly Format Jpeg = new Format(4, Resources.Converter_FormatJpeg, new JpegFormatDialog());
            public static readonly Format Png = new Format(3, Resources.Converter_FormatPdf, new PngFormatDialog());
            //public static readonly Format Ps = new Format(1, Resources.Converter_FormatPs, new PsFormatDialog(), DocumentType.PortableDocumentFormat | DocumentType.EncapsulatedPostScript);
            public static readonly Format Tiff = new Format(5, Resources.Converter_FormatTiff, new TiffFormatDialog());

            private readonly FormatDialog _dialog;

            private Format(int priority, string name, FormatDialog dialog, DocumentType allowedTypes = DocumentType.Any & ~DocumentType.Image)
            {
                Priority = priority;
                Name = name;
                _dialog = dialog;
                AllowedTypes = allowedTypes;
                _formats.Add(this);
            }

            public DocumentType AllowedTypes { get; private set; }

            public string[] Arguments { get; private set; }

            public string FileExtension
            {
                get { return _dialog.FileExtension; }
            }

            public string Name { get; private set; }

            public int Priority { get; private set; }

            public bool SupportsSingleFile
            {
                get { return _dialog.SupportsSingleFile; }
            }

            public bool ShowDialog(string useFileName = null)
            {
                // show the format dialog
                _dialog.UseFileName = useFileName;
                if (_dialog.ShowDialog() == DialogResult.OK)
                {
                    Arguments = _dialog.Arguments;
                    return true;
                }
                return false;
            }
        }

        public static void Run(IEnumerable<string> files, Format format)
        {
            // check the argument
            if (files == null)
            {
                throw new ArgumentNullException("files");
            }
            if (format == null)
            {
                throw new ArgumentNullException("format");
            }

            // show the format dialog and run the conversion
            if (format.ShowDialog())
            {
                new GhostscriptConverter(files, format).Run();
            }
        }

        private readonly Format _format;

        private GhostscriptConverter(IEnumerable<string> files, Format format)
            : base(files)
        {
            _format = format;
        }

        protected override DocumentType AllowedTypes
        {
            get { return _format.AllowedTypes; }
        }

        protected override string FormatName
        {
            get { return _format.Name; }
        }

        protected override void ConvertFile(Document file)
        {
            // create a new Ghostscript instance
            using (var gs = new Ghostscript(_format.Arguments))
            {
                // set the cancel handler
                gs.Poll += (o, e) => e.Cancel = IsCancelled();

                // initialize the document and go over all pages
                file.RunInitialize(gs);
                for (var pageNumber = 1; pageNumber <= file.PageCount; pageNumber++)
                {
                    // run the page and notify the caller
                    file.RunPage(gs, pageNumber);
                    PageCompleted();
                }
            }
        }
    }

    public class PdfConverter : Converter
    {
        public static void Run(IEnumerable<string> files)
        {
            // check the input and convert the files
            if (files == null)
            {
                throw new ArgumentNullException("files");
            }
            new PdfConverter(files).Run();
        }

        private PdfConverter(IEnumerable<string> files) : base(files) { }

        protected override DocumentType AllowedTypes
        {
            get { return DocumentType.Any & ~DocumentType.PortableDocumentFormat; }
        }

        protected override string FormatName
        {
            get { return Resources.Converter_FormatPdf; }
        }

        protected override void ConvertFile(Document file)
        {
            file.ConvertToPdf(Path.ChangeExtension(file.FilePath, "pdf"), PageCompleted, IsCancelled);
        }
    }
}
