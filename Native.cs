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
using System.Runtime.InteropServices;
using System.Text;

namespace Aufbauwerk.Tools.PdfKit
{
    public class Native
    {
        private static class Dll
        {
            public const string Kernel32 = "Kernel32.dll";
            public const string Ole32 = "Ole32.dll";
            public const string Shell32 = "Shell32.dll";
            public const string User32 = "User32.dll";
        }

        [ComImport, Guid("00000001-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IClassFactory
        {
            [PreserveSig]
            int CreateInstance([In] IntPtr pUnkOuter, [In, MarshalAs(UnmanagedType.LPStruct)] Guid riid, [Out] out IntPtr ppvObject);
            [PreserveSig]
            int LockServer([In] bool fLock);
        }

        [ComImport, Guid("7F9185B0-CB92-43c5-80A9-92277A4F7B54"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
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

        [ComImport, Guid("1c9cd5bb-98e9-4491-a60f-31aacc72b83c"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IObjectWithSelection
        {
            [PreserveSig]
            int SetSelection([In] IntPtr psia);
            [PreserveSig]
            int GetSelection([In, MarshalAs(UnmanagedType.LPStruct)] Guid riid, [Out] out IntPtr ppv);
        }

        [ComImport, Guid("000214E6-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface IShellFolder
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

        [ComImport, Guid("43826d1e-e718-42ee-bc55-a1e261c37bfe"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface IShellItem
        {
            void Dummy_BindToHandler();
            void Dummy_GetParent();
            [return: MarshalAs(UnmanagedType.LPWStr)]
            string GetDisplayName([In] uint sigdnName);
            void Dummy_GetAttributes();
            void Dummy_Compare();
        }

        [ComImport, Guid("b63ea76d-1f85-456f-a19c-48159efa858b"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface IShellItemArray
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

        internal const uint CLSCTX_LOCAL_SERVER = 0x4;
        internal const int E_ILLEGAL_METHOD_CALL = -2147483634;
        internal const uint REGCLS_SINGLEUSE = 0;
        internal const int S_OK = 0;
        internal const uint SIGDN_FILESYSPATH = 0x80058000;
        
        [DllImport(Dll.Ole32, ExactSpelling = true, CharSet = CharSet.Unicode, PreserveSig = false)]
        internal static extern int CoLockObjectExternal([MarshalAs(UnmanagedType.IUnknown)] object pUnk, bool fLock, bool fLastUnlockReleases);

        [DllImport(Dll.Ole32, ExactSpelling = true, CharSet = CharSet.Unicode, PreserveSig = false)]
        internal static extern void CoRegisterClassObject([MarshalAs(UnmanagedType.LPStruct)] Guid rclsid, [MarshalAs(UnmanagedType.Interface)] IClassFactory pUnk, uint dwClsContext, uint flags, out uint lpdwRegister);

        [DllImport(Dll.Ole32, ExactSpelling = true, CharSet = CharSet.Unicode, PreserveSig = false)]
        internal static extern void CoRevokeClassObject(uint dwRegister);

        internal static bool FAILED(int hr)
        {
            return hr < 0;
        }

        [DllImport(Dll.Kernel32, ExactSpelling = true, CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern int GetShortPathNameW(string lpszLongPath, StringBuilder lpszShortPath, int cchBuffer);

        [DllImport(Dll.User32, ExactSpelling = true, CharSet = CharSet.Unicode)]
        internal static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport(Dll.Shell32, ExactSpelling = true, CharSet = CharSet.Unicode, PreserveSig = false)]
        [return: MarshalAs(UnmanagedType.Interface)]
        internal static extern IShellFolder SHGetDesktopFolder();

        [DllImport(Dll.Shell32, ExactSpelling = true, CharSet = CharSet.Unicode, PreserveSig = false)]
        internal static extern void SHOpenFolderAndSelectItems([In] IntPtr pidlFolder, [In] int cidl, [In] IntPtr[] apidl, [In] uint dwFlags);

        internal static bool SUCCEEDED(int hr)
        {
            return hr >= 0;
        }
    }
}
