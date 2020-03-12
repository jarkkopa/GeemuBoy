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
    }
}