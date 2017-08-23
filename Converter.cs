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
        private readonly HashSet<int> _convertedPages = new HashSet<int>();
        private Document _currentFile = null;
        private readonly object _errorLock = new object();
        private readonly Queue<Document> _files = new Queue<Document>();
        private bool _initializationDone = false;
        private int _pagesCompleted = 0;
        private readonly Native.IProgressDialog _progressDialog;
        private readonly HashSet<int> _skippedPages = new HashSet<int>();
        private int _totalPages = 0;

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
                        _totalPages += file.PageCount;
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
                    _convertedPages.Clear();
                    _skippedPages.Clear();
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
                        _pagesCompleted += _currentFile.PageCount;
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
                                        _convertedPages.Clear();
                                        _skippedPages.Clear();
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

        protected void PageConverted(int pageNumber)
        {
            // add the page to the list
            _convertedPages.Add(pageNumber);
            _skippedPages.Remove(pageNumber);
            UpdateDialog();
        }

        protected void PageSkipped(int pageNumber)
        {
            // add the page to the list
            _skippedPages.Add(pageNumber);
            _convertedPages.Remove(pageNumber);
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
                    _progressDialog.SetProgress(_pagesCompleted, _totalPages);
                }
            }
            else
            {
                _progressDialog.SetLine(1, _currentFile.FilePath, true, IntPtr.Zero);
                _progressDialog.SetLine(2, string.Format(Resources.Converter_CompletedPages, _convertedPages.Count, _skippedPages.Count, _currentFile.PageCount), false, IntPtr.Zero);
                if (_initializationDone)
                {
                    _progressDialog.SetProgress(_pagesCompleted - _currentFile.PageCount + Math.Min(_currentFile.PageCount, _convertedPages.Count + _skippedPages.Count), _totalPages);
                }
            }
        }
    }

    public class GhostscriptConverter : Converter
    {
        public static void Run(IEnumerable<string> files, FormatDialog formatDialog)
        {
            // check the argument
            if (files == null)
            {
                throw new ArgumentNullException("files");
            }
            if (formatDialog == null)
            {
                throw new ArgumentNullException("formatDialog");
            }

            // show the format dialog and run the conversion
            if (formatDialog.ShowDialog() == DialogResult.OK)
            {
                new GhostscriptConverter(files, formatDialog.FormatName, formatDialog.Arguments, formatDialog.EvenPages, formatDialog.OddPages).Run();
            }
        }

        private readonly string[] _arguments;
        private readonly string _formatName;
        private readonly bool _evenPages;
        private readonly bool _oddPages;

        private GhostscriptConverter(IEnumerable<string> files, string formatName, string[] arguments, bool evenPages, bool oddPages)
            : base(files)
        {
            _formatName = formatName;
            _arguments = arguments;
            _evenPages = evenPages;
            _oddPages = oddPages;
        }

        protected override DocumentType AllowedTypes
        {
            get { return DocumentType.Any & ~DocumentType.Image; }
        }

        protected override string FormatName
        {
            get { return _formatName; }
        }

        protected override void ConvertFile(Document file)
        {
            // create a new Ghostscript instance
            using (var gs = new Ghostscript(_arguments))
            {
                // set the cancel handler
                gs.Poll += (o, e) =>
                {
                    if (IsCancelled())
                    {
                        e.Cancel = true;
                    }
                };

                // initialize the document and go over all pages
                file.RunInitialize(gs);
                for (var pageNumber = 1; pageNumber <= file.PageCount; pageNumber++)
                {
                    // ensure the page was requested
                    if (pageNumber % 2 == 0 ? _evenPages : _oddPages)
                    {
                        // run ghostscript
                        try
                        {
                            file.RunPage(gs, pageNumber);
                        }
                        catch (OperationCanceledException)
                        {
                            return;
                        }

                        // notify the caller
                        PageConverted(pageNumber);
                    }
                    else
                    {
                        // skip the page
                        PageSkipped(pageNumber);
                    }
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
            get { return "PDF"; }
        }

        protected override void ConvertFile(Document file)
        {
            file.ConvertToPdf(Path.ChangeExtension(file.FilePath, "pdf"), PageConverted, IsCancelled);
        }
    }
}
