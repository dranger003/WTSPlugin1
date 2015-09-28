using System;
using System.Runtime.InteropServices;

namespace ConsoleApplication1
{
    static class PInvoke
    {
        public const UInt32 ERROR_SUCCESS = 0;
        public const UInt32 ERROR_INVALID_HANDLE = 6;
        public const UInt32 ERROR_PIPE_NOT_CONNECTED = 233;

        public const UInt32 WTS_CURRENT_SESSION = unchecked((UInt32)(-1));
        public const UInt32 WTS_CHANNEL_OPTION_DYNAMIC = 0x00000001;

        public const UInt32 DUPLICATE_SAME_ACCESS = 0x00000002;

        public enum WTS_VIRTUAL_CLASS
        {
            WTSVirtualClientData,
            WTSVirtualFileHandle
        }

        //HANDLE
        //WINAPI
        //WTSVirtualChannelOpenEx(
        //                     IN DWORD SessionId,
        //                     _In_ LPSTR pVirtualName,   /* ascii name */
        //                     IN DWORD flags
        //                     );
        [DllImport("wtsapi32.dll", SetLastError = true)]
        public static extern IntPtr WTSVirtualChannelOpenEx(
            UInt32 SessionId,
            [MarshalAs(UnmanagedType.LPStr)]
            string pVirtualName,
            UInt32 flags);

        //BOOL
        //WINAPI
        //WTSVirtualChannelClose(
        //    IN HANDLE hChannelHandle
        //    );
        [DllImport("wtsapi32.dll", SetLastError = true)]
        public static extern Boolean WTSVirtualChannelClose(
            IntPtr hChannelHandle);

        //BOOL
        //WINAPI
        //WTSVirtualChannelQuery(
        //    IN HANDLE hChannelHandle,
        //    IN WTS_VIRTUAL_CLASS,
        //    OUT PVOID *ppBuffer,
        //    OUT DWORD *pBytesReturned
        //    );
        [DllImport("wtsapi32.dll", SetLastError = true)]
        public static extern Boolean WTSVirtualChannelQuery(
            IntPtr hChannelHandle,
            WTS_VIRTUAL_CLASS cls,
            out IntPtr ppBuffer,
            out UInt32 pBytesReturned);

        //VOID
        //WINAPI
        //WTSFreeMemory(
        //    IN PVOID pMemory
        //    );
        [DllImport("wtsapi32.dll", SetLastError = true)]
        public static extern void WTSFreeMemory(
            IntPtr pMemory);

        //WINBASEAPI
        //BOOL
        //WINAPI
        //DuplicateHandle(
        //    _In_ HANDLE hSourceProcessHandle,
        //    _In_ HANDLE hSourceHandle,
        //    _In_ HANDLE hTargetProcessHandle,
        //    _Outptr_ LPHANDLE lpTargetHandle,
        //    _In_ DWORD dwDesiredAccess,
        //    _In_ BOOL bInheritHandle,
        //    _In_ DWORD dwOptions
        //    );
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern Boolean DuplicateHandle(
            IntPtr hSourceProcessHandle,
            IntPtr hSourceHandle,
            IntPtr hTargetProcessHandle,
            out IntPtr lpTargetHandle,
            UInt32 dwDesiredAccess,
            [MarshalAs(UnmanagedType.Bool)]
            Boolean bInheritHandle,
            UInt32 dwOptions);

        //WINBASEAPI
        //HANDLE
        //WINAPI
        //GetCurrentProcess(
        //    VOID
        //    );
        [DllImport("kernel32")]
        public static extern IntPtr GetCurrentProcess();

        //WINBASEAPI
        //BOOL
        //WINAPI
        //WriteFile(
        //    _In_ HANDLE hFile,
        //    _In_reads_bytes_opt_(nNumberOfBytesToWrite) LPCVOID lpBuffer,
        //    _In_ DWORD nNumberOfBytesToWrite,
        //    _Out_opt_ LPDWORD lpNumberOfBytesWritten,
        //    _Inout_opt_ LPOVERLAPPED lpOverlapped
        //    );
        [DllImport("kernel32", SetLastError = true)]
        public static extern Boolean WriteFile(
            IntPtr hFile,
            byte[] lpBuffer,
            UInt32 nNumberOfBytesToWrite,
            out UInt32 lpNumberOfBytesWritten,
            IntPtr lpOverlapped);
    }
}
