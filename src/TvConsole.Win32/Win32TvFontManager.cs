using System;
using System.Runtime.InteropServices;
using TvConsole.Win32;

namespace TvConsole
{
    public class Win32TvFontManager : IFontManager
    {
        private IntPtr _handle;

        public int FontSizeX {get; private set;}
        public int FontSizeY { get; private set; }
        public string FontName { get; private set; }

        private CONSOLE_FONT_INFOEX _fontInfo;


        public Win32TvFontManager(IntPtr handle)
        {
            _handle = handle;
            _fontInfo = new CONSOLE_FONT_INFOEX();
            var ok = ConsoleNative.GetCurrentConsoleFontEx(_handle, false, _fontInfo);
            if (!ok)
            {
                var err = Marshal.GetLastWin32Error();
            }

            FontSizeX = _fontInfo.dwFontSize.X;
            FontSizeX = _fontInfo.dwFontSize.Y;
            FontName = _fontInfo.FaceName.Clone()?.ToString();
        }

        public void UpdateSize(short newX, short newY)
        {
            _fontInfo.dwFontSize.X = newX;
            _fontInfo.dwFontSize.Y = newY;
            var ok = ConsoleNative.SetCurrentConsoleFontEx(_handle, false, _fontInfo);
            if (ok)
            {
                FontSizeX = _fontInfo.dwFontSize.X;
                FontSizeY = _fontInfo.dwFontSize.X;
            }
            else
            {
                var err = Marshal.GetLastWin32Error();
                _fontInfo.dwFontSize.X = (short)FontSizeX;
                _fontInfo.dwFontSize.Y = (short)FontSizeY;
            }
        }
    }
}