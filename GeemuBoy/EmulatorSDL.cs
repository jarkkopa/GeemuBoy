using GeemuBoy.GB;
using SDL2;
using System;
using System.IO;
namespace GeemuBoy
{
    class EmulatorSDL
    {
        private enum State
        {
            Stop,
            Running,
            SetMemoryRead,
            SetBreakpoint,
            Quit
        }

        private State state = State.Stop;

        private readonly CPU cpu;
        private readonly Memory memory;
        private readonly PPU ppu;
        private readonly SDLDisplay display;

        private string serial = "";
        private int instructionsRun = 0;

        private readonly int totalWidth = 100;
        private readonly int leftSectionWidth = 50;

        private bool printDebug = true;

        public EmulatorSDL(string? cartridgePath, string? bootRomPath)
        {
            byte[]? cartridge = null;
            if (!string.IsNullOrEmpty(cartridgePath))
            {
                cartridge = File.ReadAllBytes(cartridgePath);
            }

            byte[]? bootRom = null;
            if (!string.IsNullOrEmpty(bootRomPath))
            {
                bootRom = File.ReadAllBytes(bootRomPath);
            }

            memory = new Memory(cartridge, bootRom);
            display = new SDLDisplay();
            ppu = new PPU(memory, display);
            cpu = new CPU(memory, ppu);
            if (bootRom == null)
            {
                cpu.SetInitialStateAfterBootSequence();
            }

            Run();
        }

        private void Run()
        {
            PrintDebugger();

            while (state != State.Quit)
            {
                while (SDL.SDL_PollEvent(out SDL.SDL_Event evt) != 0)
                {
                    switch (evt.type)
                    {
                        case SDL.SDL_EventType.SDL_QUIT:
                            state = State.Quit;
                            break;
                        case SDL.SDL_EventType.SDL_KEYDOWN:
                            switch (evt.key.keysym.sym)
                            {
                                case SDL.SDL_Keycode.SDLK_q:
                                case SDL.SDL_Keycode.SDLK_ESCAPE:
                                    state = State.Quit;
                                    break;
                                case SDL.SDL_Keycode.SDLK_n:
                                    Step();
                                    break;
                                case SDL.SDL_Keycode.SDLK_SPACE:
                                    state = state == State.Stop ? State.Running : State.Stop;
                                    break;
                                case SDL.SDL_Keycode.SDLK_p:
                                    printDebug = !printDebug;
                                    break;
                                case SDL.SDL_Keycode.SDLK_b:
                                    state = State.SetBreakpoint;
                                    PrintDebugger();
                                    break;
                                case SDL.SDL_Keycode.SDLK_m:
                                    state = State.SetMemoryRead;
                                    PrintDebugger();
                                    break;
                            }
                            break;
                    }
                }

                if (breakpoint.HasValue && cpu.PC == breakpoint.Value)
                {
                    state = State.Stop;
                    printDebug = true;
                    breakpoint = null;
                    PrintDebugger();
                }

                if (state == State.Running)
                {
                    Step();
                }
            }

            display.Dispose();
        }

        private void Step()
        {
            try
            {
                cpu.RunCommand();
                instructionsRun++;
                PrintDebugger();
            }
            catch (NotImplementedException e)
            {
                Console.WriteLine($"Stopped to opcode that is not implemented. Error: {e.Message}");
                state = State.Stop;
            }
        }

        private void PrintDebugger()
        {
            if (printDebug)
            {
                PrintInfoSection();
                PrintMemorySection();
                PrintRegisterSection();
                PrintMemoryReadSection();
                PrintBreakpointSection();
                PrintHelpSection();
            }
        }

        private void PrintInfoSection()
        {
            Console.SetCursorPosition(0, 0);
            Console.WriteLine("Emulator");
            Console.WriteLine($"Instruction #{instructionsRun}" +
                $" PPU MODE: {ppu.CurrentMode.ToString().PadRight(20)}");
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
            for (int i = cpu.PC - linesBeforePc; i < cpu.PC + linesAfterPc && i < 0xFFFF; i++)
            {
                if (i < 0)
                {
                    Console.WriteLine("   [------]: ----");
                    continue;
                }
                var code = memory.ReadByte((ushort)i);
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
            Console.WriteLine($"{border}PC: 0x{cpu.PC:X4}");
            Console.SetCursorPosition(leftSectionWidth, 7);
            Console.WriteLine($"{border}SP: 0x{cpu.SP:X4}");

            Console.SetCursorPosition(leftSectionWidth, 8);
            Console.WriteLine(border);
            Console.SetCursorPosition(leftSectionWidth, 9);
            Console.WriteLine($"{border}A: 0x{cpu.A:X2} F: 0x{cpu.F:X2}");
            Console.SetCursorPosition(leftSectionWidth, 10);
            Console.WriteLine($"{border}B: 0x{cpu.B:X2} C: 0x{cpu.C:X2}");
            Console.SetCursorPosition(leftSectionWidth, 11);
            Console.WriteLine($"{border}D: 0x{cpu.D:X2} E: 0x{cpu.E:X2}");
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
            Console.WriteLine($"Memory: [0x{lastMemoryPeekAddress:X4}]: 0x{memory.ReadByte(lastMemoryPeekAddress):X2}");
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
                    Console.WriteLine($"Memory: [0x{lastMemoryPeekAddress:X4}]: 0x{memory.ReadByte(lastMemoryPeekAddress):X2}");
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
                    Console.WriteLine($"Next breakpoint: 0x{breakpoint:X4}");
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
            Console.WriteLine("N - Step to next instruction");
            Console.WriteLine("P - Enable / disable debug print");
            Console.WriteLine("M - Read byte from memory");
            Console.WriteLine("B - Set breakpoint to address");
        }
    }
}
