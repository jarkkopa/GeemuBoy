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
            var memory = new byte[]
            {
                0x06, // LD B, 0xFE
                0x06,
                0x0E, // LD C, 0x08
                0x0E,
                0x16, // LD D, 0x16
                0x16,
                0x1E, // LD E, 0x1E
                0x1E,
                0x26, // LD H, 0x26
                0x26,
                0x2E, // LD L, 0x2E
                0x2E,
            };
            _cpu = new CPU(memory);

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
                Console.WriteLine($"{(isNextOpCode ? "->" : "  ")} 0x{ data:X2} {(isNextOpCode ? _cpu.GetOpCodeName(data) : "")}");
            }
        }
    }
}
