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
        void WriteLine(string message);
        void WriteLine();
        void WriteLine(string format, params object[] @params);

        TvCursor Cursor { get; }
    }

    public interface ISecondaryScreenBuffer : IScreenBuffer, IDisposable
    {

        bool Activate();
        void Close();
    }
}
