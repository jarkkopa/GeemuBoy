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
    }
}