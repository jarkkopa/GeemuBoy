using Xunit;

namespace GeemuBoy.GB.CpuUnits.Tests
{
    public class MiscUnitTests
    {
        [Fact()]
        public void SetCarryTest()
        {
            var miscUnit = new MiscUnit(new Memory());
            byte flags = 0b11100000;

            miscUnit.SetCarry(ref flags);

            Assert.Equal(0b10010000, flags);
        }
    }
}