using Xunit;

namespace GeemuBoyRomTests.GB.Blargg
{
    public class TimingTests : TestBase
    {
        [Fact()]
        public void InstructionTimings()
        {
            RunTest("Roms/blargg/gb-test-roms/instr_timing/instr_timing.gb", 0xC8B0);
        }
    }
}
