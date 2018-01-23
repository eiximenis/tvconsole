using System;
namespace TvConsole
{
    public abstract class TvConsoleKeyboardEvent
    {
        public bool IsKeyDown { get; }

        public int RepeatCount { get; }

        public char Character { get; }

        protected TvConsoleKeyboardEvent(bool isKeyDown, int repeatCount, char character)
        {
            IsKeyDown = isKeyDown;
            RepeatCount = repeatCount;
            Character = character;
        }

        public abstract ConsoleKeyInfo AsConsoleKeyInfo();
    }
}