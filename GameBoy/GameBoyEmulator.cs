using GameBoy.GB;
using System;
using System.IO;

namespace GameBoy
{
    class GameBoyEmulator
    {
        private enum State
        {
            Stop,
            Running,
            SetMemoryRead,
            SetBreakpoint,
            Quit
        }

        private readonly CPU cpu;

        private State state = State.Stop;

        private string serial = "";
        private int instructionsRun = 0;

        public GameBoyEmulator(string cartridgePath)
        {
            byte[] cartridge = File.ReadAllBytes(cartridgePath);

            var memory = new Memory(cartridge);
            cpu = new CPU(memory);
            //cpu.SetInitialStateAfterBootSequence();

            Console.CursorVisible = false;
            Console.WriteLine("Starting Game Boy Emulator...");
            Console.WriteLine();

            Run();
        }

        private void Run()
        {
            //PrintCpuDebug();
            PrintSections();

            while (state != State.Quit)
            {
                if (breakpoint.HasValue && cpu.PC == breakpoint.Value)
                {
                    state = State.Stop;
                }

                if (state == State.Running)
                {
                    //Thread.Sleep(20);
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
                        cpu.Reset();
                        Console.Clear();
                        //PrintCpuDebug();
                        PrintSections();
                    }
                    else if (key == ConsoleKey.M)
                    {
                        state = State.SetMemoryRead;
                        //PrintMemoryLocation();
                        PrintSections();
                    }
                    else if (key == ConsoleKey.B)
                    {
                        state = State.SetBreakpoint;
                        PrintSections();
                    }
                }
            }
        }

        private void Step()
        {
            try
            {
                cpu.RunCommand();
                instructionsRun++;
                //PrintCpuDebug();
                PrintSections();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Stopped to opcode that is not implemented. Error: {e.Message}");
                state = State.Stop;
            }
        }

        //private void PrintCpuDebug()
        //{
        //    Console.SetCursorPosition(0, 2);

        //    Console.WriteLine($"Instruction #{instructionsRun}");
        //    Console.WriteLine($"PC: 0x{cpu.PC:X4}, SP: 0x{cpu.SP:X4}");
        //    Console.WriteLine("----");
        //    Console.WriteLine($"Registers");
        //    Console.WriteLine($"A: 0x{cpu.A:X2} F: 0x{cpu.F:X2}");
        //    Console.WriteLine("----");
        //    Console.WriteLine($"B: 0x{cpu.B:X2} C: 0x{cpu.C:X2}");
        //    Console.WriteLine($"D: 0x{cpu.D:X2} E: 0x{cpu.E:X2}");
        //    Console.WriteLine($"H: 0x{cpu.H:X2} L: 0x{cpu.L:X2}");
        //    Console.WriteLine("----");

        //    Console.WriteLine("Memory");
        //    PrintMemory(4, 5);
        //    PrintTestOutput();
        //}

        //private void PrintMemory(int linesBeforePc, int linesAfterPc)
        //{
        //    var prefixed = false;
        //    for (ushort i = (ushort)(Math.Max(cpu.PC - linesBeforePc, 0)); i < cpu.PC + linesAfterPc; i++)
        //    {
        //        var code = cpu.Memory.ReadByte(i);
        //        cpu.OpCodes.TryGetValue(code, out OpCode? opCode);

        //        var name = opCode?.Name ?? (code == CPU.PREFIX_OPCODE ? "Prefix" : "Unknown instruction");

        //        if (prefixed)
        //        {
        //            cpu.OpCodesPrefixed.TryGetValue(code, out OpCode? prefixedOpCode);
        //            name = prefixedOpCode?.Name ?? "Unknown instruction";
        //        }

        //        var isNextOpCode = cpu.PC == i;
        //        var showArrow = isNextOpCode || prefixed;
        //        Console.WriteLine($"{(showArrow ? "->" : "  ")} [0x{i:X4}]: 0x{ code:X2} {(isNextOpCode || prefixed ? name.PadRight(20) : "")}");

        //        prefixed = isNextOpCode && code == CPU.PREFIX_OPCODE; // Reset double arrow for prefixed opcodes
        //    }
        //}

        //private void PrintMemoryLocation()
        //{
        //    Console.Write("Read memory address: ");
        //    var input = Console.ReadLine();
        //    try
        //    {
        //        var address = Convert.ToUInt16(input, 16);

        //        Console.WriteLine($"[0x{address:X4}]: {cpu.Memory.ReadByte(address):x4}");
        //    }
        //    catch (Exception)
        //    {
        //        Console.WriteLine("Invalid memory address.");
        //    }
        //    state = State.Stop;
        //}

        //private void PrintTestOutput()
        //{
        //    var lastByte = cpu.Memory.ReadByte(0xFF01);
        //    if (cpu.Memory.ReadByte(0xFF02) != 0)
        //    {
        //        serial += ".";
        //    }
        //    Console.WriteLine($"Output: {serial}");
        //}

        private void PrintSections()
        {
            PrintInfoSection();
            PrintMemorySection();
            PrintRegisterSection();
            PrintMemoryReadSection();
            PrintBreakpointSection();
            PrintHelpSection();
        }

        private int totalWidth = 100;
        private int leftSectionWidth = 50;

        private void PrintInfoSection()
        {
            Console.SetCursorPosition(0, 0);
            Console.WriteLine("Game Boy Emulator");
            Console.WriteLine($"Instruction #{instructionsRun}");
            Console.WriteLine($"Serial output:{serial}");
        }

        private void PrintMemorySection()
        {
            Console.SetCursorPosition(0, 3);
            PrintHorizontalLine();
            Console.WriteLine("Memory:");
            var prefixed = false;
            var linesBeforePc = 4;
            var linesAfterPc = 6;
            for (short i = (short)(cpu.PC - linesBeforePc); i < cpu.PC + linesAfterPc; i++)
            {
                if (i < 0)
                {
                    Console.WriteLine("   [------]: ----");
                    continue;
                }
                var code = cpu.Memory.ReadByte((ushort)i);
                cpu.OpCodes.TryGetValue(code, out OpCode? opCode);

                var name = opCode?.Name ?? (code == CPU.PREFIX_OPCODE ? "Prefix" : "Unknown instruction");

                if (prefixed)
                {
                    cpu.OpCodesPrefixed.TryGetValue(code, out OpCode? prefixedOpCode);
                    name = prefixedOpCode?.Name ?? "Unknown instruction";
                }

                var isNextOpCode = cpu.PC == i;
                var showArrow = isNextOpCode || prefixed;
                if (!showArrow)
                {
                    name = " ".PadRight(20);
                }
                Console.WriteLine($"{(showArrow ? "->" : "  ")} [0x{i:X4}]: 0x{ code:X2} {name.PadRight(20)}");

                prefixed = isNextOpCode && code == CPU.PREFIX_OPCODE; // Reset double arrow for prefixed opcodes
            }
        }

        private void PrintHorizontalLine()
        {
            Console.WriteLine(new string('=', totalWidth));
        }

        private void PrintRegisterSection()
        {
            string border = "|".PadRight(4);

            Console.SetCursorPosition(leftSectionWidth, 4);
            Console.WriteLine($"{border}Registers:");
            Console.SetCursorPosition(leftSectionWidth, 5);

            Console.WriteLine(border);
            Console.SetCursorPosition(leftSectionWidth, 6);
            Console.WriteLine($"{border}PC: 0x{cpu.PC:x4}");
            Console.SetCursorPosition(leftSectionWidth, 7);
            Console.WriteLine($"{border}SP: 0x{cpu.SP:x4}");

            Console.SetCursorPosition(leftSectionWidth, 8);
            Console.WriteLine(border);
            Console.SetCursorPosition(leftSectionWidth, 9);
            Console.WriteLine($"{border}A: 0x{cpu.A:x2} F: 0x{cpu.F:x2}");
            Console.SetCursorPosition(leftSectionWidth, 10);
            Console.WriteLine($"{border}B: 0x{cpu.B:x2} C: 0x{cpu.C:x2}");
            Console.SetCursorPosition(leftSectionWidth, 11);
            Console.WriteLine($"{border}D: 0x{cpu.D:x2} E: 0x{cpu.E:x2}");
            Console.SetCursorPosition(leftSectionWidth, 12);
            Console.WriteLine($"{border}H: 0x{cpu.H:x2} L: 0x{cpu.L:x2}");

            Console.SetCursorPosition(leftSectionWidth, 13);
            Console.WriteLine(border);
            Console.SetCursorPosition(leftSectionWidth, 14);
            Console.WriteLine($"{border}Flags:");
            Console.SetCursorPosition(leftSectionWidth, 15);
            Console.WriteLine($"{border}Z: {(FlagUtils.GetFlag(Flag.Z, cpu.F) ? "1" : "0")} N: {(FlagUtils.GetFlag(Flag.N, cpu.F) ? "1" : "0")}");
            Console.SetCursorPosition(leftSectionWidth, 16);
            Console.WriteLine($"{border}H: { (FlagUtils.GetFlag(Flag.H, cpu.F) ? "1" : "0")} C: { (FlagUtils.GetFlag(Flag.C, cpu.F) ? "1" : "0")}");
        }

        private ushort lastMemoryPeekAddress = 0;
        private ushort? breakpoint = null;

        private void PrintMemoryReadSection()
        {
            string border = "|".PadRight(4);
            PrintHorizontalLine();
            Console.WriteLine($"Memory: [0x{lastMemoryPeekAddress:x4}]: 0x{cpu.Memory.ReadByte(lastMemoryPeekAddress):x2}");
            if (state == State.SetMemoryRead)
            {
                Console.SetCursorPosition(leftSectionWidth, 18);
                Console.Write(new string(' ', totalWidth - leftSectionWidth));
                Console.SetCursorPosition(leftSectionWidth, 18);
                Console.Write($"{border}Read memory address: ");
                var input = Console.ReadLine();
                try
                {
                    lastMemoryPeekAddress = Convert.ToUInt16(input, 16);
                    Console.SetCursorPosition(0, 18);
                    Console.WriteLine($"Memory: [0x{lastMemoryPeekAddress:x4}]: 0x{cpu.Memory.ReadByte(lastMemoryPeekAddress):x2}");
                    Console.SetCursorPosition(leftSectionWidth, 18);
                    Console.WriteLine($"{border}{new string(' ', totalWidth - leftSectionWidth - 1)}");
                }
                catch (Exception)
                {
                    Console.SetCursorPosition(leftSectionWidth, 18);
                    Console.WriteLine($"{border}Invalid memory address {new string(' ', totalWidth - leftSectionWidth - 23)}");
                }
                state = State.Stop;
            }
            else
            {
                Console.SetCursorPosition(leftSectionWidth, 18);
                Console.WriteLine(border);
            }
        }

        private void PrintBreakpointSection()
        {
            string border = "|".PadRight(4);

            if (breakpoint.HasValue)
            {
                Console.SetCursorPosition(0, 19);
                Console.WriteLine($"Next breakpoint: 0x{breakpoint:x4}");
            }
            else
            {
                Console.SetCursorPosition(0, 19);
                Console.WriteLine("Next breakpoint: -");
            }

            if (state == State.SetBreakpoint)
            {
                Console.SetCursorPosition(leftSectionWidth, 19);
                Console.Write(new string(' ', totalWidth - leftSectionWidth));
                Console.SetCursorPosition(leftSectionWidth, 19);
                Console.Write($"{border}Set breakpoint at: ");
                var input = Console.ReadLine();
                try
                {
                    breakpoint = Convert.ToUInt16(input, 16);
                    Console.SetCursorPosition(0, 19);
                    Console.WriteLine($"Next breakpoint: 0x{breakpoint:x4}");
                    Console.SetCursorPosition(leftSectionWidth, 19);
                    Console.WriteLine($"{border}{new string(' ', totalWidth - leftSectionWidth - 1)}");
                }
                catch (Exception)
                {
                    Console.SetCursorPosition(leftSectionWidth, 19);
                    Console.WriteLine($"{border}Invalid memory address {new string(' ', totalWidth - leftSectionWidth - 23)}");
                }
                state = State.Stop;
            }
            else
            {
                Console.SetCursorPosition(leftSectionWidth, 19);
                Console.WriteLine($"{border}");
            }
        }

        private void PrintHelpSection()
        {
            PrintHorizontalLine();
            Console.WriteLine("Commands:");
            Console.WriteLine("Space - Run");
            Console.WriteLine("N - Run next instruction");
            Console.WriteLine("R - Reset");
            Console.WriteLine("M - Read byte from memory");
            Console.WriteLine("B - Set breakpoint to address");
        }
    }
}
