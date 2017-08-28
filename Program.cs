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

        private static readonly SortedDictionary<string, Action<IEnumerable<string>>> _multiFileTasks = new SortedDictionary<string, Action<IEnumerable<string>>>(StringComparer.OrdinalIgnoreCase)
        {
            { "Combine", files => Application.Run(new CombineForm(files)) },
            { "ConvertToBmp", files => Converter.Run(files, ConvertFormat.Bmp) },
            { "ConvertToEps", files => Converter.Run(files, ConvertFormat.Eps) },
            { "ConvertToJpeg", files => Converter.Run(files, ConvertFormat.Jpeg) },
            { "ConvertToPdf", files => Converter.Run(files, ConvertFormat.Pdf) },
            { "ConvertToPng", files => Converter.Run(files, ConvertFormat.Png) },
            { "ConvertToTiff", files => Converter.Run(files, ConvertFormat.Tiff) },
            { "ConvertToPs", files => Converter.Run(files, ConvertFormat.Ps) },
        };

        private static readonly SortedDictionary<string, Action<string>> _singleFileTasks = new SortedDictionary<string, Action<string>>(StringComparer.OrdinalIgnoreCase)
        {
            { "View", file=> Application.Run(new ViewForm(file)) },
            { "Extract", file => Application.Run(new ExtractForm(file)) },
            { "CombineDirectory", file => Application.Run(new CombineForm(Directory.EnumerateFiles(file, "*.pdf", SearchOption.AllDirectories))) },
        };

        private static readonly SortedDictionary<string, Action> _standaloneTasks = new SortedDictionary<string, Action>(StringComparer.OrdinalIgnoreCase)
        {
            { "-Embedding", RunComServer },
            { "Combine", () => Application.Run(new CombineForm()) },
        };

        [STAThread]
        private static void Main(string[] args)
        {
            try
            {
                // enabled styles and set the text rendering
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                // check the number of arguments
                switch (args.Length)
                {
                    case 0:
                    {
                        // go to usage
                        break;
                    }
                    case 1:
                    {
                        // try to find and run the standalone task
                        Action action;
                        if (_standaloneTasks.TryGetValue(args[0], out action))
                        {
                            action();
                            return;
                        }
                        break;
                    }
                    case 2:
                    {
                        // try to run a single file task or a multi file task with only one file
                        Action<string> action;
                        if (_singleFileTasks.TryGetValue(args[0], out action))
                        {
                            action(args[1]);
                            return;
                        }
                        goto default;
                    }
                    default:
                    {
                        // try to run a multi file task with all remaining arguments
                        Action<IEnumerable<string>> action;
                        if (_multiFileTasks.TryGetValue(args[0], out action))
                        {
                            action(args.Skip(1));
                            return;
                        }
                        break;
                    }
                }

                // show the usage dialog
                MessageBox.Show(string.Format(Resources.Program_Usage, Path.GetFileNameWithoutExtension(Application.ExecutablePath), string.Join(Resources.Program_UsageSeparator, _standaloneTasks.Keys), string.Join(Resources.Program_UsageSeparator, _singleFileTasks.Keys), string.Join(Resources.Program_UsageSeparator, _multiFileTasks.Keys)), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception e)
            {
                // show the error message
                MessageBox.Show(e.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void RunComServer()
        {
            // create and register the com server
            Application.OleRequired();
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
            Action<IEnumerable<string>> action;
            if (!_multiFileTasks.TryGetValue(comServer.Command, out action))
            {
                MessageBox.Show(string.Format(Resources.Program_UnknownVerb, comServer.Command, string.Join(", ", _multiFileTasks)), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // run the action
            action(comServer.Files);
        }
    }
}
