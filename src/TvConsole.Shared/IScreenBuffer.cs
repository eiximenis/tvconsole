using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TvConsole
{
    public interface IScreenBuffer
    {
        TvConsoleStreamProperties OutProperties { get; }
        TextWriter Out { get; }

        void Cls();
        void Write(string message);
        void WriteLine(string message);
        void WriteLine();
        void WriteLine(string format, params object[] @params);

        IConsoleCursor Cursor { get; }

        IConsoleColor ForeColor(ConsoleColor foreground);
        IConsoleColor BackColor(ConsoleColor background);
        IConsoleColor CharacterColor(ConsoleColor foreground, ConsoleColor background);
        IConsoleColor ColorScope { get; }

        ConsoleColor ForegroundColor { get; set; }
        ConsoleColor BackgroundColor { get; set; }

    }

    public interface ISecondaryScreenBuffer : IScreenBuffer, IDisposable
    {

        bool Activate();
        void Close();
    }
}
