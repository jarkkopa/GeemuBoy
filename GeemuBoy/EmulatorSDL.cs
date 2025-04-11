using GeemuBoy.GB;
using SDL3;
using System;
using System.IO;
using System.Text;

namespace GeemuBoy;

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
    
    private const int TotalWidth = 100;
    private const int LeftSectionWidth = 50;
    private const uint TargetFrameTime = 16;

    private State emulatorState;
    
    private readonly CPU cpu;
    private readonly Memory memory;
    private readonly PPU ppu;
    private readonly SDLDisplay display;

    private bool readyToRender;

    private ulong frameStartTime;
    private int instructionsRun;
    private bool printDebug;

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

        PrintDebugger();
        printDebug = false;

        ppu.RenderEvent += RenderHandler;

        emulatorState = State.Running;
        Run();
    }

    private void Run()
    {
        PrintDebugger();
            
        while (emulatorState != State.Quit)
        {
            frameStartTime = SDL.GetTicks();

            PollEvents();

            readyToRender = false;
            while (emulatorState == State.Running && !readyToRender)
            {
                Step();

                if (breakpoint.HasValue && cpu.PC == breakpoint.Value)
                {
                    emulatorState = State.Stop;
                    printDebug = true;
                    breakpoint = null;
                    PrintDebugger();
                }

                if (printDebug)
                {
                    // Poll events while printing because waiting for rendering takes too much time
                    PollEvents();
                }
            }

            var frameTotal = SDL.GetTicks() - frameStartTime;
            if (frameTotal < TargetFrameTime)
            {
                SDL.Delay((uint)(TargetFrameTime - frameTotal));
            }

            display.Render();
        }

        display.Dispose();
    }

    private void PollEvents()
    {
        while (SDL.PollEvent(out var evt))
        {
            switch ((SDL.EventType)evt.Type)
            {
                case SDL.EventType.Quit:
                    emulatorState = State.Quit;
                    break;
                case SDL.EventType.KeyUp:
                    HandleInputs();
                    break;
                case SDL.EventType.KeyDown:
                    switch (evt.Key.Key)
                    {
                        case SDL.Keycode.Q:
                        case SDL.Keycode.Escape:
                            emulatorState = State.Quit;
                            break;
                        case SDL.Keycode.N:
                            Step();
                            break;
                        case SDL.Keycode.Space:
                            emulatorState = emulatorState == State.Stop ? State.Running : State.Stop;
                            break;
                        case SDL.Keycode.P:
                            printDebug = !printDebug;
                            break;
                        case SDL.Keycode.B:
                            emulatorState = State.SetBreakpoint;
                            PrintDebugger();
                            break;
                        case SDL.Keycode.M:
                            emulatorState = State.SetMemoryRead;
                            PrintDebugger();
                            break;
                        case SDL.Keycode.Alpha1:
                            ppu.PrintBackgroundTileNumbers();
                            break;
                        case SDL.Keycode.Alpha2:
                            ppu.PrintBackgroundTileAddresses();
                            break;
                        default:
                            HandleInputs();
                            break;
                    }
                    break;
            }
        }
    }

    private void HandleInputs()
    {
        var state = SDL.GetKeyboardState(out _);
        var keys = InputRegister.Keys.None;
        if (state[(int)SDL.Scancode.Down])
        {
            keys |= InputRegister.Keys.Down;
        }
        if (state[(int)SDL.Scancode.Up])
        {
            keys |= InputRegister.Keys.Up;
        }
        if (state[(int)SDL.Scancode.Left])
        {
            keys |= InputRegister.Keys.Left;
        }
        if (state[(int)SDL.Scancode.Right])
        {
            keys |= InputRegister.Keys.Right;
        }
        if (state[(int)SDL.Scancode.A])
        {
            keys |= InputRegister.Keys.Start;
        }
        if (state[(int)SDL.Scancode.S])
        {
            keys |= InputRegister.Keys.Select;
        }
        if (state[(int)SDL.Scancode.Z])
        {
            keys |= InputRegister.Keys.B;
        }
        if (state[(int)SDL.Scancode.X])
        {
            keys |= InputRegister.Keys.A;
        }

        cpu.HandleInput(keys);
    }

    private void RenderHandler()
    {
        readyToRender = true;
    }

    private void Step()
    {
        cpu.RunCommand();
        instructionsRun++;
        PrintDebugger();
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
        Console.WriteLine($"Serial output: {SerialAsString()}");
    }

    private string SerialAsString()
    {
        StringBuilder sb = new StringBuilder();
        foreach (byte b in memory.Serial)
        {
            sb.Append($"{b:X2} ");
        }
        return sb.ToString();
    }

    private void PrintMemorySection()
    {
        Console.SetCursorPosition(0, 3);
        PrintHorizontalLine();
        Console.WriteLine("Memory:");
        var prefixed = false;
        var linesBeforePc = 4;
        var linesAfterPc = 6;
        for (var i = cpu.PC - linesBeforePc; i < cpu.PC + linesAfterPc && i < 0xFFFF; i++)
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
        Console.WriteLine(new string('=', TotalWidth));
    }

    private void PrintRegisterSection()
    {
        string border = "|".PadRight(4);

        Console.SetCursorPosition(LeftSectionWidth, 4);
        Console.WriteLine($"{border}Registers:");
        Console.SetCursorPosition(LeftSectionWidth, 5);

        Console.WriteLine(border);
        Console.SetCursorPosition(LeftSectionWidth, 6);
        Console.WriteLine($"{border}PC: 0x{cpu.PC:X4}");
        Console.SetCursorPosition(LeftSectionWidth, 7);
        Console.WriteLine($"{border}SP: 0x{cpu.SP:X4}");

        Console.SetCursorPosition(LeftSectionWidth, 8);
        Console.WriteLine(border);
        Console.SetCursorPosition(LeftSectionWidth, 9);
        Console.WriteLine($"{border}A: 0x{cpu.A:X2} F: 0x{cpu.F:X2}");
        Console.SetCursorPosition(LeftSectionWidth, 10);
        Console.WriteLine($"{border}B: 0x{cpu.B:X2} C: 0x{cpu.C:X2}");
        Console.SetCursorPosition(LeftSectionWidth, 11);
        Console.WriteLine($"{border}D: 0x{cpu.D:X2} E: 0x{cpu.E:X2}");
        Console.SetCursorPosition(LeftSectionWidth, 12);
        Console.WriteLine($"{border}H: 0x{cpu.H:x2} L: 0x{cpu.L:x2}");

        Console.SetCursorPosition(LeftSectionWidth, 13);
        Console.WriteLine(border);
        Console.SetCursorPosition(LeftSectionWidth, 14);
        Console.WriteLine($"{border}Flags:");
        Console.SetCursorPosition(LeftSectionWidth, 15);
        Console.WriteLine($"{border}Z: {(FlagUtils.GetFlag(Flag.Z, cpu.F) ? "1" : "0")} N: {(FlagUtils.GetFlag(Flag.N, cpu.F) ? "1" : "0")}");
        Console.SetCursorPosition(LeftSectionWidth, 16);
        Console.WriteLine($"{border}H: { (FlagUtils.GetFlag(Flag.H, cpu.F) ? "1" : "0")} C: { (FlagUtils.GetFlag(Flag.C, cpu.F) ? "1" : "0")}");

        Console.SetCursorPosition(LeftSectionWidth, 17);
        Console.WriteLine($"{border}DIV: 0x{ memory.ReadByte(0xFF04):X2} TIMA: 0x{ memory.ReadByte(0xFF05):X2}");
    }

    private ushort lastMemoryPeekAddress;
    private ushort? breakpoint;

    private void PrintMemoryReadSection()
    {
        var border = "|".PadRight(4);
        PrintHorizontalLine();
        Console.WriteLine($"Memory: [0x{lastMemoryPeekAddress:X4}]: 0x{memory.ReadByte(lastMemoryPeekAddress):X2}");
        if (emulatorState == State.SetMemoryRead)
        {
            Console.SetCursorPosition(LeftSectionWidth, 18);
            Console.Write(new string(' ', TotalWidth - LeftSectionWidth));
            Console.SetCursorPosition(LeftSectionWidth, 18);
            Console.Write($"{border}Read memory address: ");
            var input = Console.ReadLine();
            try
            {
                lastMemoryPeekAddress = Convert.ToUInt16(input, 16);
                Console.SetCursorPosition(0, 18);
                Console.WriteLine($"Memory: [0x{lastMemoryPeekAddress:X4}]: 0x{memory.ReadByte(lastMemoryPeekAddress):X2}");
                Console.SetCursorPosition(LeftSectionWidth, 18);
                Console.WriteLine($"{border}{new string(' ', TotalWidth - LeftSectionWidth - 1)}");
            }
            catch (Exception)
            {
                Console.SetCursorPosition(LeftSectionWidth, 18);
                Console.WriteLine($"{border}Invalid memory address {new string(' ', TotalWidth - LeftSectionWidth - 23)}");
            }
            emulatorState = State.Stop;
        }
        else
        {
            Console.SetCursorPosition(LeftSectionWidth, 18);
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

        if (emulatorState == State.SetBreakpoint)
        {
            Console.SetCursorPosition(LeftSectionWidth, 19);
            Console.Write(new string(' ', TotalWidth - LeftSectionWidth));
            Console.SetCursorPosition(LeftSectionWidth, 19);
            Console.Write($"{border}Set breakpoint at: ");
            var input = Console.ReadLine();
            try
            {
                breakpoint = Convert.ToUInt16(input, 16);
                Console.SetCursorPosition(0, 19);
                Console.WriteLine($"Next breakpoint: 0x{breakpoint:X4}");
                Console.SetCursorPosition(LeftSectionWidth, 19);
                Console.WriteLine($"{border}{new string(' ', TotalWidth - LeftSectionWidth - 1)}");
            }
            catch (Exception)
            {
                Console.SetCursorPosition(LeftSectionWidth, 19);
                Console.WriteLine($"{border}Invalid memory address {new string(' ', TotalWidth - LeftSectionWidth - 23)}");
            }
            emulatorState = State.Stop;
        }
        else
        {
            Console.SetCursorPosition(LeftSectionWidth, 19);
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