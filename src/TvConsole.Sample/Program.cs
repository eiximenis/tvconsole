using System;
using System.Linq;

namespace TvConsole.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            TvConsole.Default.WriteLine("Hello {0} {1}", "World", TvConsole.Default.IsOutputRedirected);
            var a = TvConsole.Default.ReadLine();
            TvConsole.Default.WriteLine("a is -> " + a);
            var b = TvConsole.Default.ReadLineFromConsole();
            TvConsole.Default.WriteLine("b is -> " + b);
            int i = 0;
            //while (true)
            //{

            //    var events = TvConsole.Default.ReadEvents();
            //    foreach (var ke in events.KeyboardEvents)
            //    {
            //        var info = ke.AsConsoleKeyInfo();
            //        TvConsole.Default.WriteLine($"{info.KeyChar} ({info.Key.ToString()}). shitft: {(info.Modifiers & ConsoleModifiers.Shift) != 0} down: {ke.IsKeyDown}");
            //    }

            //}

        }
    }
}
