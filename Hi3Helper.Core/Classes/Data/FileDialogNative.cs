﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
#if !DISABLE_COM
using System.Threading.Tasks;
#else
using System.Text;
using static Hi3Helper.Logger;
using static Hi3Helper.InvokeProp;
#endif

namespace Hi3Helper
{
    // Reference:
    // https://www.dotnetframework.org/default.aspx/4@0/4@0/DEVDIV_TFS/Dev10/Releases/RTMRel/ndp/fx/src/WinForms/Managed/System/WinForms/FileDialog_Vista_Interop@cs/1305376/FileDialog_Vista_Interop@cs
    public static class FileDialogNative
    {
        internal static IntPtr parentHandler = IntPtr.Zero;

        [ComImport]
        [Guid(IIDGuid.IFileOpenDialog)]
        [CoClass(typeof(FileOpenDialogRCW))]
        internal interface NativeFileOpenDialog : IFileOpenDialog
        { }

        [ComImport]
        [Guid(IIDGuid.IFileSaveDialog)]
        [CoClass(typeof(FileSaveDialogRCW))]
        internal interface NativeFileSaveDialog : IFileSaveDialog
        { }

        [ComImport]
        [ClassInterface(ClassInterfaceType.None)]
        [TypeLibType(TypeLibTypeFlags.FCanCreate)]
        [Guid(CLSIDGuid.FileOpenDialog)]
        internal class FileOpenDialogRCW
        { }

        [ComImport]
        [ClassInterface(ClassInterfaceType.None)]
        [TypeLibType(TypeLibTypeFlags.FCanCreate)]
        [Guid(CLSIDGuid.FileSaveDialog)]
        internal class FileSaveDialogRCW
        { }

        // HACK: Since the class is only available for Windows.Forms and we are avoiding to use that, then use this dummy one instead.
        internal class FileDialogCustomPlace { }

        public static void InitHandlerPointer(IntPtr handle) => parentHandler = handle;

        internal class IIDGuid
        {
            private IIDGuid() { } // Avoid FxCop violation AvoidUninstantiatedInternalClasses
            // IID GUID strings for relevant COM interfaces 
            internal const string IModalWindow = "b4db1657-70d7-485e-8e3e-6fcb5a5c1802";
            internal const string IFileDialog = "42f85136-db7e-439c-85f1-e4075d135fc8";
            internal const string IFileOpenDialog = "d57c7288-d4ad-4768-be02-9d969532d960";
            internal const string IFileSaveDialog = "84bccd23-5fde-4cdb-aea4-af64b83d78ab";
            internal const string IFileDialogEvents = "973510DB-7D7F-452B-8975-74A85828D354";
            internal const string IShellItem = "43826D1E-E718-42EE-BC55-A1E261C37BFE";
            internal const string IShellItemArray = "B63EA76D-1F85-456F-A19C-48159EFA858B";
        }

        internal class CLSIDGuid
        {
            private CLSIDGuid() { } // Avoid FxCop violation AvoidUninstantiatedInternalClasses
            internal const string FileOpenDialog = "DC1C5A9C-E88A-4dde-A5A1-60F82A20AEF7";
            internal const string FileSaveDialog = "C0B4E2F3-BA21-4773-8DBA-335EC946EB8B";
        }

        [ComImport()]
        [Guid(IIDGuid.IModalWindow)]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface IModalWindow
        {

            [PreserveSig]
            int Show([In] IntPtr parent);
        }

        internal enum SIATTRIBFLAGS
        {
            SIATTRIBFLAGS_AND = 0x00000001, // if multiple items and the attributes together.
            SIATTRIBFLAGS_OR = 0x00000002, // if multiple items or the attributes together.
            SIATTRIBFLAGS_APPCOMPAT = 0x00000003, // Call GetAttributes directly on the ShellFolder for multiple attributes 
        }

        [ComImport]
        [Guid(IIDGuid.IShellItemArray)]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface IShellItemArray
        {
            // Not supported: IBindCtx

            void BindToHandler([In, MarshalAs(UnmanagedType.Interface)] IntPtr pbc, [In] ref Guid rbhid, [In] ref Guid riid, out IntPtr ppvOut);

            void GetPropertyStore([In] int Flags, [In] ref Guid riid, out IntPtr ppv);

            void GetPropertyDescriptionList([In] ref PROPERTYKEY keyType, [In] ref Guid riid, out IntPtr ppv);

            void GetAttributes([In] SIATTRIBFLAGS dwAttribFlags, [In] uint sfgaoMask, out uint psfgaoAttribs);

            void GetCount(out uint pdwNumItems);

            void GetItemAt([In] uint dwIndex, [MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

            void EnumItems([MarshalAs(UnmanagedType.Interface)] out IntPtr ppenumShellItems);
        }

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        internal struct PROPERTYKEY
        {
            internal Guid fmtid;
            internal uint pid;
        }

        [ComImport()]
        [Guid(IIDGuid.IFileDialog)]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface IFileDialog
        {
            [PreserveSig]
            int Show([In] IntPtr parent);

            void SetFileTypes([In] uint cFileTypes, [In][MarshalAs(UnmanagedType.LPArray)] COMDLG_FILTERSPEC[] rgFilterSpec);

            void SetFileTypeIndex([In] uint iFileType);

            void GetFileTypeIndex(out uint piFileType);

            void Advise([In, MarshalAs(UnmanagedType.Interface)] IFileDialogEvents pfde, out uint pdwCookie);

            void Unadvise([In] uint dwCookie);

            void SetOptions([In] FOS fos);

            void GetOptions(out FOS pfos);

            void SetDefaultFolder([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi);

            void SetFolder([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi);

            void GetFolder([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

            void GetCurrentSelection([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

            void SetFileName([In, MarshalAs(UnmanagedType.LPWStr)] string pszName);

            void GetFileName([MarshalAs(UnmanagedType.LPWStr)] out string pszName);

            void SetTitle([In, MarshalAs(UnmanagedType.LPWStr)] string pszTitle);

            void SetOkButtonLabel([In, MarshalAs(UnmanagedType.LPWStr)] string pszText);

            void SetFileNameLabel([In, MarshalAs(UnmanagedType.LPWStr)] string pszLabel);

            void GetResult([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

            void AddPlace([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi, int alignment);

            void SetDefaultExtension([In, MarshalAs(UnmanagedType.LPWStr)] string pszDefaultExtension);

            void Close([MarshalAs(UnmanagedType.Error)] int hr);

            void SetClientGuid([In] ref Guid guid);

            void ClearClientData();

            void SetFilter([MarshalAs(UnmanagedType.Interface)] IntPtr pFilter);
        }

        [ComImport()]
        [Guid(IIDGuid.IFileOpenDialog)]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface IFileOpenDialog : IFileDialog
        {
            [PreserveSig]
            new int Show([In] IntPtr parent);

            void SetFileTypes([In] uint cFileTypes, [In] ref COMDLG_FILTERSPEC rgFilterSpec);

            new void SetFileTypeIndex([In] uint iFileType);

            new void GetFileTypeIndex(out uint piFileType);

            new void Advise([In, MarshalAs(UnmanagedType.Interface)] IFileDialogEvents pfde, out uint pdwCookie);

            new void Unadvise([In] uint dwCookie);

            new void SetOptions([In] FOS fos);

            new void GetOptions(out FOS pfos);

            new void SetDefaultFolder([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi);

            new void SetFolder([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi);

            new void GetFolder([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

            new void GetCurrentSelection([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

            new void SetFileName([In, MarshalAs(UnmanagedType.LPWStr)] string pszName);

            new void GetFileName([MarshalAs(UnmanagedType.LPWStr)] out string pszName);

            new void SetTitle([In, MarshalAs(UnmanagedType.LPWStr)] string pszTitle);

            new void SetOkButtonLabel([In, MarshalAs(UnmanagedType.LPWStr)] string pszText);

            new void SetFileNameLabel([In, MarshalAs(UnmanagedType.LPWStr)] string pszLabel);

            new void GetResult([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

            void AddPlace([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi, FileDialogCustomPlace fdcp);

            new void SetDefaultExtension([In, MarshalAs(UnmanagedType.LPWStr)] string pszDefaultExtension);

            new void Close([MarshalAs(UnmanagedType.Error)] int hr);

            new void SetClientGuid([In] ref Guid guid);

            new void ClearClientData();

            new void SetFilter([MarshalAs(UnmanagedType.Interface)] IntPtr pFilter);

            void GetResults([MarshalAs(UnmanagedType.Interface)] out IShellItemArray ppenum);

            void GetSelectedItems([MarshalAs(UnmanagedType.Interface)] out IShellItemArray ppsai);
        }

        [ComImport(),
        Guid(IIDGuid.IFileSaveDialog),
        InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface IFileSaveDialog : IFileDialog
        {
            [PreserveSig]
            new int Show([In] IntPtr parent);

            void SetFileTypes([In] uint cFileTypes, [In] ref COMDLG_FILTERSPEC rgFilterSpec);

            new void SetFileTypeIndex([In] uint iFileType);

            new void GetFileTypeIndex(out uint piFileType);

            new void Advise([In, MarshalAs(UnmanagedType.Interface)] IFileDialogEvents pfde, out uint pdwCookie);

            new void Unadvise([In] uint dwCookie);

            new void SetOptions([In] FOS fos);

            new void GetOptions(out FOS pfos);

            new void SetDefaultFolder([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi);

            new void SetFolder([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi);

            new void GetFolder([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

            new void GetCurrentSelection([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

            new void SetFileName([In, MarshalAs(UnmanagedType.LPWStr)] string pszName);

            new void GetFileName([MarshalAs(UnmanagedType.LPWStr)] out string pszName);

            new void SetTitle([In, MarshalAs(UnmanagedType.LPWStr)] string pszTitle);

            new void SetOkButtonLabel([In, MarshalAs(UnmanagedType.LPWStr)] string pszText);

            new void SetFileNameLabel([In, MarshalAs(UnmanagedType.LPWStr)] string pszLabel);

            new void GetResult([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

            void AddPlace([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi, FileDialogCustomPlace fdcp);

            new void SetDefaultExtension([In, MarshalAs(UnmanagedType.LPWStr)] string pszDefaultExtension);

            new void Close([MarshalAs(UnmanagedType.Error)] int hr);

            new void SetClientGuid([In] ref Guid guid);

            new void ClearClientData();

            new void SetFilter([MarshalAs(UnmanagedType.Interface)] IntPtr pFilter);

            void SetSaveAsItem([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi);

            void SetProperties([In, MarshalAs(UnmanagedType.Interface)] IntPtr pStore);

            void SetCollectedProperties([In, MarshalAs(UnmanagedType.Interface)] IntPtr pList, [In] int fAppendDefault);

            void GetProperties([MarshalAs(UnmanagedType.Interface)] out IntPtr ppStore);

            void ApplyProperties([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi, [In, MarshalAs(UnmanagedType.Interface)] IntPtr pStore, [In, ComAliasName("ShellObjects.wireHWND")] ref IntPtr hwnd, [In, MarshalAs(UnmanagedType.Interface)] IntPtr pSink);
        }

        [ComImport,
        Guid(IIDGuid.IFileDialogEvents),
        InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface IFileDialogEvents
        {
            // NOTE: some of these callbacks are cancelable - returning S_FALSE means that 
            // the dialog should not proceed (e.g. with closing, changing folder); to
            // support this, we need to use the PreserveSig attribute to enable us to return 
            // the proper HRESULT 
            [PreserveSig]
            int OnFileOk([In, MarshalAs(UnmanagedType.Interface)] IFileDialog pfd);

            [PreserveSig]
            int OnFolderChanging([In, MarshalAs(UnmanagedType.Interface)] IFileDialog pfd, [In, MarshalAs(UnmanagedType.Interface)] IShellItem psiFolder);

            void OnFolderChange([In, MarshalAs(UnmanagedType.Interface)] IFileDialog pfd);

            void OnSelectionChange([In, MarshalAs(UnmanagedType.Interface)] IFileDialog pfd);

            void OnShareViolation([In, MarshalAs(UnmanagedType.Interface)] IFileDialog pfd, [In, MarshalAs(UnmanagedType.Interface)] IShellItem psi, out FDE_SHAREVIOLATION_RESPONSE pResponse);

            void OnTypeChange([In, MarshalAs(UnmanagedType.Interface)] IFileDialog pfd);

            void OnOverwrite([In, MarshalAs(UnmanagedType.Interface)] IFileDialog pfd, [In, MarshalAs(UnmanagedType.Interface)] IShellItem psi, out FDE_OVERWRITE_RESPONSE pResponse);
        }

        [ComImport,
        Guid(IIDGuid.IShellItem),
        InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface IShellItem
        {
            void BindToHandler([In, MarshalAs(UnmanagedType.Interface)] IntPtr pbc, [In] ref Guid bhid, [In] ref Guid riid, out IntPtr ppv);

            void GetParent([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

            void GetDisplayName([In] SIGDN sigdnName, [MarshalAs(UnmanagedType.LPWStr)] out string ppszName);

            void GetAttributes([In] uint sfgaoMask, out uint psfgaoAttribs);

            void Compare([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi, [In] uint hint, out int piOrder);
        }

        internal enum SIGDN : uint
        {
            SIGDN_NORMALDISPLAY = 0x00000000,           // SHGDN_NORMAL 
            SIGDN_PARENTRELATIVEPARSING = 0x80018001,   // SHGDN_INFOLDER | SHGDN_FORPARSING
            SIGDN_DESKTOPABSOLUTEPARSING = 0x80028000,  // SHGDN_FORPARSING 
            SIGDN_PARENTRELATIVEEDITING = 0x80031001,   // SHGDN_INFOLDER | SHGDN_FOREDITING
            SIGDN_DESKTOPABSOLUTEEDITING = 0x8004c000,  // SHGDN_FORPARSING | SHGDN_FORADDRESSBAR
            SIGDN_FILESYSPATH = 0x80058000,             // SHGDN_FORPARSING
            SIGDN_URL = 0x80068000,                     // SHGDN_FORPARSING 
            SIGDN_PARENTRELATIVEFORADDRESSBAR = 0x8007c001,     // SHGDN_INFOLDER | SHGDN_FORPARSING | SHGDN_FORADDRESSBAR
            SIGDN_PARENTRELATIVE = 0x80080001           // SHGDN_INFOLDER 
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4)]
        internal struct COMDLG_FILTERSPEC
        {
            [MarshalAs(UnmanagedType.LPWStr)]
            internal string pszName;
            [MarshalAs(UnmanagedType.LPWStr)]
            internal string pszSpec;
        }

        [Flags]
        internal enum FOS : uint
        {
            FOS_OVERWRITEPROMPT = 0x00000002,
            FOS_STRICTFILETYPES = 0x00000004,
            FOS_NOCHANGEDIR = 0x00000008,
            FOS_PICKFOLDERS = 0x00000020,
            FOS_FORCEFILESYSTEM = 0x00000040, // Ensure that items returned are filesystem items. 
            FOS_ALLNONSTORAGEITEMS = 0x00000080, // Allow choosing items that have no storage.
            FOS_NOVALIDATE = 0x00000100,
            FOS_ALLOWMULTISELECT = 0x00000200,
            FOS_PATHMUSTEXIST = 0x00000800,
            FOS_FILEMUSTEXIST = 0x00001000,
            FOS_CREATEPROMPT = 0x00002000,
            FOS_SHAREAWARE = 0x00004000,
            FOS_NOREADONLYRETURN = 0x00008000,
            FOS_NOTESTFILECREATE = 0x00010000,
            FOS_HIDEMRUPLACES = 0x00020000,
            FOS_HIDEPINNEDPLACES = 0x00040000,
            FOS_NODEREFERENCELINKS = 0x00100000,
            FOS_DONTADDTORECENT = 0x02000000,
            FOS_FORCESHOWHIDDEN = 0x10000000,
            FOS_DEFAULTNOMINIMODE = 0x20000000
        }

        internal enum FDE_SHAREVIOLATION_RESPONSE
        {
            FDESVR_DEFAULT = 0x00000000,
            FDESVR_ACCEPT = 0x00000001,
            FDESVR_REFUSE = 0x00000002
        }

        internal enum FDE_OVERWRITE_RESPONSE
        {
            FDEOR_DEFAULT = 0x00000000,
            FDEOR_ACCEPT = 0x00000001,
            FDEOR_REFUSE = 0x00000002
        }

#if DISABLE_COM
        // Flags for FolderPicker
        public const int WM_USER = 0x400;
        public const int BFFM_INITIALIZED = 1;
        public const int BFFM_SELCHANGED = 2;
        public const int BFFM_SETSELECTIONW = WM_USER + 103;
        public const int BFFM_SETSTATUSTEXTW = WM_USER + 104;

        private const uint BIF_DONTGOBELOWDOMAIN = 0x0002;
        private const uint BIF_USENEWUI = 0x0040 + 0x0010;

        private static string _initialPath = "";

        private static int OnBrowseEvent(IntPtr hWnd, int msg, IntPtr lp, IntPtr lpData)
        {
            switch (msg)
            {
                case BFFM_INITIALIZED:
                    {
                        SendMessage(new HandleRef(null, hWnd), BFFM_SETSELECTIONW, 1, _initialPath);
                        break;
                    }
                case BFFM_SELCHANGED:
                    {
                        IntPtr pathPtr = Marshal.AllocHGlobal((int)(260 * Marshal.SystemDefaultCharSize));
                        if (SHGetPathFromIDList(lp, pathPtr))
                            SendMessage(new HandleRef(null, hWnd), BFFM_SETSTATUSTEXTW, 0, pathPtr);
                        Marshal.FreeHGlobal(pathPtr);
                        break;
                    }
            }

            return 0;
        }

        public static string GetFolderPicker()
        {
            StringBuilder sb = new StringBuilder(256);
            IntPtr bufferAddress = Marshal.AllocHGlobal(256);
            IntPtr pidl = IntPtr.Zero;
            BROWSEINFO bi = new BROWSEINFO();
            bi.hwndOwner = parentHandler;
            bi.pidlRoot = IntPtr.Zero;
            bi.ulFlags = BIF_USENEWUI | BIF_DONTGOBELOWDOMAIN;
            bi.lpfn = new BrowseCallBackProc(OnBrowseEvent);
            bi.lParam = IntPtr.Zero;
            bi.iImage = 0;

            try
            {
                pidl = SHBrowseForFolder(ref bi);
                if (!SHGetPathFromIDList(pidl, bufferAddress))
                {
                    return null;
                }
                sb.Append(Marshal.PtrToStringAuto(bufferAddress));
            }
            finally
            {
                Marshal.FreeCoTaskMem(pidl);
            }

            return sb.ToString();
        }

        public static string GetFilePicker(Dictionary<string, string> FileTypeFilter = null)
        {
            var ofn = new OpenFileName();
            ofn.structSize = Marshal.SizeOf(ofn);
            ofn.filter = SetFileTypeFiler(FileTypeFilter);

            ofn.file = new string(new char[256]);
            ofn.maxFile = ofn.file.Length;

            ofn.fileTitle = new string(new char[64]);
            ofn.maxFileTitle = ofn.fileTitle.Length;

            ofn.dlgOwner = parentHandler;

            try
            {
                if (GetOpenFileName(ofn))
                    return ofn.file;
            }
            catch (Exception ex)
            {
                LogWriteLine($"{ex}", LogType.Error, true);
            }

            return null;
        }


        private static string SetFileTypeFiler(Dictionary<string, string> FileTypeFilter)
        {
            string ret = "";
            foreach (KeyValuePair<string, string> kvp in FileTypeFilter)
            {
                ret += $"{kvp.Key} ({kvp.Value})\0{kvp.Value}\0";
            }

            return ret + "\0";
        }

#else
        public static async Task<List<string>> GetMultiFilePicker(Dictionary<string, string> FileTypeFilter = null) => await Task.Run(() =>
        {
            IFileOpenDialog dialog = null;
            IShellItemArray resShell;

            try
            {
                dialog = new NativeFileOpenDialog();
                dialog.SetOptions(FOS.FOS_NOREADONLYRETURN | FOS.FOS_DONTADDTORECENT | FOS.FOS_ALLOWMULTISELECT);
                SetFileTypeFiler(ref dialog, FileTypeFilter);

                if (dialog.Show(parentHandler) < 0) return null;

                dialog.GetResults(out resShell);
                return GetIShellItemArray(dialog, resShell);
            }
            catch (COMException)
            {
                return null;
            }
            finally
            {
                if (dialog != null) Marshal.FinalReleaseComObject(dialog);
            }
        }).ConfigureAwait(false);

        public static async Task<string> GetFilePicker(Dictionary<string, string> FileTypeFilter = null, string title = null) => await Task.Run(() =>
        {
            IFileOpenDialog dialog = null;
            IShellItem resShell;
            string result;

            try
            {
                dialog = new NativeFileOpenDialog();
                if (title != null)
                {
                    dialog.SetTitle(title);
                }
                SetFileTypeFiler(ref dialog, FileTypeFilter);
                dialog.SetOptions(FOS.FOS_NOREADONLYRETURN | FOS.FOS_DONTADDTORECENT);

                if (dialog.Show(parentHandler) < 0) return null;

                dialog.GetResult(out resShell);
                resShell.GetDisplayName(SIGDN.SIGDN_FILESYSPATH, out result);
                return result;
            }
            catch (COMException)
            {
                return null;
            }
            finally
            {
                if (dialog != null) Marshal.FinalReleaseComObject(dialog);
            }
        }).ConfigureAwait(false);

        public static async Task<string> GetFileSavePicker(Dictionary<string, string> FileTypeFilter = null, string title = null) => await Task.Run(() =>
        {
            IFileSaveDialog dialog = null;
            IShellItem resShell;
            string result;

            try
            {
                dialog = new NativeFileSaveDialog();
                if (title != null)
                {
                    dialog.SetTitle(title);
                }
                SetFileTypeSaveFiler(ref dialog, FileTypeFilter);
                dialog.SetOptions(FOS.FOS_NOREADONLYRETURN | FOS.FOS_DONTADDTORECENT);

                if (dialog.Show(parentHandler) < 0) return null;

                dialog.GetResult(out resShell);
                resShell.GetDisplayName(SIGDN.SIGDN_FILESYSPATH, out result);
                return result;
            }
            catch (COMException)
            {
                return null;
            }
            finally
            {
                if (dialog != null) Marshal.FinalReleaseComObject(dialog);
            }
        }).ConfigureAwait(false);

        public static async Task<List<string>> GetMultiFolderPicker() => await Task.Run(() =>
        {
            IFileOpenDialog dialog = null;
            IShellItemArray resShell;

            try
            {
                dialog = new NativeFileOpenDialog();
                dialog.SetOptions(FOS.FOS_NOREADONLYRETURN | FOS.FOS_ALLOWMULTISELECT | FOS.FOS_DONTADDTORECENT | FOS.FOS_PICKFOLDERS);

                if (dialog.Show(parentHandler) < 0) return null;

                dialog.GetResults(out resShell);
                return GetIShellItemArray(dialog, resShell);
            }
            catch (COMException)
            {
                return null;
            }
            finally
            {
                if (dialog != null) Marshal.FinalReleaseComObject(dialog);
            }
        }).ConfigureAwait(false);

        public static async Task<string> GetFolderPicker() => await Task.Run(() =>
        {
            IFileDialog dialog = null;
            IShellItem resShell;
            string result;

            try
            {
                dialog = new NativeFileOpenDialog();
                dialog.SetOptions(FOS.FOS_NOREADONLYRETURN | FOS.FOS_DONTADDTORECENT | FOS.FOS_PICKFOLDERS);

                if (dialog.Show(parentHandler) < 0) return null;

                dialog.GetFolder(out resShell);
                resShell.GetDisplayName(SIGDN.SIGDN_DESKTOPABSOLUTEPARSING, out result);
                return result;
            }
            catch (COMException)
            {
                return null;
            }
            finally
            {
                if (dialog != null) Marshal.FinalReleaseComObject(dialog);
            }
        }).ConfigureAwait(false);

        private static void SetFileTypeSaveFiler(ref IFileSaveDialog dialog, Dictionary<string, string> FileTypeFilter)
        {
            List<COMDLG_FILTERSPEC> fileTypes = new List<COMDLG_FILTERSPEC>();

            if (FileTypeFilter != null)
            {
                foreach (KeyValuePair<string, string> entry in FileTypeFilter)
                    fileTypes.Add(new COMDLG_FILTERSPEC { pszName = entry.Key, pszSpec = entry.Value });

                dialog.SetFileTypes((uint)fileTypes.Count, fileTypes.ToArray());
            }
        }

        private static void SetFileTypeFiler(ref IFileOpenDialog dialog, Dictionary<string, string> FileTypeFilter)
        {
            List<COMDLG_FILTERSPEC> fileTypes = new List<COMDLG_FILTERSPEC>();

            if (FileTypeFilter != null)
            {
                foreach (KeyValuePair<string, string> entry in FileTypeFilter)
                    fileTypes.Add(new COMDLG_FILTERSPEC { pszName = entry.Key, pszSpec = entry.Value });

                dialog.SetFileTypes((uint)fileTypes.Count, fileTypes.ToArray());
            }
        }

        private static List<string> GetIShellItemArray(in IFileOpenDialog dialog, in IShellItemArray itemArray)
        {
            List<string> results = new List<string>();
            IShellItem item;
            uint fileCount;
            string _res;

            itemArray.GetCount(out fileCount);
            for (uint i = 0; i < fileCount; i++)
            {
                itemArray.GetItemAt(i, out item);
                item.GetDisplayName(SIGDN.SIGDN_FILESYSPATH, out _res);
                results.Add(_res);
            }

            return results;
        }
#endif
    }
}
