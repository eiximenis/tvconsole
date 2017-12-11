using System;
using System.Text;

namespace TvConsole
{
    public class TvVirtualTerminal
    {
        private readonly TvConsole _console;

        public const char ESC = '\x1b';
        public const string CSI = "\x1b[";

        public TvVirtualTerminal(TvConsole console)
        {
            _console = console;
            Cursor = new TvVirtualTerminalCursor(console);
        }

        public TvVirtualTerminalCursor Cursor {get;}
    }
}