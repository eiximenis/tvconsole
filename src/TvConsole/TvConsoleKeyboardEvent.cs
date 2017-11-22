using TvConsole.Extensions;
using TvConsole.Win32;

namespace TvConsole
{
    public class TvConsoleKeyboardEvent
    {
        private readonly  KEY_EVENT_RECORD _keyEvent;

        public bool IsKeyDown => _keyEvent.bKeyDown;

        public int RepeatCount => _keyEvent.wRepeatCount;

        public char Character => _keyEvent.UnicodeChar;

        public System.ConsoleKeyInfo AsConsoleKeyInfo() => TvConsoleKeyboardEvent.AsConsoleKeyInfo(_keyEvent);

        public TvConsoleKeyboardEvent(KEY_EVENT_RECORD keyEvent) => _keyEvent = keyEvent;
        
        public static System.ConsoleKeyInfo AsConsoleKeyInfo(KEY_EVENT_RECORD keyEvent)
        {
            var (ctrl, alt, shift) = keyEvent.dwControlKeyState.GetModifiers();
            return new System.ConsoleKeyInfo(keyEvent.UnicodeChar, (System.ConsoleKey)keyEvent.wVirtualKeyCode, shift, alt, ctrl);
        }


    }
}