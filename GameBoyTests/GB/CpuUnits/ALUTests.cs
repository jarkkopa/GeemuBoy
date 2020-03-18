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

            var cycles = alu.Add(ref to, 0x0F, ref flags);

            Assert.Equal(0xB9, to);
            Assert.Equal(0b00100000, flags);
            Assert.Equal(4, cycles);
        }

        [Fact()]
        public void AddSetsCarryTest()
        {
            var memory = new Memory(new byte[0]);
            var alu = new ALU(memory);
            byte flags = 0b11000000;
            byte to = 0xFE;

            var cycles = alu.Add(ref to, 0x03, ref flags);

            Assert.Equal(0x01, to);
            Assert.Equal(0b00110000, flags);
            Assert.Equal(4, cycles);
        }

        [Fact()]
        public void AddSetsZeroTest()
        {
            var memory = new Memory(new byte[0]);
            var alu = new ALU(memory);
            byte flags = 0x0;
            byte to = 0x0;

            var cycles = alu.Add(ref to, 0x0, ref flags);

            Assert.Equal(0x0, to);
            Assert.Equal(0b10000000, flags);
            Assert.Equal(4, cycles);
        }

        [Fact()]
        public void AddFromMemoryTest()
        {
            var memory = new Memory(new byte[0]);
            var alu = new ALU(memory);
            byte flags = 0b11000000;
            byte to = 0xFE;
            byte addrHigh = 0xAB;
            byte addrLow = 0xCD;
            memory.WriteByte(0xABCD, 0x03);

            var cycles = alu.AddFromMemory(ref to, addrHigh, addrLow, ref flags);

            Assert.Equal(0x01, to);
            Assert.Equal(0b00110000, flags);
            Assert.Equal(8, cycles);
        }

        ///////
        [Fact()]
        public void AddWithCarrySetsHalfCarryTest()
        {
            var memory = new Memory(new byte[0]);
            var alu = new ALU(memory);
            byte flags = 0b11010000;
            byte to = 0xAA;

            var cycles = alu.Add(ref to, 0x0F, ref flags, true);

            Assert.Equal(0xBA, to);
            Assert.Equal(0b00100000, flags);
            Assert.Equal(4, cycles);
        }

        [Fact()]
        public void AddWithCarrySetsCarryTest()
        {
            var memory = new Memory(new byte[0]);
            var alu = new ALU(memory);
            byte flags = 0b11010000;
            byte to = 0xFE;

            var cycles = alu.Add(ref to, 0x03, ref flags, true);

            Assert.Equal(0x02, to);
            Assert.Equal(0b00110000, flags);
            Assert.Equal(4, cycles);
        }

        [Fact()]
        public void AddFromMemoryWithCarryTest()
        {
            var memory = new Memory(new byte[0]);
            var alu = new ALU(memory);
            byte flags = 0b11010000;
            byte to = 0xFE;
            byte addrHigh = 0xAB;
            byte addrLow = 0xCD;
            memory.WriteByte(0xABCD, 0x03);

            var cycles = alu.AddFromMemory(ref to, addrHigh, addrLow, ref flags, true);

            Assert.Equal(0x02, to);
            Assert.Equal(0b00110000, flags);
            Assert.Equal(8, cycles);
        }

        [Fact()]
        public void SubtractTest()
        {
            var memory = new Memory(new byte[0]);
            var alu = new ALU(memory);
            byte from = 0x11;
            byte flags = 0b00000000;

            var cycles = alu.Subtract(ref from, 0x01, ref flags);

            Assert.Equal(0x10, from);
            Assert.Equal(0b01000000, flags);
            Assert.Equal(4, cycles);
        }

        [Fact()]
        public void SubtractSetsCarryTest()
        {
            var memory = new Memory(new byte[0]);
            var alu = new ALU(memory);
            byte from = 0xE0;
            byte flags = 0b00000000;

            var cycles = alu.Subtract(ref from, 0xF0, ref flags);

            Assert.Equal(0xF0, from);
            Assert.Equal(0b01010000, flags);
            Assert.Equal(4, cycles);
        }

        [Fact()]
        public void SubtractSetsHalfCarryTest()
        {
            var memory = new Memory(new byte[0]);
            var alu = new ALU(memory);
            byte from = 0x11;
            byte flags = 0b00000000;

            var cycles = alu.Subtract(ref from, 0x02, ref flags);

            Assert.Equal(0xF, from);
            Assert.Equal(0b01100000, flags);
            Assert.Equal(4, cycles);
        }

        [Fact()]
        public void SubtractSetsZeroFlagTest()
        {
            var memory = new Memory(new byte[0]);
            var alu = new ALU(memory);
            byte from = 0xFF;
            byte flags = 0b00000000;

            var cycles = alu.Subtract(ref from, 0xFF, ref flags);

            Assert.Equal(0x0, from);
            Assert.Equal(0b11000000, flags);
            Assert.Equal(4, cycles);
        }

        [Fact()]
        public void SubtractWithCarryTest()
        {
            var memory = new Memory(new byte[0]);
            var alu = new ALU(memory);
            byte from = 0xF;
            byte flags = 0b00010000;

            var cycles = alu.Subtract(ref from, 0x01, ref flags, true);

            Assert.Equal(0xD, from);
            Assert.Equal(0b01000000, flags);
            Assert.Equal(4, cycles);
        }

        [Fact()]
        public void SubtractWithCarrySetsCarryTest()
        {
            var memory = new Memory(new byte[0]);
            var alu = new ALU(memory);
            byte from = 0xE1;
            byte flags = 0b00010000;

            var cycles = alu.Subtract(ref from, 0xF0, ref flags, true);

            Assert.Equal(0xF0, from);
            Assert.Equal(0b01010000, flags);
            Assert.Equal(4, cycles);
        }

        [Fact()]
        public void SubtractWithCarrySetsHalfCarryTest()
        {
            var memory = new Memory(new byte[0]);
            var alu = new ALU(memory);
            byte from = 0x11;
            byte flags = 0b00010000;

            var cycles = alu.Subtract(ref from, 0x02, ref flags, true);

            Assert.Equal(0xE, from);
            Assert.Equal(0b01100000, flags);
            Assert.Equal(4, cycles);
        }

        [Fact()]
        public void SubtractWithCarrySetsZeroFlagTest()
        {
            var memory = new Memory(new byte[0]);
            var alu = new ALU(memory);
            byte from = 0xFF;
            byte flags = 0b00010000;

            var cycles = alu.Subtract(ref from, 0xFE, ref flags, true);

            Assert.Equal(0x0, from);
            Assert.Equal(0b11000000, flags);
            Assert.Equal(4, cycles);
        }

        [Fact()]
        public void SubtractFromMemoryTest()
        {
            var memory = new Memory(new byte[0]);
            var alu = new ALU(memory);
            byte from = 0xFF;
            byte addrHigh = 0xAC;
            byte addrLow = 0xDC;
            byte flags = 0b00000000;
            memory.WriteByte(0xACDC, 0xF0);

            var cycles = alu.SubtractFromMemory(ref from, addrHigh, addrLow, ref flags);

            Assert.Equal(0x0F, from);
            Assert.Equal(0b01000000, flags);
            Assert.Equal(8, cycles);
        }

        [Fact()]
        public void SubtractWithCarryFromMemoryTest()
        {
            var memory = new Memory(new byte[0]);
            var alu = new ALU(memory);
            byte from = 0x11;
            byte addrHigh = 0xAC;
            byte addrLow = 0xDC;
            byte flags = 0b00010000;
            memory.WriteByte(0xACDC, 0xF1);

            var cycles = alu.SubtractFromMemory(ref from, addrHigh, addrLow, ref flags, true);

            Assert.Equal(0x1F, from);
            Assert.Equal(0b01110000, flags);
            Assert.Equal(8, cycles);
        }
    }
}