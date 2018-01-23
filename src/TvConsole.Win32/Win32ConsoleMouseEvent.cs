using System;
using System.Collections.Generic;
using System.Text;
using TvConsole.Extensions;

namespace TvConsole.Win32
{
    public class Win32ConsoleMouseEvent : TvConsoleMouseEvent
    {
        private readonly MOUSE_EVENT_RECORD _mouseEvent;
        public override bool HasButtonPressed(TvMouseButton buttonToCheck) => (_mouseEvent.dwButtonState & (uint)buttonToCheck) == (uint)buttonToCheck;
       
        public Win32ConsoleMouseEvent(MOUSE_EVENT_RECORD mouseEvent) :
            base(mouseEvent.dwMousePosition.X, mouseEvent.dwMousePosition.Y, (TvConsoleMouseEventType)mouseEvent.dwEventFlags)
        {
            _mouseEvent = mouseEvent;
            var modifiers = _mouseEvent.dwControlKeyState.GetModifiers();
            CtrlPressed = modifiers.ctrl;
            AltPressed = modifiers.alt;
            ShiftPressed = modifiers.shift;
        }
    }
}
