using Xunit;

namespace GeemuBoy.GB.Tests
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

        [Fact()]
        public void GetBitTest()
        {
            Assert.False(BitUtils.GetBit(0b10000000, 7));
            Assert.True(BitUtils.GetBit(0b01000000, 6));
            Assert.True(BitUtils.GetBit(0b00100000, 5));
            Assert.True(BitUtils.GetBit(0b00010000, 4));
            Assert.True(BitUtils.GetBit(0b00001000, 3));
            Assert.True(BitUtils.GetBit(0b00000100, 2));
            Assert.True(BitUtils.GetBit(0b00000010, 1));
            Assert.True(BitUtils.GetBit(0b00000001, 0));
        }

        [Fact()]
        public void SetBitTest()
        {
            Assert.Equal(0b00001111, BitUtils.SetBit(0b00001011, 2, true));
            Assert.Equal(0b00001011, BitUtils.SetBit(0b00001111, 2, false));
            Assert.Equal(0b00001111, BitUtils.SetBit(0b00001111, 2, true));
            Assert.Equal(0b00001011, BitUtils.SetBit(0b00001011, 2, false));
        }
    }
}