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

namespace Aufbauwerk.Tools.PdfKit
{
    internal static class Native
    {
        private static class Dll
        {
            public const string GsDll32 = "gsdll32.dll";
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

        [ComImport, Guid("85075acf-231f-40ea-9610-d26b7b58f638"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IInitializeCommand
        {
            void Initialize([In, MarshalAs(UnmanagedType.LPWStr)] string pszCommandName, [In] IntPtr ppb);
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
            void SetSelection([In] IntPtr psia);
            void GetSelection([In, MarshalAs(UnmanagedType.LPStruct)] Guid riid, [Out] out IntPtr ppv);
        }

        [ComImport, Guid("00000114-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IOleWindow
        {
            IntPtr GetWindow();
            void Dummy_ContextSensitiveHelp();
        }

        [ComImport, Guid("EBBC7C04-315E-11d2-B62F-006097DF5BD4"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IProgressDialog
        {
            void StartProgressDialog([In] IntPtr hwndParent, [In] IntPtr punkEnableModless, [In] uint dwFlags, [In] IntPtr pvReserved);
            void StopProgressDialog();
            void SetTitle([In, MarshalAs(UnmanagedType.LPWStr)] string pwzTitle);
            void Dummy_SetAnimation();
            [PreserveSig]
            [return: MarshalAs(UnmanagedType.Bool)]
            bool HasUserCancelled();
            void SetProgress([In] int dwCompleted, [In] int dwTotal);
            void Dummy_SetProgress64();
            void SetLine([In] int dwLineNum, [In, MarshalAs(UnmanagedType.LPWStr)] string pwzString, [In, MarshalAs(UnmanagedType.Bool)] bool fCompactPath, [In] IntPtr pvResevered);
            void SetCancelMsg([In, MarshalAs(UnmanagedType.LPWStr)] string pwzCancelMsg, [In] IntPtr pvReserved);
            void Timer([In] uint dwTimerAction, [In] IntPtr pvResevered);
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
        internal static readonly Guid CLSID_ProgressDialog = new Guid("F8383852-FCD3-11d1-A6B9-006097DF5BD4");
        internal const uint DISPLAY_COLORS_RGB = 1 << 2;
        internal const uint DISPLAY_DEPTH_8 = 1 << 11;
        internal const uint DISPLAY_LITTLEENDIAN = 1 << 16;
        internal const uint DISPLAY_UNUSED_LAST = 1 << 7;
        internal const int DISPLAY_VERSION_MAJOR_V1 = 1;
        internal const int DISPLAY_VERSION_MINOR_V1 = 0;
        internal const int GS_ARG_ENCODING_UTF8 = 1;
        internal const int gs_error_interrupt = -6;
        internal const int gs_error_Fatal = -100;
        internal const int gs_error_NeedInput = -106;
        internal const int gs_error_Quit = -101;
        internal const int gs_error_undefinedresult = -23;
        internal const uint PDTIMER_PAUSE = 0x00000002;
        internal const uint PDTIMER_RESUME = 0x00000003;
        internal const uint PROGDLG_AUTOTIME = 0x00000002;
        internal const uint REGCLS_SINGLEUSE = 0;
        internal const int S_OK = 0;
        internal const uint SIGDN_FILESYSPATH = 0x80058000;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        internal delegate int display_close(IntPtr handle, IntPtr device);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        internal delegate IntPtr display_memalloc(IntPtr handle, IntPtr device, int size);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        internal delegate int display_memfree(IntPtr handle, IntPtr device, IntPtr mem);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        internal delegate int display_open(IntPtr handle, IntPtr device);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        internal delegate int display_page(IntPtr handle, IntPtr device, int copies, int flush);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        internal delegate int display_preclose(IntPtr handle, IntPtr device);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        internal delegate int display_presize(IntPtr handle, IntPtr device, int width, int height, int raster, uint format);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        internal delegate int display_size(IntPtr handle, IntPtr device, int width, int height, int raster, uint format, IntPtr pimage);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        internal delegate int display_sync(IntPtr handle, IntPtr device);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        internal delegate int display_update(IntPtr handle, IntPtr device, int x, int y, int w, int h);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        internal delegate int poll_fn(IntPtr caller_handle);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        internal delegate int stderr_fn(IntPtr caller_handle, IntPtr buf, int len);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        internal delegate int stdin_fn(IntPtr caller_handle, IntPtr buf, int len);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        internal delegate int stdout_fn(IntPtr caller_handle, IntPtr buf, int len);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct display_callback_v1
        {
            public int size;
            public int version_major;
            public int version_minor;
            public display_open display_open;
            public display_preclose display_preclose;
            public display_close display_close;
            public display_presize display_presize;
            public display_size display_size;
            public display_sync display_sync;
            public display_page display_page;
            public display_update display_update;
            public display_memalloc display_memalloc;
            public display_memfree display_memfree;

            public display_callback_v1(bool useHGlobal)
            {
                size = Marshal.SizeOf(typeof(display_callback_v1));
                version_major = Native.DISPLAY_VERSION_MAJOR_V1;
                version_minor = Native.DISPLAY_VERSION_MINOR_V1;
                display_open = (handle, device) => 0;
                display_preclose = (handle, device) => 0;
                display_close = (handle, device) => 0;
                display_presize = (handle, device, width, height, raster, format) => 0;
                display_size = (handle, device, width, height, raster, format, pimage) => 0;
                display_sync = (handle, device) => 0;
                display_page = (handle, device, copies, flush) => 0;
                display_update = null;
                display_memalloc = useHGlobal ? new display_memalloc((handle, device, size2) => Marshal.AllocHGlobal(size2)) : null;
                display_memfree = useHGlobal ? new display_memfree((handle, device, mem) => { Marshal.FreeHGlobal(mem); return 0; }) : null;
                display_memfree = null;
            }
        };

        [DllImport(Dll.Ole32, ExactSpelling = true, CharSet = CharSet.Unicode, PreserveSig = false)]
        internal static extern int CoLockObjectExternal([MarshalAs(UnmanagedType.IUnknown)] object pUnk, bool fLock, bool fLastUnlockReleases);

        [DllImport(Dll.Ole32, ExactSpelling = true, CharSet = CharSet.Unicode, PreserveSig = false)]
        internal static extern void CoRegisterClassObject([MarshalAs(UnmanagedType.LPStruct)] Guid rclsid, [MarshalAs(UnmanagedType.Interface)] IClassFactory pUnk, uint dwClsContext, uint flags, out uint lpdwRegister);

        [DllImport(Dll.Ole32, ExactSpelling = true, CharSet = CharSet.Unicode, PreserveSig = false)]
        internal static extern void CoRevokeClassObject(uint dwRegister);

        [DllImport(Dll.GsDll32, ExactSpelling = true, CharSet = CharSet.Unicode)]
        internal static extern void gsapi_delete_instance(IntPtr instance);

        [DllImport(Dll.GsDll32, ExactSpelling = true, CharSet = CharSet.Unicode)]
        internal static extern int gsapi_exit(IntPtr instance);

        [DllImport(Dll.GsDll32, ExactSpelling = true, CharSet = CharSet.Unicode)]
        internal static extern int gsapi_init_with_args(IntPtr instance, int argc, IntPtr[] argv);

        [DllImport(Dll.GsDll32, ExactSpelling = true, CharSet = CharSet.Unicode)]
        internal static extern int gsapi_new_instance(out IntPtr pinstance, IntPtr caller_handle);

        [DllImport(Dll.GsDll32, ExactSpelling = true, CharSet = CharSet.Unicode)]
        internal static extern int gsapi_run_string_begin(IntPtr instance, int user_errors, out int pexit_code);

        [DllImport(Dll.GsDll32, ExactSpelling = true, CharSet = CharSet.Unicode)]
        internal static extern int gsapi_run_string_continue(IntPtr instance, IntPtr str, int length, int user_errors, out int pexit_code);

        [DllImport(Dll.GsDll32, ExactSpelling = true, CharSet = CharSet.Unicode)]
        internal static extern int gsapi_run_string_end(IntPtr instance, int user_errors, out int pexit_code);

        [DllImport(Dll.GsDll32, ExactSpelling = true, CharSet = CharSet.Unicode)]
        internal static extern int gsapi_run_string_with_length(IntPtr instance, IntPtr str, int length, int user_errors, out int pexit_code);

        [DllImport(Dll.GsDll32, ExactSpelling = true, CharSet = CharSet.Unicode)]
        internal static extern int gsapi_set_arg_encoding(IntPtr instance, int encoding);

        [DllImport(Dll.GsDll32, ExactSpelling = true, CharSet = CharSet.Unicode)]
        internal static extern int gsapi_set_display_callback(IntPtr instance, IntPtr callback);

        [DllImport(Dll.GsDll32, ExactSpelling = true, CharSet = CharSet.Unicode)]
        internal static extern int gsapi_set_poll(IntPtr instance, poll_fn poll_fn);

        [DllImport(Dll.GsDll32, ExactSpelling = true, CharSet = CharSet.Unicode)]
        internal static extern int gsapi_set_stdio(IntPtr instance, stdin_fn stdin_fn, stdout_fn stdout_fn, stderr_fn stderr_fn);

        [DllImport(Dll.User32, ExactSpelling = true, CharSet = CharSet.Unicode)]
        internal static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport(Dll.User32, ExactSpelling = true, CharSet = CharSet.Unicode)]
        internal static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport(Dll.Shell32, ExactSpelling = true, CharSet = CharSet.Unicode, PreserveSig = false)]
        [return: MarshalAs(UnmanagedType.Interface)]
        internal static extern IShellFolder SHGetDesktopFolder();

        [DllImport(Dll.Shell32, ExactSpelling = true, CharSet = CharSet.Unicode, PreserveSig = false)]
        internal static extern void SHOpenFolderAndSelectItems(IntPtr pidlFolder, int cidl, IntPtr[] apidl, uint dwFlags);
    }
}
