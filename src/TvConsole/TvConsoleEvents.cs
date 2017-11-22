using System;
using System.Collections.Generic;
using System.Text;
using TvConsole.Win32;

namespace TvConsole
{
    public class TvConsoleEvents
    {

        private static TvConsoleEvents _emptyInstance = new TvConsoleEvents();
        private readonly List<TvConsoleKeyboardEvent> _keyboardEvents;

        public static TvConsoleEvents Empty => _emptyInstance;
        public IEnumerable<TvConsoleKeyboardEvent> KeyboardEvents  => _keyboardEvents;

        private TvConsoleEvents()
        {
            _keyboardEvents = new List<TvConsoleKeyboardEvent>();
        }


        public TvConsoleEvents(INPUT_RECORD[] buffer) : this()
        {
            foreach (var record in buffer)
            {
                switch (record.EventType)
                {
                    case ConsoleEventTypes.FOCUS_EVENT:
                        break;
                    case ConsoleEventTypes.KEY_EVENT:
                        _keyboardEvents.Add(new TvConsoleKeyboardEvent(record.KeyEvent));
                        break;
                    case ConsoleEventTypes.MENU_EVENT:
                        break;
                    case ConsoleEventTypes.MOUSE_EVENT:
                        break;
                    case ConsoleEventTypes.WINDOW_BUFFER_SIZE_EVENT:
                        break;
                }
            }
        }
    }
}
