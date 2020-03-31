using GameBoy.GB.CpuUnits;
using Xunit;

namespace GameBoy.GB.CpuUnits.Tests
{
    public class BitUnitTests
    {
        [Fact()]
        public void TestBitTest()
        {
            var bitUnit = new BitUnit(new Memory());
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

        [Fact()]
        public void RotateLeftTest()
        {
            // No carry
            var bitUnit = new BitUnit(new Memory());
            byte value = 0b01111110;
            byte flags = 0b01100000;

            bitUnit.RotateLeft(ref value, ref flags);

            Assert.Equal(0b11111100, value);
            Assert.Equal(0b00000000, flags);

            // Test to and from carry
            value = 0b11111110;
            flags = 0b11110000;

            bitUnit.RotateLeft(ref value, ref flags);

            Assert.Equal(0b11111101, value);
            Assert.Equal(0b00010000, flags);

            // Resutl zero
            value = 0x0;
            flags = 0x0;

            bitUnit.RotateLeft(ref value, ref flags);

            Assert.Equal(0x0, value);
            Assert.Equal(0b10000000, flags);
        }

        [Fact()]
        public void RotateLeftTest1()
        {
            var memory = new Memory();
            var bitUnit = new BitUnit(memory);
            byte flags = 0b01100000;
            memory.WriteByte(0xABBA, 0b11111110);

            bitUnit.RotateLeft(0xAB, 0xBA, ref flags);

            Assert.Equal(0b11111101, memory.ReadByte(0xABBA));
            Assert.Equal(0b00010000, flags);
        }
    }
}