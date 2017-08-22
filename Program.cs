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
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Aufbauwerk.Tools.PdfKit.Properties;

namespace Aufbauwerk.Tools.PdfKit
{
    internal static class Program
    {
        [ComVisible(true), Guid("F0ECDC65-CA33-47DA-991B-2DE627A3566F"), ClassInterface(ClassInterfaceType.None)]
        private sealed class EmbeddedComServer : Native.IInitializeCommand, Native.IExecuteCommand, Native.IObjectWithSelection, Native.IClassFactory
        {
            private readonly ManualResetEventSlim _waitEvent = new ManualResetEventSlim();

            public string Command { get; private set; }

            public string[] Files { get; private set; }

            public bool Wait(int millisecondsTimeout)
            {
                // wait for ready
                return _waitEvent.Wait(millisecondsTimeout);
            }

            #region IClassFactory Methods

            int Native.IClassFactory.CreateInstance(IntPtr pUnkOuter, Guid riid, out IntPtr ppvObject)
            {
                // query the current instance for a given interface
                ppvObject = IntPtr.Zero;
                var pUnk = pUnkOuter != IntPtr.Zero ? Marshal.CreateAggregatedObject(pUnkOuter, this) : Marshal.GetIUnknownForObject(this);
                try { return Marshal.QueryInterface(pUnk, ref riid, out ppvObject); }
                finally { Marshal.Release(pUnk); }
            }

            int Native.IClassFactory.LockServer(bool fLock)
            {
                // lock or unlock the server
                return Native.CoLockObjectExternal(this, fLock, true);
            }

            #endregion

            #region IExecuteCommand Methods

            void Native.IExecuteCommand.SetKeyState(uint grfKeyState) { }

            void Native.IExecuteCommand.SetParameters(string pszParameters) { }

            void Native.IExecuteCommand.SetPosition(Point pt) { }

            void Native.IExecuteCommand.SetShowWindow(int nShow) { }

            void Native.IExecuteCommand.SetNoShowUI(bool fNoShowUI) { }

            void Native.IExecuteCommand.SetDirectory(string pszDirectory) { }

            void Native.IExecuteCommand.Execute()
            {
                // signal ready
                _waitEvent.Set();
            }

            #endregion

            #region IInitializeCommand Methods

            void Native.IInitializeCommand.Initialize(string pszCommandName, IntPtr ppb)
            {
                // store the verb name
                Command = pszCommandName;
            }

            #endregion

            #region IObjectWithSelection Methods

            void Native.IObjectWithSelection.SetSelection(IntPtr psia)
            {
                // store the file array
                var array = (Native.IShellItemArray)Marshal.GetTypedObjectForIUnknown(psia, typeof(Native.IShellItemArray));
                var files = new string[array.GetCount()];
                for (var i = 0; i < files.Length; i++)
                {
                    files[i] = array.GetItemAt(i).GetDisplayName(Native.SIGDN_FILESYSPATH);
                }
                Marshal.ReleaseComObject(array);
                Files = files;
            }

            void Native.IObjectWithSelection.GetSelection(Guid riid, out IntPtr ppv)
            {
                throw new NotImplementedException();
            }

            #endregion
        }

        private enum CommandLineTask
        {
            Combine = 1,
            CombineDirectory,
            Extract,
            View,
        }

        private enum ComServerTask
        {
            Combine = 1,
            ConvertToBmp,
            ConvertToEps,
            ConvertToJpeg,
            ConvertToPdf,
            ConvertToPng,
            ConvertToPs,
            ConvertToTiff,
        }

        private const string ComEmbedding = "-Embedding";

        [STAThread]
        private static void Main(string[] args)
        {
            try
            {
                // enabled styles and set the text rendering
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                // check the arguments
                if (args.Length > 0)
                {
                    if (string.Equals(args[0], ComEmbedding, StringComparison.OrdinalIgnoreCase))
                    {
                        if (args.Length == 1)
                        {
                            // create and register the com server
                            var comServer = new EmbeddedComServer();
                            uint cookie;
                            Native.CoRegisterClassObject(comServer.GetType().GUID, comServer, Native.CLSCTX_LOCAL_SERVER, Native.REGCLS_SINGLEUSE, out cookie);
                            try
                            {
                                // wait for it to be ready
                                if (!comServer.Wait(10000))
                                {
                                    MessageBox.Show(Resources.Program_ServerTimeout, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    return;
                                }
                            }
                            finally
                            {
                                // unregister the com server
                                Native.CoRevokeClassObject(cookie);
                            }

                            // check the files and parse the command verb
                            if (comServer.Files == null)
                            {
                                MessageBox.Show(Resources.Program_NoFiles, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                            if (comServer.Command == null)
                            {
                                MessageBox.Show(Resources.Program_NoCommand, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                            ComServerTask task;
                            if (!Enum.TryParse(comServer.Command, true, out task))
                            {
                                MessageBox.Show(string.Format(Resources.Program_UnknownVerb, comServer.Command, string.Join(", ", Enum.GetNames(typeof(ComServerTask)))), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }

                            // perform the task
                            switch (task)
                            {
                                case ComServerTask.Combine:
                                {
                                    Application.Run(new CombineForm(comServer.Files));
                                    break;
                                }
                                case ComServerTask.ConvertToBmp:
                                {
                                    GhostscriptConverter.Run(comServer.Files, new BmpFormatDialog());
                                    break;
                                }
                                case ComServerTask.ConvertToJpeg:
                                {
                                    GhostscriptConverter.Run(comServer.Files, new JpegFormatDialog());
                                    break;
                                }
                                case ComServerTask.ConvertToPdf:
                                {
                                    PdfConverter.Run(comServer.Files);
                                    break;
                                }
                                case ComServerTask.ConvertToPng:
                                {
                                    GhostscriptConverter.Run(comServer.Files, new PngFormatDialog());
                                    break;
                                }
                                case ComServerTask.ConvertToTiff:
                                {
                                    GhostscriptConverter.Run(comServer.Files, new TiffFormatDialog());
                                    break;
                                }
                                default:
                                {
                                    throw new NotImplementedException();
                                }
                            }
                            return;
                        }
                    }
                    else
                    {
                        // try to get the command line task
                        CommandLineTask task;
                        if (Enum.TryParse(args[0], true, out task))
                        {
                            switch (task)
                            {
                                case CommandLineTask.Combine:
                                {
                                    // show the combine form and add all given files
                                    Application.Run(args.Length > 1 ? new CombineForm(args.Skip(1)) : new CombineForm());
                                    return;
                                }
                                case CommandLineTask.CombineDirectory:
                                {
                                    if (args.Length == 2)
                                    {
                                        // show the combine form and add all pdfs from the given directory
                                        Application.Run(new CombineForm(Directory.EnumerateFiles(args[1], "*.pdf", SearchOption.AllDirectories)));
                                        return;
                                    }
                                    break;
                                }
                                case CommandLineTask.Extract:
                                {
                                    if (args.Length == 2)
                                    {
                                        // show the extract form with the given file
                                        Application.Run(new ExtractForm(args[1]));
                                        return;
                                    }
                                    break;
                                }
                                case CommandLineTask.View:
                                {
                                    if (args.Length == 2)
                                    {
                                        // show the document
                                        Application.Run(new ViewForm(args[1]));
                                        return;
                                    }
                                    break;
                                }
                                default:
                                {
                                    throw new NotImplementedException();
                                }
                            }
                        }
                    }
                }

                // show the usage dialog
                MessageBox.Show(string.Format(Resources.Program_Usage, Environment.GetCommandLineArgs()[0], ComEmbedding, CommandLineTask.Combine, CommandLineTask.CombineDirectory, CommandLineTask.Extract, CommandLineTask.View), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception e)
            {
                // show the error message
                MessageBox.Show(e.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
