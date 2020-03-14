using Xunit;

namespace GameBoy.GB.Tests
{
    public class BitUtilsTests
    {
        [Fact()]
        public void BytesToUshortTest()
        {
            ushort result = BitUtils.BytesToUshort(0xAA, 0xFF);

            Assert.Equal(0xAAFF, result);
        }

        [Fact()]
        public void MostSignificantByteTest()
        {
            byte value = BitUtils.MostSignificantByte(0xABCD);

            Assert.Equal(0xAB, value);
        }

        [Fact()]
        public void LeastSignificantByteTest()
        {
            byte value = BitUtils.LeastSignificantByte(0xABCD);

            Assert.Equal(0xCD, value);
        }
    }
}