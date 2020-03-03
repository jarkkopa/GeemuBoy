using Xunit;
using GameBoy.GB;
using System;
using System.Collections.Generic;
using System.Text;

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