using GameBoy.GB.CpuUnits;
using Xunit;

namespace GameBoy.GB.CpuUnits.Tests
{
    public class ALUTests
    {
        [Fact()]
        public void AddSetsHalfCarryTest()
        {
            var memory = new Memory(new byte[0]);
            var alu = new ALU(memory);
            byte flags = 0b11010000;
            byte to = 0xAA;

            alu.Add(ref to, 0x0F, ref flags);

            Assert.Equal(0xB9, to);
            Assert.Equal(0b00100000, flags);
        }

        [Fact()]
        public void AddSetsCarryTest()
        {
            var memory = new Memory(new byte[0]);
            var alu = new ALU(memory);
            byte flags = 0b11000000;
            byte to = 0xFE;

            alu.Add(ref to, 0x03, ref flags);

            Assert.Equal(0x01, to);
            Assert.Equal(0b00110000, flags);
        }

        [Fact()]
        public void AddSetsZeroTest()
        {
            var memory = new Memory(new byte[0]);
            var alu = new ALU(memory);
            byte flags = 0x0;
            byte to = 0x0;

            alu.Add(ref to, 0x0, ref flags);

            Assert.Equal(0x0, to);
            Assert.Equal(0b10000000, flags);
        }

        [Fact()]
        public void AddWithCarrySetsHalfCarryTest()
        {
            var memory = new Memory(new byte[0]);
            var alu = new ALU(memory);
            byte flags = 0b11010000;
            byte to = 0xAA;

            alu.Add(ref to, 0x0F, ref flags, true);

            Assert.Equal(0xBA, to);
            Assert.Equal(0b00100000, flags);
        }

        [Fact()]
        public void AddWithCarrySetsCarryTest()
        {
            var memory = new Memory(new byte[0]);
            var alu = new ALU(memory);
            byte flags = 0b11010000;
            byte to = 0xFE;

            alu.Add(ref to, 0x03, ref flags, true);

            Assert.Equal(0x02, to);
            Assert.Equal(0b00110000, flags);
        }

        [Fact()]
        public void SubtractTest()
        {
            var memory = new Memory(new byte[0]);
            var alu = new ALU(memory);
            byte from = 0x11;
            byte flags = 0b00000000;

            alu.Subtract(ref from, 0x01, ref flags);

            Assert.Equal(0x10, from);
            Assert.Equal(0b01000000, flags);
        }

        [Fact()]
        public void SubtractSetsCarryTest()
        {
            var memory = new Memory(new byte[0]);
            var alu = new ALU(memory);
            byte from = 0xE0;
            byte flags = 0b00000000;

            alu.Subtract(ref from, 0xF0, ref flags);

            Assert.Equal(0xF0, from);
            Assert.Equal(0b01010000, flags);
        }

        [Fact()]
        public void SubtractSetsHalfCarryTest()
        {
            var memory = new Memory(new byte[0]);
            var alu = new ALU(memory);
            byte from = 0x11;
            byte flags = 0b00000000;

            alu.Subtract(ref from, 0x02, ref flags);

            Assert.Equal(0xF, from);
            Assert.Equal(0b01100000, flags);
        }

        [Fact()]
        public void SubtractSetsZeroFlagTest()
        {
            var memory = new Memory(new byte[0]);
            var alu = new ALU(memory);
            byte from = 0xFF;
            byte flags = 0b00000000;

            alu.Subtract(ref from, 0xFF, ref flags);

            Assert.Equal(0x0, from);
            Assert.Equal(0b11000000, flags);
        }

        [Fact()]
        public void SubtractWithCarryTest()
        {
            var memory = new Memory(new byte[0]);
            var alu = new ALU(memory);
            byte from = 0xF;
            byte flags = 0b00010000;

            alu.Subtract(ref from, 0x01, ref flags, true);

            Assert.Equal(0xD, from);
            Assert.Equal(0b01000000, flags);
        }

        [Fact()]
        public void SubtractWithCarrySetsCarryTest()
        {
            var memory = new Memory(new byte[0]);
            var alu = new ALU(memory);
            byte from = 0xE1;
            byte flags = 0b00010000;

            alu.Subtract(ref from, 0xF0, ref flags, true);

            Assert.Equal(0xF0, from);
            Assert.Equal(0b01010000, flags);
        }

        [Fact()]
        public void SubtractWithCarrySetsHalfCarryTest()
        {
            var memory = new Memory(new byte[0]);
            var alu = new ALU(memory);
            byte from = 0x11;
            byte flags = 0b00010000;

            alu.Subtract(ref from, 0x02, ref flags, true);

            Assert.Equal(0xE, from);
            Assert.Equal(0b01100000, flags);
        }

        [Fact()]
        public void SubtractWithCarrySetsZeroFlagTest()
        {
            var memory = new Memory(new byte[0]);
            var alu = new ALU(memory);
            byte from = 0xFF;
            byte flags = 0b00010000;

            alu.Subtract(ref from, 0xFE, ref flags, true);

            Assert.Equal(0x0, from);
            Assert.Equal(0b11000000, flags);
        }

        [Fact()]
        public void AndTest()
        {
            var memory = new Memory(new byte[0]);
            var alu = new ALU(memory);
            byte to = 0xAA;
            byte flags = 0b00000000;

            alu.And(ref to, 0xF2, ref flags);

            Assert.Equal(0xA2, to);
            Assert.Equal(0b00100000, flags);
        }

        [Fact()]
        public void OrTest()
        {
            var memory = new Memory(new byte[0]);
            var alu = new ALU(memory);
            byte to = 0xF0;
            byte flags = 0b11110000;

            alu.Or(ref to, 0x0F, ref flags);

            Assert.Equal(0xFF, to);
            Assert.Equal(0x00, flags);
        }

        [Fact()]
        public void XorTest()
        {
            var memory = new Memory(new byte[0]);
            var alu = new ALU(memory);
            byte to = 0xAF;
            byte flags = 0b11110000;

            alu.Xor(ref to, 0xF3, ref flags);

            Assert.Equal(0x5C, to);
            Assert.Equal(0x00, flags);
        }
    }
}