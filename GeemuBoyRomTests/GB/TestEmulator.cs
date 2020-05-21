using GeemuBoy.GB;
using System.IO;

namespace GeemuBoyRomTests.GB
{
    public class TestEmulator
    {
        public Memory Memory { get; }
        private readonly CPU cpu;
        private readonly PPU ppu;
        private readonly BlankDisplay display;

        private readonly ushort stopAddress;

        public TestEmulator(string romPath, ushort stopAddress)
        {
            this.stopAddress = stopAddress;

            byte[] cartridge = File.ReadAllBytes(romPath);

            Memory = new Memory(cartridge);
            display = new BlankDisplay();
            ppu = new PPU(Memory, display);
            cpu = new CPU(Memory, ppu);
            cpu.SetInitialStateAfterBootSequence();
        }

        public void Run()
        {
            while (cpu.PC != stopAddress)
            {
                cpu.RunCommand();
            }
        }
    }
}
