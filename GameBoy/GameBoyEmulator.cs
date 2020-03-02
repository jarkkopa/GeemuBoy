using GameBoy.GB;
using System;
using System.Threading;

namespace GameBoy
{
    class GameBoyEmulator
    {
        private CPU _cpu;

        public GameBoyEmulator()
        {
            _cpu = new CPU();

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
                    Thread.Sleep(500);
                    Step();
                }

                if (Console.KeyAvailable)
                {

                    var key = Console.ReadKey(true).Key;

                    if (key == ConsoleKey.Escape || key == ConsoleKey.Q)
                    {
                        quit = true;
                    }
                    else if (key == ConsoleKey.N)
                    {
                        Step();
                    }
                    else if (key == ConsoleKey.Spacebar)
                    {
                        running = !running;
                    }
                    else if (key == ConsoleKey.R)
                    {
                        running = false;
                        _cpu.Reset();
                        Console.Clear();
                        PrintCpuDebug();
                    }
                }
            }
        }

        private void Step()
        {
            _cpu.RunCommand();
            PrintCpuDebug();
        }

        private void PrintCpuDebug()
        {
            Console.SetCursorPosition(0, 2);

            Console.WriteLine($"PC: 0x{_cpu.PC:X4}, SP: 0x{_cpu.SP:X4}");
            Console.WriteLine("----");
            Console.WriteLine($"Registers");
            Console.WriteLine($"A: 0x{_cpu.a:X2} F: 0x{_cpu.f:X2}");
            Console.WriteLine("----");
            Console.WriteLine($"B: 0x{_cpu.b:X2} C: 0x{_cpu.c:X2}");
            Console.WriteLine($"D: 0x{_cpu.d:X2} E: 0x{_cpu.e:X2}");
            Console.WriteLine($"H: 0x{_cpu.h:X2} L: 0x{_cpu.l:X2}");
            Console.WriteLine("----");

            Console.WriteLine("Memory");
            for (int i = 0; i < _cpu.Memory.Length; i++)
            {
                bool isNextOpCode = _cpu.PC == i;
                var data = _cpu.Memory[i];
                Console.WriteLine($"{(isNextOpCode ? "->" : "  ")} { data:X2} {(isNextOpCode ? _cpu.GetOpCodeName(data) : "")}");
            }
        }
    }
}
