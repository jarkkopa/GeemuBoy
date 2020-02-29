using GameBoy.GB;
using System;
using System.Threading;

namespace GameBoy
{
    class GameBoyEmulator
    {
        private Random _rng;

        public GameBoyEmulator()
        {
            _rng = new Random();

            Console.CursorVisible = false;
            Console.WriteLine("Staring Game Boy Emulator...");
            Console.WriteLine();

            Run();
        }

        private void Run()
        {

            bool quit = false;
            bool running = false;

            PrintCpuDebug();

            while (quit == false)
            {
                if (running)
                {
                    // TODO: run command
                    Thread.Sleep(100);
                    PrintCpuDebug();
                }

                if (Console.KeyAvailable)
                {

                    var key = Console.ReadKey(true).Key;

                    if (key == ConsoleKey.Escape)
                    {
                        quit = true;
                    }
                    else if (key == ConsoleKey.N)
                    {
                        // TODO: run command
                        PrintCpuDebug();
                    }
                    else if (key == ConsoleKey.R)
                    {
                        running = !running;
                    }
                }
            }
        }

        private void PrintCpuDebug()
        {
            var pc = _rng.Next(0, 0xFFFF);
            var sp = _rng.Next(0, 0xFFFF);
            var a = _rng.Next(0, 0xFF);
            var f = _rng.Next(0, 0xFF);
            var b = _rng.Next(0, 0xFF);
            var c = _rng.Next(0, 0xFF);
            var d = _rng.Next(0, 0xFF);
            var e = _rng.Next(0, 0xFF);
            var h = _rng.Next(0, 0xFF);
            var l = _rng.Next(0, 0xFF);

            Console.SetCursorPosition(0, 2);

            Console.WriteLine($"Registers");
            Console.WriteLine($"A: 0x{a:X2} F: 0x{f:X2}");
            Console.WriteLine($"B: 0x{b:X2} C: 0x{c:X2}");
            Console.WriteLine($"D: 0x{d:X2} E: 0x{e:X2}");
            Console.WriteLine($"H: 0x{h:X2} L: 0x{l:X2}");
            Console.WriteLine($"PC: 0x{pc:X4}, SP: 0x{sp:X4}");
        }
    }
}
