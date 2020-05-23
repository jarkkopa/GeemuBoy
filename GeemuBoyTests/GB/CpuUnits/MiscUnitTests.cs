using GeemuBoy.GB.CpuUnits;
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

        [Fact()]
        public void DecimalAdjustTest()
        {
            var miscUnit = new MiscUnit(new Memory());
            byte flags = 0;

            byte value = 0x01;
            miscUnit.DecimalAdjust(ref value, ref flags);
            Assert.Equal(0x1, value);

            value = 0x0A;
            miscUnit.DecimalAdjust(ref value, ref flags);
            Assert.Equal(0x10, value);

            value = 0x0F;
            miscUnit.DecimalAdjust(ref value, ref flags);
            Assert.Equal(0x15, value);

            flags = 0b01100000;
            value = 0x1F;
            miscUnit.DecimalAdjust(ref value, ref flags);
            Assert.Equal(0x19, value);
        }
    }
}