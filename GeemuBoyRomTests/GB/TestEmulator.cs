using GeemuBoy.GB;
using System;
using System.IO;

namespace GeemuBoyRomTests.GB
{
    public class TestEmulator
    {
        private readonly Memory memory;
        private readonly CPU cpu;
        private readonly PPU ppu;
        private readonly BlankDisplay display;

        private readonly ushort stopAddress;

        public TestEmulator(string romPath, ushort stopAddress)
        {
            this.stopAddress = stopAddress;

            byte[] cartridge = File.ReadAllBytes(romPath);

            memory = new Memory(cartridge);
            display = new BlankDisplay();
            ppu = new PPU(memory, display);
            cpu = new CPU(memory, ppu);
            cpu.SetInitialStateAfterBootSequence();
        }

        public void Run(Action<Memory> assertResult)
        {
            while (cpu.PC != stopAddress)
            {
                cpu.RunCommand();
            }

            assertResult(memory);
        }
    }
}
