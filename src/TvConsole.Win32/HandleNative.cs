using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using HANDLE = System.IntPtr;

namespace TvConsole.Win32
{
    public static class HandleNative
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CloseHandle(HANDLE hObject);
    }
}
