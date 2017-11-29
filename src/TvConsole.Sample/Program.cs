using System;
using System.Linq;

namespace TvConsole.Sample
{
    class Program
    {
        static void Main(string[] args)
        {

            TvConsole.Instance.WriteLine("Welcome to TvConsole samples");
            TvConsole.Instance.WriteLine();

            DoMenu();
        }

        private static void PrintMenu()
        {
            TvConsole.Instance.WriteLine("1 - Multiple screen buffers");
            TvConsole.Instance.WriteLine("2 - Cursor movement");
            TvConsole.Instance.WriteLine("3 - Fonts & colors");
            TvConsole.Instance.WriteLine("4 - Events");
            TvConsole.Instance.WriteLine("0 - Exit");
        }

        private static void DoMenu()
        {
            TvConsole.Instance.Cls();
            var exit = false;
            while (!exit)
            {

                PrintMenu();
                var key = TvConsole.Instance.ReadKey();

                switch (key.KeyChar)
                {
                    case '1': MultipleScreenBuffers(); break;
                    case '2': CursorMove(); break;
                    case '3': FontsAndColors(); break;
                    case '4': MouseEvents(); break;
                    case '0': exit = true; break;
                    default:
                        TvConsole.Instance.WriteLine($"Invalid otion: {key.KeyChar}");
                        PrintMenu();
                        break;
                }
            }
        }

        private static void MouseEvents()
        {
            TvConsole.Instance.Cls();
            TvConsole.Instance.WriteLine("Move the mouse over the window");
            TvConsole.Instance.DisableInputMode(Win32.ConsoleInputModes.ENABLE_QUICK_EDIT_MODE);
            var finish = false;

            while (!finish)
            {
                var events = TvConsole.Instance.ReadEvents();
                if (events.HasEvents) { TvConsole.Instance.Cls(); }
                foreach (var ke in events.KeyboardEvents)
                {
                    var info = ke.AsConsoleKeyInfo();
                    TvConsole.Instance.WriteLine($"{info.KeyChar} ({info.Key.ToString()}). shitft: {(info.Modifiers & ConsoleModifiers.Shift) != 0} down: {ke.IsKeyDown}");
                    if (ke.AsConsoleKeyInfo().Key == ConsoleKey.Escape)
                    {
                        finish = true;
                    }
                }

                foreach (var me in events.MouseEvents)
                {

                    TvConsole.Instance.WriteLine($"Mouse in: ({me.X},{me.Y})");
                }
            }

                TvConsole.Instance.EnableInputMode(Win32.ConsoleInputModes.ENABLE_QUICK_EDIT_MODE);
        }

        private static void FontsAndColors()
        {
            var values = Enum.GetValues(typeof(ConsoleColor));
            TvConsole.Instance.WriteLine("--- Color Table ---");
            using (TvConsole.Instance.ColorScope)
            {
                foreach (ConsoleColor value in values)
                {
                    TvConsole.Instance.ForegroundColor = value;
                    foreach (ConsoleColor bvalue in values)
                    {
                        TvConsole.Instance.BackgroundColor = bvalue;
                        TvConsole.Instance.Write($"{Enum.GetName(typeof(ConsoleColor), bvalue)}");
                    }
                    TvConsole.Instance.WriteLine();
                }
            }

            TvConsole.Instance.WriteLine("--- End of table color---");
            TvConsole.Instance.ReadLine();
        }



        private static void CursorMove()
        {
            TvConsole.Instance.Cls();
            TvConsole.Instance.WriteLine("Use cursor keys to move the cursor. <Esc> to finish");
            var key = TvConsole.Instance.ReadKey();
            while (key.Key != ConsoleKey.Escape)
            {
                switch (key.Key)
                {
                    case ConsoleKey.UpArrow:
                        TvConsole.Instance.Cursor.Up();
                        break;
                    case ConsoleKey.DownArrow:
                        TvConsole.Instance.Cursor.Down();
                        break;
                    case ConsoleKey.LeftArrow:
                        TvConsole.Instance.Cursor.Left();
                        break;
                    case ConsoleKey.RightArrow:
                        TvConsole.Instance.Cursor.Right();
                        break;
                }

                key = TvConsole.Instance.ReadKey();
            }

            DoMenu();
        }

        private static void MultipleScreenBuffers()
        {
            TvConsole.Instance.Cls();
            TvConsole.Instance.WriteLine("There are 10 screen buffers created (plus the default). Use +/- to iterate between all them. Press <Esc> to finsh.");
            TvConsole.Instance.WriteLine("This is the default screen buffer that can't be deleted.");
            var buffers = new ISecondaryScreenBuffer[10];
            for (var i = 0; i < 10; i++)
            {
                buffers[i] = TvConsole.Instance.CreateNewScreenBuffer();
                buffers[i].WriteLine($"You are now in the screen buffer #{i + 1}");
            }

            var buffidx = 0;
            var key = TvConsole.Instance.ReadKey();
            while (key.Key != ConsoleKey.Escape)
            {
                switch (key.Key)
                {
                    case ConsoleKey.Add:
                        buffidx++;
                        buffidx = buffidx % 11;
                        if (buffidx == 10)
                        {
                            TvConsole.Instance.ActivateDefaultScreenBuffer();
                        }
                        else
                        {
                            TvConsole.Instance.ActivateScreenBuffer(buffers[buffidx]);
                        }
                        break;
                    case ConsoleKey.Subtract:
                        buffidx--;
                        if (buffidx < 0) buffidx = 10;
                        buffidx = buffidx % 11;
                        if (buffidx == 10)
                        {
                            TvConsole.Instance.ActivateDefaultScreenBuffer();
                        }
                        else
                        {
                            TvConsole.Instance.ActivateScreenBuffer(buffers[buffidx]);
                        }
                        break;
                }
                key = TvConsole.Instance.ReadKey();
            }

            foreach (var buffer in buffers) { buffer.Close(); }

            TvConsole.Instance.ActivateDefaultScreenBuffer();
            TvConsole.Instance.WriteLine("Press <enter> to return to menu.");
            TvConsole.Instance.ReadLine();
        }
    }
}
