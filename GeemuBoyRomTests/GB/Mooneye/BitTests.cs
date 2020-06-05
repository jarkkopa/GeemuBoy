using Xunit;

namespace GeemuBoyRomTests.GB.Mooneye
{
    public class BitTests : TestBase
    {
        [Fact()]
        public void MemOamTest()
        {
            RunTest("Roms/mooneye/acceptance/bits/mem_oam.gb", 0x486E);
        }

        [Fact()]
        public void RegFTest()
        {
            RunTest("Roms/mooneye/acceptance/bits/reg_f.gb", 0x4B2E);
        }

        [Fact()]
        public void UnusedHwio()
        {
            RunTest("Roms/mooneye/acceptance/bits/unused_hwio-GS.gb", 0x486E);
        }
    }
}
