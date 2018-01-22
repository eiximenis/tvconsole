using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using TvConsole.Extensions;

#if PLATFORM_WINDOWS
using TvConsole.Win32;
using ConsoleShim = TvConsole.Win32.Impl.Win32Console;
#endif

namespace TvConsole
{
    public class TvConsole : IScreenBuffer
    {
        private const int STDIN = -10;
        private const int STDOUT = -11;

        public static TvConsole Instance { get; private set; }

        private readonly IntPtr _hstdin;
        private readonly IntPtr _hstdout;
        private ISecondaryScreenBuffer _currentBuffer;
        private readonly ConsoleShim _shim;


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

            var newBuffer = new TvScreenBuffer(handle, this, outputRedirected: false, disposable: true);
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

        public ISecondaryScreenBuffer DefaultBuffer { get; }

        public TvFontManager FontManager { get; }

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

        public bool IsInputModeEnabled(ConsoleInputModes modeToCheck)
        {
            ConsoleNative.GetConsoleMode(_hstdin, out uint currentMode);
            return (currentMode & (uint)modeToCheck) == (uint)modeToCheck;
        }


        internal void SetBufferActive(ISecondaryScreenBuffer buffer)
        {
            _currentBuffer = buffer;
        }

        public void EnableInputMode(ConsoleInputModes modeToEnable)
        {
            ConsoleNative.GetConsoleMode(_hstdin, out uint currentMode);
            var newMode = (uint)currentMode | (uint)modeToEnable;
            ConsoleNative.SetConsoleMode(_hstdin, newMode);
        }

        public void DisableInputMode(ConsoleInputModes modeToDisable)
        {
            ConsoleNative.GetConsoleMode(_hstdin, out uint currentMode);
            var newMode = (uint)currentMode & (uint)~modeToDisable;
            ConsoleNative.SetConsoleMode(_hstdin, newMode);
        }

        public bool IsOutputModeEnabled(ConsoleOutputModes modeToCheck)
        {
            ConsoleNative.GetConsoleMode(_hstdout, out uint currentMode);
            return (currentMode & (uint)modeToCheck) == (uint)modeToCheck;
        }

        public void EnableOutputMode(ConsoleOutputModes modeToEnable)
        {
            ConsoleNative.GetConsoleMode(_hstdout, out uint currentMode);
            var newMode = (uint)currentMode | (uint)modeToEnable;
            ConsoleNative.SetConsoleMode(_hstdout, newMode);
        }

        public void DisableOutputMode(ConsoleOutputModes modeToDisable)
        {
            ConsoleNative.GetConsoleMode(_hstdout, out uint currentMode);
            var newMode = (uint)currentMode & (uint)~modeToDisable;
            ConsoleNative.SetConsoleMode(_hstdout, newMode);
        }

        public TvVirtualTerminal GetVirtualTerminal()
        {

            if (!IsOutputModeEnabled(ConsoleOutputModes.ENABLE_VIRTUAL_TERMINAL_PROCESSING))
            {
                EnableOutputMode(ConsoleOutputModes.ENABLE_VIRTUAL_TERMINAL_PROCESSING);
            }

            return new TvVirtualTerminal(this);
        }

        private TvConsole(bool allowRedirect)
        {
            _shim = new ConsoleShim();
            _hstdin = ConsoleNative.GetStdHandle(STDIN);
            _hstdout = ConsoleNative.GetStdHandle(STDOUT);
            IsInputRedirected = FileNative.GetFileType(_hstdin) != FILE_TYPE.FILE_TYPE_CHAR;
            IsOutputRedirected = FileNative.GetFileType(_hstdout) != FILE_TYPE.FILE_TYPE_CHAR;
            DefaultBuffer = new TvScreenBuffer(_hstdout, this, IsOutputRedirected, disposable: false);
            _currentBuffer = DefaultBuffer;
            FontManager = new TvFontManager(_hstdout);
            InProperties = new TvConsoleStreamProperties((int)ConsoleNative.GetConsoleCP(), System.Console.InputEncoding, IsInputRedirected);
            In = new StreamReader(new TvConsoleStream(_hstdin, FileAccess.Read, InProperties.UseFileApis), InProperties.Encoding,
                detectEncodingFromByteOrderMarks: false,
                bufferSize: 256,
                leaveOpen: true);

            var bufferInfo = CONSOLE_SCREEN_BUFFER_INFO_EX.New();
            var ok = ConsoleNative.GetConsoleScreenBufferInfoEx(_hstdout, ref bufferInfo);
            var err = Marshal.GetLastWin32Error();

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



        public int Read() => In.Read();

        public string ReadLine() => In.ReadLine();

        public int ReadFromConsole()
        {
            ConsoleNative.ReadConsole(_hstdin, out char character, 1, out IntPtr charsRead, IntPtr.Zero);
            return charsRead.ToInt32() == 1 ? character : -1;
        }


        public string ReadLineFromConsole() => ReadLineFromConsole(preserveNewLine: false);
        public string ReadLineFromConsole(bool preserveNewLine)
        {
            var sb = new StringBuilder();
            var buffer = new char[512];
            var charsRead = ReadChunk(buffer);
            sb.Append(buffer, 0, charsRead);
            while (charsRead == buffer.Length)
            {
                charsRead = ReadChunk(buffer);
                sb.Append(buffer, 0, charsRead);
            }
            if (!preserveNewLine)
            {
                sb.Remove(sb.Length - Environment.NewLine.Length, Environment.NewLine.Length);
            }
            return sb.ToString();
        }
        public bool KeyAvailable
        {
            get
            {
                ConsoleNative.GetNumberOfConsoleInputEvents(_hstdin, out uint numEvents);
                var buffer = new INPUT_RECORD[numEvents];
                ConsoleNative.PeekConsoleInput(_hstdin, buffer, (uint)buffer.Length, out uint eventsRead);
                foreach (var record in buffer)
                {
                    if (record.EventType == ConsoleEventTypes.KEY_EVENT && record.KeyEvent.bKeyDown && !record.KeyEvent.wVirtualKeyCode.IsModifierKey())
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public ConsoleKeyInfo ReadKey()
        {
            while (true)
            {
                var evt = ReadConsoleEvent();
                if (evt.EventType == ConsoleEventTypes.KEY_EVENT && evt.KeyEvent.bKeyDown && !!evt.KeyEvent.wVirtualKeyCode.IsModifierKey())
                {
                    return TvConsoleKeyboardEvent.AsConsoleKeyInfo(evt.KeyEvent);
                }
            }
        }

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



        private int ReadChunk(char[] buffer)
        {
            ConsoleNative.ReadConsole(_hstdin, buffer, (uint)buffer.Length, out IntPtr charsRead, IntPtr.Zero);
            return charsRead.ToInt32();
        }

        private uint PeekChunk(INPUT_RECORD[] buffer)
        {
            ConsoleNative.PeekConsoleInput(_hstdin, buffer, (uint)buffer.Length, out uint eventsRead);
            return eventsRead;
        }

        public TvConsoleEvents ReadEvents()
        {
            ConsoleNative.GetNumberOfConsoleInputEvents(_hstdin, out uint numEvents);

            if (numEvents > 0)
            {
                var buffer = new INPUT_RECORD[numEvents];
                ConsoleNative.ReadConsoleInput(_hstdin, buffer, (uint)buffer.Length, out uint eventsRead);
                return new TvConsoleEvents(buffer);
            }
            else
            {
                return TvConsoleEvents.Empty;
            }
        }


        private INPUT_RECORD ReadConsoleEvent()
        {
            var buffer = new INPUT_RECORD[1];
            ConsoleNative.ReadConsoleInput(_hstdin, buffer, (uint)buffer.Length, out uint eventsRead);
            return buffer[0];
        }

    }
}
