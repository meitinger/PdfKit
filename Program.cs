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
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Aufbauwerk.Tools.PdfKit
{
    public static class Program
    {
        [ComVisible(true), Guid("F0ECDC65-CA33-47DA-991B-2DE627A3566F"), ClassInterface(ClassInterfaceType.None)]
        public sealed class CombineDelegate : Native.IExecuteCommand, Native.IObjectWithSelection, Native.IClassFactory
        {
            private readonly CombineForm form;
            private IntPtr itemArray;

            internal CombineDelegate(CombineForm form)
            {
                // store the form
                this.form = form;
            }

            ~CombineDelegate()
            {
                // release the item array
                if (itemArray != IntPtr.Zero)
                {
                    Marshal.Release(itemArray);
                    itemArray = IntPtr.Zero;
                }
            }

            #region IExecuteCommand Methods

            void Native.IExecuteCommand.SetKeyState(uint grfKeyState) { }

            void Native.IExecuteCommand.SetParameters(string pszParameters) { }

            void Native.IExecuteCommand.SetPosition(Point pt) { }

            void Native.IExecuteCommand.SetShowWindow(int nShow) { }

            void Native.IExecuteCommand.SetNoShowUI(bool fNoShowUI) { }

            void Native.IExecuteCommand.SetDirectory(string pszDirectory) { }

            void Native.IExecuteCommand.Execute()
            {
                // show the form and add all selected files
                Native.SetForegroundWindow(form.Handle);
                if (itemArray != IntPtr.Zero)
                {
                    var array = (Native.IShellItemArray)Marshal.GetTypedObjectForIUnknown(itemArray, typeof(Native.IShellItemArray));
                    form.InsertFiles(Enumerable.Range(0, array.GetCount()).Select(i => array.GetItemAt(i).GetDisplayName(Native.SIGDN_FILESYSPATH)), 0);
                }
            }

            #endregion

            #region IObjectWithSelection Methods

            int Native.IObjectWithSelection.SetSelection(IntPtr psia)
            {
                // release a previous selection
                if (itemArray != IntPtr.Zero)
                {
                    var result = Marshal.Release(itemArray);
                    if (Native.FAILED(result))
                    {
                        return result;
                    }
                    itemArray = IntPtr.Zero;
                }

                // set the new selection
                if (psia != IntPtr.Zero)
                {
                    var result = Marshal.AddRef(itemArray);
                    if (Native.FAILED(result))
                    {
                        return result;
                    }
                    itemArray = psia;
                }

                // indicate success
                return Native.S_OK;
            }

            int Native.IObjectWithSelection.GetSelection(Guid riid, out IntPtr ppv)
            {
                // return an error if no selection was set
                if (itemArray == IntPtr.Zero)
                {
                    ppv = IntPtr.Zero;
                    return Native.E_ILLEGAL_METHOD_CALL;
                }

                // query and return the interface
                return Marshal.QueryInterface(itemArray, ref riid, out ppv);
            }

            #endregion

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
        }

        private enum TaskToPerform
        {
            Extract,
            Combine,
            CombineCom,
            CombineDirectory,
        }

        [STAThread]
        private static void Main(string[] args)
        {
            // enabled styles and set the text rendering
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // make sure the arguments are valid
            TaskToPerform taskToPerform;
            if (args.Length < 1 || !Enum.TryParse(args[0], true, out taskToPerform) || taskToPerform != TaskToPerform.Combine && args.Length != 2)
            {
                // show the usage dialog
                MessageBox.Show(string.Format("USAGE:\n  {0} {1} <file>\n  {0} {2} [<file> [...]]\n  {0} {3} <directory>\n  {0} {4} -Embedded", Environment.GetCommandLineArgs()[0], TaskToPerform.Extract, TaskToPerform.Combine, TaskToPerform.CombineDirectory, TaskToPerform.CombineCom), null, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // create the main form based on the task
            Form form;
            switch (taskToPerform)
            {
                case TaskToPerform.Extract:
                {
                    form = new ExtractForm(args[1]);
                    break;
                }
                case TaskToPerform.Combine:
                {
                    form = args.Length == 1 ? new CombineForm() : new CombineForm(args.Skip(1));
                    break;
                }
                case TaskToPerform.CombineCom:
                {
                    RunCombineComServer();
                    return;
                }
                case TaskToPerform.CombineDirectory:
                {
                    form = new CombineForm(Directory.EnumerateFiles(args[1], "*.pdf", SearchOption.AllDirectories));
                    break;
                }
                default:
                {
                    throw new NotImplementedException();
                }
            }

            // run the application
            Application.Run(form);
        }

        private static void RunCombineComServer()
        {
            // register the factory for the delegate an run the com server
            var form = new CombineForm();
            uint cookie;
            Native.CoRegisterClassObject(typeof(CombineDelegate).GUID, new CombineDelegate(form), Native.CLSCTX_LOCAL_SERVER, Native.REGCLS_SINGLEUSE, out cookie);
            RuntimeHelpers.PrepareConstrainedRegions();
            try { Application.Run(form); }
            finally { Native.CoRevokeClassObject(cookie); }
        }
    }
}
