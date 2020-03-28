using Xunit;

namespace GameBoy.GB.CpuUnits.Tests
{
    public class BitUnitTests
    {
        [Fact()]
        public void TestBitTest()
        {
            var bitUnit = new BitUnit();
            byte value = 0b01010011;

            byte flags = 0b01010000;
            bitUnit.TestBit(value, 7, ref flags);
            Assert.Equal(0b10110000, flags);

            flags = 0b10010000;
            bitUnit.TestBit(value, 6, ref flags);
            Assert.Equal(0b00110000, flags);

            flags = 0b00010000;
            bitUnit.TestBit(value, 5, ref flags);
            Assert.Equal(0b10110000, flags);

            flags = 0b10010000;
            bitUnit.TestBit(value, 4, ref flags);
            Assert.Equal(0b00110000, flags);

            flags = 0b00000000;
            bitUnit.TestBit(value, 3, ref flags);
            Assert.Equal(0b10100000, flags);

            flags = 0b00000000;
            bitUnit.TestBit(value, 2, ref flags);
            Assert.Equal(0b10100000, flags);

            flags = 0b10000000;
            bitUnit.TestBit(value, 1, ref flags);
            Assert.Equal(0b00100000, flags);

            flags = 0b10000000;
            bitUnit.TestBit(value, 0, ref flags);
            Assert.Equal(0b00100000, flags);
        }
    }
}