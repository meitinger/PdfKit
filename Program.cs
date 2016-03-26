/* Copyright (C) 2016, Manuel Meitinger
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
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Windows.Forms;
using Ghostscript.NET;

namespace Aufbauwerk.Tools.PdfKit
{
    [ComImport]
    [Guid("7F9185B0-CB92-43c5-80A9-92277A4F7B54")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IExecuteCommand
    {
        void SetKeyState([In] uint grfKeyState);
        void SetParameters([In, MarshalAs(UnmanagedType.LPWStr)] string pszParameters);
        void SetPosition([In] Point pt);
        void SetShowWindow([In] int nShow);
        void SetNoShowUI([In] bool fNoShowUI);
        void SetDirectory([In, MarshalAs(UnmanagedType.LPWStr)] string pszDirectory);
        void Execute();
    }

    [ComImport]
    [Guid("1c9cd5bb-98e9-4491-a60f-31aacc72b83c")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IObjectWithSelection
    {
        void SetSelection([In] IntPtr psia);
        [PreserveSig]
        int GetSelection([In, MarshalAs(UnmanagedType.LPStruct)] Guid riid, [Out] out IntPtr ppv);
    }

    [ComImport]
    [Guid("00000001-0000-0000-C000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IClassFactory
    {
        [PreserveSig]
        int CreateInstance([In] IntPtr pUnkOuter, [In, MarshalAs(UnmanagedType.LPStruct)] Guid riid, [Out] out IntPtr ppvObject);
        void LockServer([In] bool fLock);
    }

    [ComVisible(true)]
    [Guid("F0ECDC65-CA33-47DA-991B-2DE627A3566F")]
    [ClassInterface(ClassInterfaceType.None)]
    public sealed class CombineDelegate : IExecuteCommand, IObjectWithSelection, IClassFactory
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

        void IExecuteCommand.SetKeyState(uint grfKeyState) { }
        void IExecuteCommand.SetParameters(string pszParameters) { }
        void IExecuteCommand.SetPosition(Point pt) { }
        void IExecuteCommand.SetShowWindow(int nShow) { }
        void IExecuteCommand.SetNoShowUI(bool fNoShowUI) { }
        void IExecuteCommand.SetDirectory(string pszDirectory) { }

        void IExecuteCommand.Execute()
        {
            // add all selected files
            if (itemArray != IntPtr.Zero)
                foreach (var file in Program.EnumerateShellItemArray(itemArray))
                    form.AddFile(file);
        }

        void IObjectWithSelection.SetSelection(IntPtr psia)
        {
            // release a previous selection
            if (itemArray != IntPtr.Zero)
                Marshal.Release(itemArray);

            // set the new selection
            itemArray = psia;
            if (psia != IntPtr.Zero)
                Marshal.AddRef(itemArray);
        }

        int IObjectWithSelection.GetSelection(Guid riid, out IntPtr ppv)
        {
            // return an error if no selection was set
            if (itemArray == IntPtr.Zero)
            {
                ppv = IntPtr.Zero;
                return Marshal.GetHRForException(new InvalidOperationException());
            }

            // query and return the interface
            return Marshal.QueryInterface(itemArray, ref riid, out ppv);
        }

        int IClassFactory.CreateInstance(IntPtr pUnkOuter, Guid riid, out IntPtr ppvObject)
        {
            // query the current instance for a given interface
            ppvObject = IntPtr.Zero;
            var pUnk = pUnkOuter != IntPtr.Zero ? Marshal.CreateAggregatedObject(pUnkOuter, this) : Marshal.GetIUnknownForObject(this);
            try { return Marshal.QueryInterface(pUnk, ref riid, out ppvObject); }
            finally { Marshal.Release(pUnk); }
        }

        void IClassFactory.LockServer(bool fLock) { }
    }

    public static class Program
    {
        private const uint SIGDN_FILESYSPATH = 0x80058000;
        private const uint CLSCTX_LOCAL_SERVER = 0x4;
        private const uint REGCLS_SINGLEUSE = 0;

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetShortPathName(string path, StringBuilder shortPath, int shortPathLength);

        [DllImport("shell32.dll", ExactSpelling = true, PreserveSig = false)]
        private static extern void SHOpenFolderAndSelectItems([In] IntPtr pidlFolder, [In] int cidl, [In] IntPtr[] apidl, [In] uint dwFlags);

        [DllImport("shell32.dll", ExactSpelling = true, PreserveSig = false)]
        [return: MarshalAs(UnmanagedType.Interface)]
        private static extern IShellFolder SHGetDesktopFolder();

        [DllImport("ole32.dll", ExactSpelling = true, PreserveSig = false)]
        private static extern void CoRegisterClassObject([MarshalAs(UnmanagedType.LPStruct)] Guid rclsid, [MarshalAs(UnmanagedType.Interface)] IClassFactory pUnk, uint dwClsContext, uint flags, out uint lpdwRegister);

        [DllImport("ole32.dll", ExactSpelling = true, PreserveSig = false)]
        private static extern void CoRevokeClassObject(uint dwRegister);

        [ComImport]
        [Guid("000214E6-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IShellFolder
        {
            void ParseDisplayName([In] IntPtr hwnd, [In] IntPtr pbc, [In, MarshalAs(UnmanagedType.LPWStr)] string pszDisplayName, [In] IntPtr pchEaten, [Out] out IntPtr ppidl, [In] IntPtr pdwAttributes);
            void Dummy_EnumObjects();
            [return: MarshalAs(UnmanagedType.Interface)]
            object BindToObject([In] IntPtr pidl, [In] IntPtr pbc, [In, MarshalAs(UnmanagedType.LPStruct)] Guid riid);
            void Dummy_BindToStorage();
            void Dummy_CompareIDs();
            void Dummy_CreateViewObject();
            void Dummy_GetAttributesOf();
            void Dummy_GetUIObjectOf();
            void Dummy_GetDisplayNameOf();
            void Dummy_SetNameOf();
        }

        [ComImport]
        [Guid("b63ea76d-1f85-456f-a19c-48159efa858b")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IShellItemArray
        {
            void Dummy_BindToHandler();
            void Dummy_GetPropertyStore();
            void Dummy_GetPropertyDescriptionList();
            void Dummy_GetAttributes();
            int GetCount();
            [return: MarshalAs(UnmanagedType.Interface)]
            IShellItem GetItemAt([In] int dwIndex);
            void Dummy_EnumItems();
        }

        [ComImport]
        [Guid("43826d1e-e718-42ee-bc55-a1e261c37bfe")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IShellItem
        {
            void Dummy_BindToHandler();
            void Dummy_GetParent();
            [return: MarshalAs(UnmanagedType.LPWStr)]
            string GetDisplayName([In] uint sigdnName);
            void Dummy_GetAttributes();
            void Dummy_Compare();
        }

        private enum TaskToPerform
        {
            Extract,
            Combine,
            CombineCom,
            CombineDirectory,
        }

        internal static GhostscriptVersionInfo GhostscriptVersion
        {
            get
            {
                // build the ghostscript dll path
                return new GhostscriptVersionInfo(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "gsdll32.dll"));
            }
        }

        internal static IEnumerable<string> EnumerateShellItemArray(IntPtr psia)
        {
            // enumerate over a shell item array
            if (psia == IntPtr.Zero)
                throw new ArgumentNullException("psia");
            var array = (IShellItemArray)Marshal.GetTypedObjectForIUnknown(psia, typeof(IShellItemArray));
            for (var i = 0; i < array.GetCount(); i++)
                yield return array.GetItemAt(i).GetDisplayName(SIGDN_FILESYSPATH);
        }

        internal static void OpenFolderAndSelectItems(string folder, params string[] files)
        {
            // check the input and get the folder pidl
            if (folder == null)
                throw new ArgumentNullException("folder");
            var desktop = SHGetDesktopFolder();
            var folderPidl = IntPtr.Zero;
            desktop.ParseDisplayName(IntPtr.Zero, IntPtr.Zero, folder, IntPtr.Zero, out folderPidl, IntPtr.Zero);
            try
            {
                // check if there are any files to select
                if (files != null && files.Length > 0)
                {
                    // get the folder object
                    var folderObject = (IShellFolder)desktop.BindToObject(folderPidl, IntPtr.Zero, typeof(IShellFolder).GUID);
                    var filesPidl = new List<IntPtr>();
                    try
                    {
                        // get all child pidls
                        foreach (var file in files)
                        {
                            var filePidl = IntPtr.Zero;
                            folderObject.ParseDisplayName(IntPtr.Zero, IntPtr.Zero, file, IntPtr.Zero, out filePidl, IntPtr.Zero);
                            filesPidl.Add(filePidl);
                        }

                        // show the folder and select the items
                        SHOpenFolderAndSelectItems(folderPidl, filesPidl.Count, filesPidl.ToArray(), 0);
                    }
                    finally
                    {
                        // free the child pidls
                        foreach (var filePidl in filesPidl)
                            Marshal.FreeCoTaskMem(filePidl);
                    }
                }
                else
                    // simply show the folder
                    SHOpenFolderAndSelectItems(folderPidl, 0, null, 0);
            }
            finally
            {
                // free the folder pidl
                Marshal.FreeCoTaskMem(folderPidl);
            }
        }

        internal static string GetShortPathName(string path)
        {
            // return the 8.3 version of the path
            if (path == null)
                throw new ArgumentNullException("path");
            var buffer = new StringBuilder(300);
            int len;
            while ((len = GetShortPathName(path, buffer, buffer.Capacity)) > buffer.Capacity)
                buffer.EnsureCapacity(len);
            if (len == 0)
                throw new Win32Exception();
            return buffer.ToString();
        }

        private static void RunCombineComServer()
        {
            // register the factory for the delegate an run the com server
            var form = new CombineForm();
            uint cookie;
            CoRegisterClassObject(typeof(CombineDelegate).GUID, new CombineDelegate(form), CLSCTX_LOCAL_SERVER, REGCLS_SINGLEUSE, out cookie);
            RuntimeHelpers.PrepareConstrainedRegions();
            try { Application.Run(form); }
            finally { CoRevokeClassObject(cookie); }
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
                    form = new ExtractForm(args[1]);
                    break;
                case TaskToPerform.Combine:
                    form = args.Length == 1 ? new CombineForm() : new CombineForm(args.Skip(1));
                    break;
                case TaskToPerform.CombineCom:
                    RunCombineComServer();
                    return;
                case TaskToPerform.CombineDirectory:
                    form = new CombineForm(Directory.EnumerateFiles(args[1], "*.pdf", SearchOption.AllDirectories));
                    break;
                default:
                    throw new NotImplementedException();
            }

            // run the application
            Application.Run(form);
        }
    }
}
