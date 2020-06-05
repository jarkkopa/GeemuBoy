using GeemuBoy.GB;
using Xunit;

namespace GeemuBoyRomTests.GB.Blargg
{
    public class InterruptTests : TestBase
    {
        [Fact()]
        public void HaltBug()
        {

            RunTest("Roms/blargg/gb-test-roms/halt_bug.gb", 0xC818, AssertResultFromVRAM);
        }

        private void AssertResultFromVRAM(Memory memory)
        {
            Assert.Equal((byte)'P', memory.ReadByte(0x99A0));
            Assert.Equal((byte)'a', memory.ReadByte(0x99A1));
            Assert.Equal((byte)'s', memory.ReadByte(0x99A2));
            Assert.Equal((byte)'s', memory.ReadByte(0x99A3));
            Assert.Equal((byte)'e', memory.ReadByte(0x99A4));
            Assert.Equal((byte)'d', memory.ReadByte(0x99A5));
        }
    }
}
