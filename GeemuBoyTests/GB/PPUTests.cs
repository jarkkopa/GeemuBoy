using FakeItEasy;
using Xunit;

namespace GeemuBoy.GB.Tests
{
    public class PPUTests
    {
        [Fact()]
        public void TimingTest()
        {
            // TODO Assert lcd control register values
            var display = A.Fake<IDisplay>();
            var memory = new Memory();
            var ppu = new PPU(memory, display);
            byte control = 0x80;
            memory.WriteByte(0xFF40, control);
            for (int line = 0; line < 154; line++)
            {
                Assert.Equal(line, memory.ReadByte(0xFF44));
                if (line <= 143)
                {
                    // Rendering scanlines
                    Assert.Equal(PPU.Mode.OamSearch, ppu.CurrentMode);
                    ppu.Tick(80);
                    Assert.Equal(PPU.Mode.PixelTransfer, ppu.CurrentMode);
                    ppu.Tick(172);
                    Assert.Equal(PPU.Mode.HBlank, ppu.CurrentMode);
                    ppu.Tick(204);
                    Assert.Equal(line + 1, memory.ReadByte(0xFF44));
                }
                else
                {
                    // V-Blank period (lines 144 - 153)
                    if (line == 144)
                    {
                        byte interruptFlag = memory.ReadByte(0xFF0F);
                        Assert.True(interruptFlag.IsBitSet(0));
                    }
                    // One scanline takes 456 cycles
                    ppu.Tick(456);
                    if (line < 153)
                    {
                        Assert.Equal(PPU.Mode.VBlank, ppu.CurrentMode);
                        Assert.Equal(line + 1, memory.ReadByte(0xFF44));
                    }
                    else
                    {
                        Assert.Equal(PPU.Mode.OamSearch, ppu.CurrentMode);
                        Assert.Equal(0, memory.ReadByte(0xFF44));
                    }
                }
            }
        }
    }
}