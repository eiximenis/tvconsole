using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TvConsole.Extensions;
using TvConsole.Shared;

namespace TvConsole.Win32
{
    public class Win32ConsolePal : IConsolePal
    {
        private const int STDIN = -10;
        private const int STDOUT = -11;

        private readonly IntPtr _hstdin;
        private readonly IntPtr _hstdout;

        public bool IsInputRedirected { get; }
        public bool IsOutputRedirected { get; }

        public ISecondaryScreenBuffer DefaultBuffer { get; }

        public IFontManager FontManager { get; }

        public TextReader In { get; }

        public bool KeyAvaliable
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

        public Win32ConsolePal(TvConsoleStreamProperties streamProps)
        {
            _hstdin = ConsoleNative.GetStdHandle(STDIN);
            _hstdout = ConsoleNative.GetStdHandle(STDOUT);
            IsInputRedirected = FileNative.GetFileType(_hstdin) != FILE_TYPE.FILE_TYPE_CHAR;
            IsOutputRedirected = FileNative.GetFileType(_hstdout) != FILE_TYPE.FILE_TYPE_CHAR;
            FontManager = new Win32TvFontManager(_hstdout);
            DefaultBuffer = new Win32TvScreenBuffer(_hstdout, IsOutputRedirected, disposable: false);

            In = new StreamReader(new Win32TvConsoleStream(_hstdin, FileAccess.Read, streamProps.UseFileApis), streamProps.Encoding,
                detectEncodingFromByteOrderMarks: false,
                bufferSize: 256,
                leaveOpen: true);
        }


        public TvConsoleEvents ReadEvents()
        {
            ConsoleNative.GetNumberOfConsoleInputEvents(_hstdin, out uint numEvents);

            if (numEvents > 0)
            {
                var buffer = new INPUT_RECORD[numEvents];
                ConsoleNative.ReadConsoleInput(_hstdin, buffer, (uint)buffer.Length, out uint eventsRead);
                return new TvConsoleEvents().Add(buffer);
            }
            else
            {
                return TvConsoleEvents.Empty;
            }
        }

        public ConsoleKeyInfo ReadKey()
        {
            while (true)
            {
                var evt = ReadConsoleEvent();
                if (evt.EventType == ConsoleEventTypes.KEY_EVENT && evt.KeyEvent.bKeyDown && !!evt.KeyEvent.wVirtualKeyCode.IsModifierKey())
                {
                    return Win32ConsoleKeyboardEvent.AsConsoleKeyInfo(evt.KeyEvent);
                }
            }

        }

        private INPUT_RECORD ReadConsoleEvent()
        {
            var buffer = new INPUT_RECORD[1];
            ConsoleNative.ReadConsoleInput(_hstdin, buffer, (uint)buffer.Length, out uint eventsRead);
            return buffer[0];
        }

        public void EnableMouseSupport()
        {
            DisableInputMode(ConsoleInputModes.ENABLE_QUICK_EDIT_MODE);
        }

        public void DisableMouseSupport()
        {
            EnableInputMode(ConsoleInputModes.ENABLE_QUICK_EDIT_MODE);
        }

        private bool IsInputModeEnabled(ConsoleInputModes modeToCheck)
        {
            ConsoleNative.GetConsoleMode(_hstdin, out uint currentMode);
            return (currentMode & (uint)modeToCheck) == (uint)modeToCheck;
        }


        private void EnableInputMode(ConsoleInputModes modeToEnable)
        {
            ConsoleNative.GetConsoleMode(_hstdin, out uint currentMode);
            var newMode = (uint)currentMode | (uint)modeToEnable;
            ConsoleNative.SetConsoleMode(_hstdin, newMode);
        }

        private void DisableInputMode(ConsoleInputModes modeToDisable)
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

    }
}
