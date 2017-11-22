using System;
using System.Collections.Generic;
using System.Text;
using TvConsole.Win32;

namespace TvConsole
{
    public class TvConsoleMouseEvent
    {
        private MOUSE_EVENT_RECORD _mouseEvent;

        public int X { get; }
        public int Y { get; }

        public TvConsoleMouseEvent(MOUSE_EVENT_RECORD mouseEvent)
        {
            _mouseEvent = mouseEvent;
            X = _mouseEvent.dwMousePosition.X;
            Y = _mouseEvent.dwMousePosition.Y;
        }
    }
}
