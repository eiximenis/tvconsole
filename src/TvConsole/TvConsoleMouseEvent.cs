using System;
using System.Collections.Generic;
using System.Text;
using TvConsole.Extensions;
using TvConsole.Win32;

namespace TvConsole
{
    public class TvConsoleMouseEvent
    {
        private MOUSE_EVENT_RECORD _mouseEvent;

        public int X { get; }
        public int Y { get; }
        public bool HasButtonPressed(TvMouseButton buttonToCheck) => (_mouseEvent.dwButtonState & (uint)buttonToCheck) == (uint)buttonToCheck;

        public bool CtrlPressed { get; }
        public bool AltPressed { get; }
        public bool ShiftPressed { get; }

        public TvConsoleMouseEventType EventType { get; }

        public TvConsoleMouseEvent(MOUSE_EVENT_RECORD mouseEvent)
        {
            _mouseEvent = mouseEvent;
            X = _mouseEvent.dwMousePosition.X;
            Y = _mouseEvent.dwMousePosition.Y;
            var modifiers = _mouseEvent.dwControlKeyState.GetModifiers();
            CtrlPressed = modifiers.ctrl;
            AltPressed = modifiers.alt;
            ShiftPressed = modifiers.shift;
            EventType = (TvConsoleMouseEventType)_mouseEvent.dwEventFlags;
        }
    }

    public enum TvMouseButton : uint
    {
        LeftButton = 0x0001,
        RightButton = 0x0002,
        SecondLeftButton = 0x0004,
        ThirdLeftButton = 0x0008,
        FourthLeftButton = 0x0010
    }


}
