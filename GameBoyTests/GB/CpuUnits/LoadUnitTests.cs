using GameBoy.GB.CpuUnits;
using Xunit;

namespace GameBoy.GB.CpuUnits.Tests
{
    public class LoadUnitTests
    {
        [Fact()]
        public void CopyTest()
        {
            var memory = new Memory(new byte[0]);
            var loadUnit = new LoadUnit(memory);
            byte to = 0x00;
            byte from = 0xFF;

            var cycles = loadUnit.Copy(ref to, from);

            Assert.Equal(0xFF, to);
            Assert.Equal(4, cycles);
        }

        [Fact()]
        public void LoadImmediate8Test()
        {
            var memory = new Memory(new byte[]
            {
                0xFF
            });
            var loadUnit = new LoadUnit(memory);

            byte to = 0x00;
            ushort PC = 0;

            var cycles = loadUnit.LoadImmediateByte(ref to, ref PC);

            Assert.Equal(0xFF, to);
            Assert.Equal(1, PC);
            Assert.Equal(8, cycles);
        }

        [Fact()]
        public void LoadFromCombinedAddressTest()
        {
            var memory = new Memory(new byte[0]);
            var loadUnit = new LoadUnit(memory);
            byte to = 0x00;
            memory.WriteByte(0xABBA, 0x10);

            var cycles = loadUnit.LoadFromAddress(ref to, 0xAB, 0xBA);

            Assert.Equal(0x10, to);
            Assert.Equal(8, cycles);
        }

        [Fact()]
        public void LoadFromAddressTest()
        {
            var memory = new Memory(new byte[0]);
            var loadUnit = new LoadUnit(memory);
            byte to = 0x00;
            memory.WriteByte(0xABBA, 0x10);

            var cycles = loadUnit.LoadFromAddress(ref to, 0xAB, 0xBA);

            Assert.Equal(0x10, to);
            Assert.Equal(8, cycles);
        }

        [Fact()]
        public void LoadImmediate8ToAddressTest()
        {
            var memory = new Memory(new byte[]
            {
                0xAA
            });
            var loadUnit = new LoadUnit(memory);
            ushort PC = 0x00;

            var cycles = loadUnit.LoadImmediateByteToAddress(0xCC, 0xDD, ref PC);

            Assert.Equal(0xAA, memory.ReadByte(0xCCDD));
            Assert.Equal(0x01, PC);
            Assert.Equal(12, cycles);
        }

        [Fact()]
        public void LoadFromImmediateAddressTest()
        {
            var memory = new Memory(new byte[]
            {
                0xCC,
                0xDD
            });
            var loadUnit = new LoadUnit(memory);
            ushort PC = 0x00;
            byte to = 0x00;
            memory.WriteByte(0xCCDD, 0xAA);

            var cycles = loadUnit.LoadFromImmediateAddress(ref to, ref PC);

            Assert.Equal(0xAA, to);
            Assert.Equal(0x02, PC);
            Assert.Equal(16, cycles);
        }

        [Fact()]
        public void WriteToCombinedAddressTest()
        {
            var memory = new Memory(new byte[0]);
            var loadUnit = new LoadUnit(memory);
            byte source = 0x66;

            var cycles = loadUnit.WriteToAddress(0xAB, 0xBA, source);

            Assert.Equal(0x66, memory.ReadByte(0xABBA));
            Assert.Equal(8, cycles);
        }

        [Fact()]
        public void WriteToAddressTest()
        {
            var memory = new Memory(new byte[0]);
            var loadUnit = new LoadUnit(memory);
            byte source = 0x66;

            var cycles = loadUnit.WriteToAddress(0xABBA, source);

            Assert.Equal(0x66, memory.ReadByte(0xABBA));
            Assert.Equal(8, cycles);
        }

        [Fact()]
        public void WriteImmediateAddressTest()
        {
            var memory = new Memory(new byte[]
            {
                0xAB,
                0xBA
            });
            var loadUnit = new LoadUnit(memory);
            byte source = 0xDD;
            ushort PC = 0x00;

            var cycles = loadUnit.WriteToImmediateAddress(source, ref PC);

            Assert.Equal(0xDD, memory.ReadByte(0xABBA));
            Assert.Equal(0x02, PC);
            Assert.Equal(16, cycles);
        }

        [Fact()]
        public void LoadFromAddressAndDecrementTest()
        {
            var memory = new Memory(new byte[0]);
            var loadUnit = new LoadUnit(memory);

            byte dest = 0x00;
            byte regH = 0xAB;
            byte regL = 0x00;
            memory.WriteByte(0xAB00, 0xEE);

            var cycles = loadUnit.LoadFromAddressAndDecrement(ref dest, ref regH, ref regL);

            Assert.Equal(0xEE, dest);
            Assert.Equal(0xAA, regH);
            Assert.Equal(0xFF, regL);
            Assert.Equal(8, cycles);
        }
    }
}