using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using TvConsole.Extensions;
using TvConsole.Win32;

namespace TvConsole
{
    public class TvConsole
    {
        private const int STDIN = -10;
        private const int STDOUT = -11;
        private static Lazy<TvConsole> _instance;

        public static TvConsole Default => _instance.Value;

        private readonly IntPtr _hstdin;
        private readonly IntPtr _hstdout;


        static TvConsole()
        {
            _instance = new Lazy<TvConsole>(() => new TvConsole(allowRedirect: true, forceFileApi: false));
        }

        public bool IsInputRedirected { get; }
        public bool IsOutputRedirected { get; }

        public TextWriter Out { get; }
        public TextReader In { get; }

        public TvConsoleStreamProperties OutProperties { get; }
        public TvConsoleStreamProperties InProperties { get; }
       
        
        public bool IsInputModeEnabled(ConsoleInputModes modeToCheck)
        {
            ConsoleNative.GetConsoleMode(_hstdin, out uint currentMode);
            return (currentMode & (uint)modeToCheck) == (uint)modeToCheck;
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

        private TvConsole(bool allowRedirect, bool forceFileApi)
        {
            _hstdin = ConsoleNative.GetStdHandle(STDIN);
            _hstdout = ConsoleNative.GetStdHandle(STDOUT);

            IsInputRedirected = FileNative.GetFileType(_hstdin) != FILE_TYPE.FILE_TYPE_CHAR;
            IsOutputRedirected = FileNative.GetFileType(_hstdout) != FILE_TYPE.FILE_TYPE_CHAR;
            OutProperties = new TvConsoleStreamProperties((int)ConsoleNative.GetConsoleOutputCP(), System.Console.OutputEncoding, IsOutputRedirected, forceFileApi);
            Out = new StreamWriter(new TvConsoleStream(_hstdout, FileAccess.ReadWrite, OutProperties.UseFileApis), OutProperties.Encoding) { AutoFlush = true };
            InProperties = new TvConsoleStreamProperties((int)ConsoleNative.GetConsoleCP(), System.Console.InputEncoding, IsInputRedirected, forceFileApi);
            In = new StreamReader(new TvConsoleStream(_hstdin, FileAccess.Read, InProperties.UseFileApis), InProperties.Encoding,
                detectEncodingFromByteOrderMarks: false,
                bufferSize: 256,
                leaveOpen: true);

            if (!allowRedirect && (IsInputRedirected || IsOutputRedirected))
            {
                throw new InvalidOperationException("Console input and/or ouput is redirected and this is not allowed by current TvConsole.");
            }
        }

        public void WriteLine(string message) => Out.WriteLine(message);
        public void WriteLine(string format, params object[] @params) => WriteLine(string.Format(format, @params));

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
                if (evt.EventType == ConsoleEventTypes.KEY_EVENT && !!evt.KeyEvent.wVirtualKeyCode.IsModifierKey())
                {
                    return TvConsoleKeyboardEvent.AsConsoleKeyInfo(evt.KeyEvent);
                }
            }
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
