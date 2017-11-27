using System;
using TvConsole.Win32;

namespace TvConsole
{
    public class TvCursor
    {
        private IntPtr _hstdout;

        internal TvCursor(IntPtr hstdout)
        {
            _hstdout = hstdout;
        }

        public (short X, short Y) GetPosition()
        {
            ConsoleNative.GetConsoleScreenBufferInfo(_hstdout, out CONSOLE_SCREEN_BUFFER_INFO info);
            return (info.dwCursorPosition.X, info.dwCursorPosition.Y);
        }

        public void MoveTo(short X, short Y)
        {
            ConsoleNative.SetConsoleCursorPosition(_hstdout, new COORD(X, Y));
        }

        public void Up()
        {
            (var x, var y) = GetPosition();
            MoveTo(x, (short)(y - 1 < 0 ? 0 : y - 1));
        }

        public void Down()
        {
            (var x, var y) = GetPosition();
            MoveTo(x, (short)(y + 1));
        }

        public void Left()
        {
            (var x, var y) = GetPosition();
            MoveTo((short)(x - 1 < 0 ? 0 : x - 1), y);

        }

        public void Right()
        {
            (var x, var y) = GetPosition();
            MoveTo((short)(x + 1), y);
        }
    }
}