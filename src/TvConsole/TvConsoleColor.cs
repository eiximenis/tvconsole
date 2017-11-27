using System;
using System.Collections.Generic;
using System.Text;
using TvConsole.Win32;

namespace TvConsole
{
    public class TvConsoleColor : IDisposable
    {

        private const ushort FOREGROUND_BLUE = 0x0001;
        private const ushort FOREGROUND_GREEN = 0x0002;
        private const ushort FOREGROUND_RED = 0x0004;
        private const ushort FOREGROUND_INTENSITY = 0x0008;
        private const ushort BACKGROUND_BLUE = 0x0010;
        private const ushort BACKGROUND_GREEN = 0x0020;
        private const ushort BACKGROUND_RED = 0x0040;
        private const ushort BACKGROUND_INTENSITY = 0x0080;


        private readonly IntPtr _hstdout;
        private readonly short _oldAttributes;
        internal TvConsoleColor(IntPtr handle, ConsoleColor color)
        {
            _hstdout = handle;
            ConsoleNative.GetConsoleScreenBufferInfo(_hstdout, out CONSOLE_SCREEN_BUFFER_INFO info);
            _oldAttributes = info.wAttributes;
            ConsoleNative.SetConsoleTextAttribute(_hstdout, ConsoleColorToAttribute(color));
        }

        public void Dispose()
        {
            ConsoleNative.SetConsoleTextAttribute(_hstdout, (ushort)_oldAttributes);
        }

        private ushort ConsoleColorToAttribute(ConsoleColor color)
        {
            switch (color)
            {
                case ConsoleColor.Black:
                    return (ushort)0;
                case ConsoleColor.Blue:
                    return FOREGROUND_BLUE | FOREGROUND_INTENSITY;
                case ConsoleColor.Cyan:
                    return FOREGROUND_BLUE | FOREGROUND_GREEN | FOREGROUND_INTENSITY;
                case ConsoleColor.Green:
                    return FOREGROUND_GREEN | FOREGROUND_INTENSITY;
                case ConsoleColor.Magenta:
                    return FOREGROUND_BLUE | FOREGROUND_GREEN | FOREGROUND_INTENSITY;
                case ConsoleColor.Red:
                    return FOREGROUND_RED | FOREGROUND_INTENSITY;
                case ConsoleColor.White:
                    return FOREGROUND_RED | FOREGROUND_GREEN | FOREGROUND_BLUE | FOREGROUND_INTENSITY;
                case ConsoleColor.Yellow:
                    return FOREGROUND_RED | FOREGROUND_GREEN | FOREGROUND_INTENSITY;
                case ConsoleColor.DarkBlue:
                    return FOREGROUND_BLUE;
                case ConsoleColor.DarkCyan:
                    return FOREGROUND_BLUE | FOREGROUND_GREEN;
                case ConsoleColor.DarkGray:
                    return FOREGROUND_RED | FOREGROUND_GREEN | FOREGROUND_BLUE;
                case ConsoleColor.DarkGreen:
                    return FOREGROUND_GREEN;
                case ConsoleColor.DarkMagenta:
                    return FOREGROUND_BLUE | FOREGROUND_GREEN;
                case ConsoleColor.DarkRed:
                    return FOREGROUND_RED;
                case ConsoleColor.DarkYellow:
                    return FOREGROUND_RED | FOREGROUND_GREEN;
                case ConsoleColor.Gray:
                    return FOREGROUND_RED | FOREGROUND_GREEN | FOREGROUND_BLUE;
                default:
                    return 0;
            }
        }

    }
}
