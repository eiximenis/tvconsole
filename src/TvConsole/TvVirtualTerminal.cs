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
        }

        public void DrawBorder()
        {
            //var str = $"{ESC}(0{CSI}104;93mx{CSI}0m{ESC}(B";
            _console.Out.Write($"{ESC}[101;93m STYLES [0m");
        }

    }
}