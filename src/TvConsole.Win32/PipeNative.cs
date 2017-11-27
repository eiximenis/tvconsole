using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace TvConsole.Win32
{

    [StructLayout(LayoutKind.Sequential)]
    public struct SECURITY_ATTRIBUTES
    {
        public int nLength;
        public IntPtr lpSecurityDescriptor;
        public int bInheritHandle;
    }

    public static class PipeNative
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CreatePipe([Out] out IntPtr hReadPipe, [Out] out IntPtr hWritePipe, ref SECURITY_ATTRIBUTES lpPipeAttributes, uint nSize);
    }
}
