using System;
using System.Linq;

namespace TvConsole.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            var terminal = TvConsole.Default.GetVirtualTerminal();
            terminal.DrawBorder();


            /* TvConsole.Default.WriteLine("Hello {0} {1}", "World", TvConsole.Default.IsOutputRedirected);
            while (true)
            {

                var events = TvConsole.Default.ReadEvents();
                foreach (var ke in events.KeyboardEvents)
                {
                    var info = ke.AsConsoleKeyInfo();
                    TvConsole.Default.WriteLine($"{info.KeyChar} ({info.Key.ToString()}). shitft: {(info.Modifiers & ConsoleModifiers.Shift) != 0} down: {ke.IsKeyDown}");
                }

                foreach (var me in events.MouseEvents)
                {

                    TvConsole.Default.WriteLine($"Mouse in: ({me.X},{me.Y})");
                }


            }
            */

            TvConsole.Default.Read();

        }
    }
}
