using GameBoy.GB;
using System;
using System.Threading;

namespace GameBoy
{
    class GameBoyEmulator
    {
        private enum State
        {
            Stop,
            Running,
            Quit
        }

        private CPU _cpu;

        private State state = State.Stop;

        private ushort memoryPrintLines = 0;

        public GameBoyEmulator()
        {
            var cartridge = new byte[]
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
            memoryPrintLines = (ushort)cartridge.Length;

            var memory = new Memory(cartridge);
            _cpu = new CPU(memory);

            Console.CursorVisible = false;
            Console.WriteLine("Staring Game Boy Emulator...");
            Console.WriteLine();

            Run();
        }

        private void Run()
        {



            PrintCpuDebug();

            while (state != State.Quit)
            {
                if (state == State.Running)
                {
                    Thread.Sleep(500);
                    Step();
                }

                if (Console.KeyAvailable)
                {

                    var key = Console.ReadKey(true).Key;

                    if (key == ConsoleKey.Escape || key == ConsoleKey.Q)
                    {
                        state = State.Quit;
                    }
                    else if (key == ConsoleKey.N)
                    {
                        Step();
                    }
                    else if (key == ConsoleKey.Spacebar)
                    {
                        state = state == State.Running ? State.Stop : State.Running;
                    }
                    else if (key == ConsoleKey.R)
                    {
                        state = State.Stop;
                        _cpu.Reset();
                        Console.Clear();
                        PrintCpuDebug();
                    }
                }
            }
        }

        private void Step()
        {
            try
            {
                _cpu.RunCommand();
                PrintCpuDebug();
            }
            catch (Exception e)
            {
                Console.WriteLine("Stopped to opcode that is not implemented", e);
                state = State.Stop;
            }
        }

        private void PrintCpuDebug()
        {
            Console.SetCursorPosition(0, 2);

            Console.WriteLine($"PC: 0x{_cpu.PC:X4}, SP: 0x{_cpu.SP:X4}");
            Console.WriteLine("----");
            Console.WriteLine($"Registers");
            Console.WriteLine($"A: 0x{_cpu.A:X2} F: 0x{_cpu.F:X2}");
            Console.WriteLine("----");
            Console.WriteLine($"B: 0x{_cpu.B:X2} C: 0x{_cpu.C:X2}");
            Console.WriteLine($"D: 0x{_cpu.D:X2} E: 0x{_cpu.E:X2}");
            Console.WriteLine($"H: 0x{_cpu.H:X2} L: 0x{_cpu.L:X2}");
            Console.WriteLine("----");

            Console.WriteLine("Memory");
            for (ushort i = 0; i < Math.Min(memoryPrintLines, Memory.MAX_ADDR); i++)
            {
                bool isNextOpCode = _cpu.PC == i;
                var data = _cpu.Memory.ReadByte(i);
                Console.WriteLine($"{(isNextOpCode ? "->" : "  ")} 0x{ data:X2} {(isNextOpCode ? _cpu.GetOpCodeName(data) : "")}");
            }
        }
    }
}
