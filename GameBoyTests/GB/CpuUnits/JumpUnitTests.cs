using Xunit;

namespace GameBoy.GB.CpuUnits.Tests
{
    public class JumpUnitTests
    {
        [Fact()]
        public void JumpToAddressTest()
        {
            var memory = new Memory(new byte[0]);
            var jumpUnit = new JumpUnit(memory);
            ushort pc = 0x0000;

            jumpUnit.JumpToAddress(0xABCD, ref pc);

            Assert.Equal(0xABCD, pc);
        }
    }
}