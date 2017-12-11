namespace TvConsole
{
    public class TvVirtualTerminalCursor : IConsoleCursor
    {

        private readonly TvConsole _console;
        public TvVirtualTerminalCursor(TvConsole console)
        {
            _console = console;
        }
        public void Up()
        {
            _console.Write($"{TvVirtualTerminal.ESC}A");
        }

        public void Down()
        {
            _console.Write($"{TvVirtualTerminal.ESC}B");
        }

        public void Left()
        {
            _console.Write($"{TvVirtualTerminal.ESC}D");
        }

        public void Right()
        {
            _console.Write($"{TvVirtualTerminal.ESC}C");
        }
    }
}