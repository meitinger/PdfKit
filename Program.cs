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
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Windows.Forms;
using Ghostscript.NET;

namespace Aufbauwerk.Tools.PdfKit
{
    class Program
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetShortPathName(string path, StringBuilder shortPath, int shortPathLength);

        [DllImport("Shell32.dll", ExactSpelling = true, PreserveSig = false)]
        private static extern void SHOpenFolderAndSelectItems([In] IntPtr pidlFolder, [In] int cidl, [In] IntPtr[] apidl, [In] uint dwFlags);

        [DllImport("Shell32.dll", ExactSpelling = true, PreserveSig = false)]
        [return: MarshalAs(UnmanagedType.Interface)]
        private static extern IShellFolder SHGetDesktopFolder();

        [ComImport, Guid("000214E6-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IShellFolder
        {
            void ParseDisplayName([In] IntPtr hwnd, [In] IBindCtx pbc, [MarshalAs(UnmanagedType.LPWStr)] [In] string pszDisplayName, [In, Optional] IntPtr pchEaten, out IntPtr ppidl, [In, Optional] IntPtr pdwAttributes);

            void Dummy();

            [return: MarshalAs(UnmanagedType.Interface)]
            IShellFolder BindToObject([In] IntPtr pidl, [In] IBindCtx pbc, [In, MarshalAs(UnmanagedType.LPStruct)] Guid riid);
        }

        private static Guid IID_IShellFolder = typeof(IShellFolder).GUID;

        internal static void OpenFolderAndSelectItems(string folder, params string[] files)
        {
            // check the input and get the folder pidl
            if (folder == null)
                throw new ArgumentNullException("folder");
            var desktop = SHGetDesktopFolder();
            var folderPidl = IntPtr.Zero;
            desktop.ParseDisplayName(IntPtr.Zero, null, folder, IntPtr.Zero, out folderPidl, IntPtr.Zero);
            try
            {
                // check if there are any files to select
                if (files != null && files.Length > 0)
                {
                    // get the folder object
                    var folderObject = desktop.BindToObject(folderPidl, null, IID_IShellFolder);
                    var filesPidl = new List<IntPtr>();
                    try
                    {
                        // get all child pidls
                        foreach (var file in files)
                        {
                            var filePidl = IntPtr.Zero;
                            folderObject.ParseDisplayName(IntPtr.Zero, null, file, IntPtr.Zero, out filePidl, IntPtr.Zero);
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

        internal static readonly GhostscriptVersionInfo GhostscriptVersion = new GhostscriptVersionInfo("gsdll32.dll");

        private enum TaskToPerform
        {
            Extract,
            Combine,
            CombineDirectory,
            CombineIdList
        }

        [STAThread]
        private static void Main(string[] args)
        {
            // enabled styles and set the text rendering
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // make sure the arguments are valid
            TaskToPerform taskToPerform;
            if (args.Length != 2 || !Enum.TryParse(args[0], true, out taskToPerform))
            {
                // show the usage dialog
                MessageBox.Show(string.Format("USAGE: {0} <{1}> <arg>", Environment.GetCommandLineArgs()[0], string.Join("|", Enum.GetNames(typeof(TaskToPerform)))), null, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // create the main form based on the task
            Form form;
            switch(taskToPerform){
                case TaskToPerform.Extract:
                    form = new ExtractForm(args[1]);
                    break;
                case TaskToPerform.Combine:
                    form = new CombineForm();
                    break;
                case TaskToPerform.CombineDirectory:
                    form = new CombineForm(Directory.EnumerateFiles(args[1], "*.pdf", SearchOption.AllDirectories));
                    break;
        //        case TaskToPerform.CombineIdList:

          //          break;
                default:
                    throw new NotImplementedException();
            }

            // run the application
            Application.Run(form);
        }
    }
}
