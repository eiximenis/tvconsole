using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TvConsole.Shared
{
    public interface IConsolePal
    {
        bool IsInputRedirected { get; }
        bool IsOutputRedirected { get; }
        ISecondaryScreenBuffer DefaultBuffer { get; }
        IFontManager FontManager { get; }

        TextReader In { get; }
        bool KeyAvaliable { get; }

        TvConsoleEvents ReadEvents();
        ConsoleKeyInfo ReadKey();
        void EnableMouseSupport();
        void DisableMouseSupport();
    }
}
