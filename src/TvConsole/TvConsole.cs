using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using TvConsole.Extensions;
using TvConsole.Shared;

#if PLATFORM_WINDOWS
using TvConsole.Win32;
#endif

namespace TvConsole
{
    public class TvConsole : IScreenBuffer
    {
        public static TvConsole Instance { get; private set; }

        private ISecondaryScreenBuffer _currentBuffer;
        private readonly IConsolePal _pal;


        private void Destroy()
        {
            Out?.Dispose();
            In?.Dispose();
            Instance = null;
        }

        static TvConsole()
        {
            Instance = new TvConsole(allowRedirect: true);
        }

        public ISecondaryScreenBuffer ActivateNewScreenBuffer()
        {
            var buffer = CreateNewScreenBuffer();
            buffer.Activate();
            return buffer;
        }

        public ISecondaryScreenBuffer ActivateDefaultScreenBuffer() => ActivateScreenBuffer(DefaultBuffer);

        public ISecondaryScreenBuffer ActivateScreenBuffer(ISecondaryScreenBuffer screenBuffer)
        {
            screenBuffer.Activate();
            return screenBuffer;
        }

        public ISecondaryScreenBuffer CreateNewScreenBuffer()
        {
            IntPtr handle = ConsoleNative.CreateConsoleScreenBuffer(
                AccessMode.GENERIC_READ | AccessMode.GENERIC_WRITE,
                ShareMode.FILE_SHARE_WRITE, IntPtr.Zero, ScreenBufferFlags.CONSOLE_TEXTMODE_BUFFER, IntPtr.Zero);

            if (handle.ToInt32() == -1)
            {
                var err = Marshal.GetLastWin32Error();
                throw new InvalidOperationException($"Can't create a new screen buffer (err: {err})");
            }

            var newBuffer = new Win32TvScreenBuffer(handle, outputRedirected: false, disposable: true);
            return newBuffer;
        }

        public void Cls() => _currentBuffer.Cls();

        public bool IsInputRedirected { get; }
        public bool IsOutputRedirected { get; }
        public TvConsoleStreamProperties InProperties { get; }
        public TextReader In { get; }

        public TextWriter Out => _currentBuffer.Out;
        public TvConsoleStreamProperties OutProperties => _currentBuffer.OutProperties;

        public IConsoleCursor Cursor => _currentBuffer.Cursor;

        public ISecondaryScreenBuffer DefaultBuffer => _pal.DefaultBuffer;

        public IFontManager FontManager => _pal.FontManager;

        public void EnableMouseSupport() => _pal.EnableMouseSupport();
        public void DisableMouseSupport() => _pal.DisableMouseSupport();

        public void Close()
        {
            var ok = ConsoleNative.FreeConsole();
            if (ok)
            {
                Destroy();
            }
        }

        public static bool CreateConsole()
        {
            var ok = ConsoleNative.AllocConsole();
            if (ok)
            {
                Instance = new TvConsole(allowRedirect: true);
            }

            return ok;
        }





        internal void SetBufferActive(ISecondaryScreenBuffer buffer)
        {
            _currentBuffer = buffer;
        }


        /*
        public TvVirtualTerminal GetVirtualTerminal()
        {

            if (!IsOutputModeEnabled(ConsoleOutputModes.ENABLE_VIRTUAL_TERMINAL_PROCESSING))
            {
                EnableOutputMode(ConsoleOutputModes.ENABLE_VIRTUAL_TERMINAL_PROCESSING);
            }

            return new TvVirtualTerminal(this);
        }
      */

        private TvConsole(bool allowRedirect)
        {
            InProperties = new TvConsoleStreamProperties((int)ConsoleNative.GetConsoleCP(), System.Console.InputEncoding, IsInputRedirected);
#if PLATFORM_WINDOWS
            _pal = new Win32ConsolePal(InProperties);
#endif
            IsInputRedirected = _pal.IsInputRedirected;
            IsOutputRedirected = _pal.IsOutputRedirected;
            _currentBuffer = _pal.DefaultBuffer;
            In = _pal.In;

            if (!allowRedirect && (IsInputRedirected || IsOutputRedirected))
            {
                Destroy();
                throw new InvalidOperationException("Console input and/or ouput is redirected and this is not allowed by current TvConsole.");
            }
        }

        public void Write(string message) => _currentBuffer.Write(message);
        public void WriteLine(string message) => _currentBuffer.WriteLine(message);
        public void WriteLine(string format, params object[] @params) => _currentBuffer.WriteLine(format, @params);
        public void WriteLine() => _currentBuffer.WriteLine();

        public void WriteCharacterAt(int x, int y, char character, int count = 1) => _currentBuffer.WriteCharacterAt(x, y, character, count);
        public void WriteCharacterAt(int x, int y, char character, ConsoleColor foreColor, ConsoleColor backColor, int count = 1) =>
            _currentBuffer.WriteCharacterAt(x, y, character, foreColor, backColor, count);

        public int Read() => In.Read();

        public string ReadLine() => In.ReadLine();


        public bool KeyAvailable => _pal.KeyAvaliable;

        public TvConsoleEvents ReadEvents() => _pal.ReadEvents();

        public ConsoleKeyInfo ReadKey() => _pal.ReadKey();

        public IConsoleColor ForeColor(ConsoleColor foreground) => _currentBuffer.ForeColor(foreground);
        public IConsoleColor BackColor(ConsoleColor background) => _currentBuffer.BackColor(background);
        public IConsoleColor CharacterColor(ConsoleColor foreground, ConsoleColor background) => _currentBuffer.CharacterColor(foreground, background);



        public IConsoleColor ColorScope => _currentBuffer.ColorScope;

        public ConsoleColor ForegroundColor
        {
            get => _currentBuffer.ForegroundColor;
            set => _currentBuffer.ForegroundColor = value;
        }

        public ConsoleColor BackgroundColor
        {
            get => _currentBuffer.BackgroundColor;
            set => _currentBuffer.BackgroundColor = value;
        }


    }
}
