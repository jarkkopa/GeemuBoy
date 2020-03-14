using Xunit;

namespace GameBoy.GB.CpuUnits.Tests
{
    public class LoadUnitTests
    {
        [Fact()]
        public void LoadTest()
        {
            var memory = new Memory(new byte[0]);
            var loadUnit = new LoadUnit(memory);
            byte to = 0x00;
            byte from = 0xFF;

            var cycles = loadUnit.Load(ref to, from);

            Assert.Equal(0xFF, to);
            Assert.Equal(4, cycles);
        }

        [Fact()]
        public void LoadWordTest()
        {
            var memory = new Memory(new byte[0]);
            var loadUnit = new LoadUnit(memory);
            byte high = 0x00;
            byte low = 0x00;
            ushort from = 0xABCD;

            var cycles = loadUnit.Load(ref high, ref low, from);

            Assert.Equal(0xAB, high);
            Assert.Equal(0xCD, low);
            Assert.Equal(4, cycles);
        }

        [Fact()]
        public void LoadWordToWord()
        {
            var memory = new Memory(new byte[0]);
            var loadUnit = new LoadUnit(memory);
            ushort to = 0x00;
            ushort from = 0xABCD;

            var cycles = loadUnit.Load(ref to, from);

            Assert.Equal(0xABCD, to);
            Assert.Equal(4, cycles);
        }

        [Fact()]
        public void LoadBytesToWord()
        {
            var memory = new Memory(new byte[0]);
            var loadUnit = new LoadUnit(memory);
            ushort to = 0x00;
            byte fromHigh = 0xAB;
            byte fromLow = 0xCD;

            var cycles = loadUnit.Load(ref to, fromHigh, fromLow);

            Assert.Equal(0xABCD, to);
            Assert.Equal(8, cycles);
        }

        [Fact()]
        public void LoadAdjustedPositiveTest()
        {
            var memory = new Memory(new byte[0]);
            var loadUnit = new LoadUnit(memory);
            byte toHigh = 0xF0;
            byte toLow = 0xF0;
            ushort from = 0xABFF;
            byte addValue = 0x01;

            var cycles = loadUnit.LoadAdjusted(ref toHigh, ref toLow, from, addValue);

            Assert.Equal(0xAC, toHigh);
            Assert.Equal(0x00, toLow);
            Assert.Equal(8, cycles);
            // TODO: Assert flags
        }

        [Fact()]
        public void LoadAdjustedNegativeTest()
        {
            var memory = new Memory(new byte[0]);
            var loadUnit = new LoadUnit(memory);
            byte toHigh = 0xF0;
            byte toLow = 0xF0;
            ushort from = 0xAC00;
            byte addValue = 0xFF; // signed -1

            var cycles = loadUnit.LoadAdjusted(ref toHigh, ref toLow, from, addValue);

            Assert.Equal(0xAB, toHigh);
            Assert.Equal(0xFF, toLow);
            Assert.Equal(8, cycles);
            // TODO: Assert flags
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
            byte addrHigh = 0xAB;
            byte addrLow = 0x00;
            memory.WriteByte(0xAB00, 0xEE);

            var cycles = loadUnit.LoadFromAddressAndIncrement(ref dest, ref addrHigh, ref addrLow, -1);

            Assert.Equal(0xEE, dest);
            Assert.Equal(0xAA, addrHigh);
            Assert.Equal(0xFF, addrLow);
            Assert.Equal(8, cycles);
        }

        [Fact()]
        public void LoadFromAddressAndIncrementTest()
        {
            var memory = new Memory(new byte[0]);
            var loadUnit = new LoadUnit(memory);

            byte dest = 0x00;
            byte addrHigh = 0xAA;
            byte addrLow = 0xFF;
            memory.WriteByte(0xAAFF, 0xEE);

            var cycles = loadUnit.LoadFromAddressAndIncrement(ref dest, ref addrHigh, ref addrLow, 1);

            Assert.Equal(0xEE, dest);
            Assert.Equal(0xAB, addrHigh);
            Assert.Equal(0x00, addrLow);
            Assert.Equal(8, cycles);
        }

        [Fact()]
        public void WriteToAddressAndDecrementTest()
        {
            var memory = new Memory(new byte[0]);
            var loadUnit = new LoadUnit(memory);

            byte source = 0xDD;
            byte addrHigh = 0xAB;
            byte addrLow = 0x00;
            var cycles = loadUnit.WriteToAddressAndIncrement(ref addrHigh, ref addrLow, source, -1);

            Assert.Equal(0xDD, memory.ReadByte(0xAB00));
            Assert.Equal(0xAA, addrHigh);
            Assert.Equal(0xFF, addrLow);
            Assert.Equal(8, cycles);
        }

        [Fact()]
        public void WriteToAddressAndIncrementTest()
        {
            var memory = new Memory(new byte[0]);
            var loadUnit = new LoadUnit(memory);

            byte source = 0xDD;
            byte addrHigh = 0xAA;
            byte addrLow = 0xFF;
            var cycles = loadUnit.WriteToAddressAndIncrement(ref addrHigh, ref addrLow, source, 1);

            Assert.Equal(0xDD, memory.ReadByte(0xAAFF));
            Assert.Equal(0xAB, addrHigh);
            Assert.Equal(0x00, addrLow);
            Assert.Equal(8, cycles);
        }
    }
}