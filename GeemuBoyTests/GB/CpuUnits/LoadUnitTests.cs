using Xunit;

namespace GeemuBoy.GB.CpuUnits.Tests
{
    public class LoadUnitTests
    {
        [Fact()]
        public void LoadTest()
        {
            var memory = new Memory();
            var loadUnit = new LoadUnit(memory);
            byte to = 0x00;
            byte from = 0xFF;

            loadUnit.Load(ref to, from);

            Assert.Equal(0xFF, to);
        }

        [Fact()]
        public void LoadWordTest()
        {
            var memory = new Memory();
            var loadUnit = new LoadUnit(memory);
            byte high = 0x00;
            byte low = 0x00;
            ushort from = 0xABCD;

            loadUnit.Load(ref high, ref low, from);

            Assert.Equal(0xAB, high);
            Assert.Equal(0xCD, low);
        }

        [Fact()]
        public void LoadWordToWord()
        {
            var memory = new Memory();
            var loadUnit = new LoadUnit(memory);
            ushort to = 0x00;
            ushort from = 0xABCD;

            loadUnit.Load(ref to, from);

            Assert.Equal(0xABCD, to);
        }

        [Fact()]
        public void LoadBytesToWord()
        {
            var memory = new Memory();
            var loadUnit = new LoadUnit(memory);
            ushort to = 0x00;
            byte fromHigh = 0xAB;
            byte fromLow = 0xCD;

            loadUnit.Load(ref to, fromHigh, fromLow);

            Assert.Equal(0xABCD, to);
        }

        [Fact()]
        public void LoadAdjustedPositiveTest()
        {
            var memory = new Memory();
            var loadUnit = new LoadUnit(memory);
            byte H = 0xF0;
            byte L = 0xF0;
            ushort SP = 0x0000;
            byte flags = 0x0;

            loadUnit.LoadAdjusted(ref H, ref L, SP, 0x01, ref flags);

            Assert.Equal(0x0001, BitUtils.BytesToUshort(H,L));
            Assert.Equal(0x0, flags);

            SP = 0x0001;
            flags = 0x0;

            loadUnit.LoadAdjusted(ref H, ref L, SP, 0x01, ref flags);

            Assert.Equal(0x0002, BitUtils.BytesToUshort(H, L));
            Assert.Equal(0x0, flags);

            SP = 0x000F;
            flags = 0x0;

            loadUnit.LoadAdjusted(ref H, ref L, SP, 0x01, ref flags);

            Assert.Equal(0x0010, BitUtils.BytesToUshort(H, L));
            Assert.Equal(0b00100000, flags);

            SP = 0x0010;
            flags = 0x0;

            loadUnit.LoadAdjusted(ref H, ref L, SP, 0x01, ref flags);

            Assert.Equal(0x0011, BitUtils.BytesToUshort(H, L));
            Assert.Equal(0x0, flags);

            SP = 0x001F;
            flags = 0x0;

            loadUnit.LoadAdjusted(ref H, ref L, SP, 0x01, ref flags);

            Assert.Equal(0x0020, BitUtils.BytesToUshort(H, L));
            Assert.Equal(0b00100000, flags);

            SP = 0x007F;
            flags = 0x0;

            loadUnit.LoadAdjusted(ref H, ref L, SP, 0x01, ref flags);

            Assert.Equal(0x0080, BitUtils.BytesToUshort(H, L));
            Assert.Equal(0b00100000, flags);

            SP = 0x0080;
            flags = 0x0;

            loadUnit.LoadAdjusted(ref H, ref L, SP, 0x01, ref flags);

            Assert.Equal(0x0081, BitUtils.BytesToUshort(H, L));
            Assert.Equal(0x0, flags);

            SP = 0x00FF;
            flags = 0x0;

            loadUnit.LoadAdjusted(ref H, ref L, SP, 0x01, ref flags);

            Assert.Equal(0x0100, BitUtils.BytesToUshort(H, L));
            Assert.Equal(0b00110000, flags);

            SP = 0x0100;
            flags = 0x0;

            loadUnit.LoadAdjusted(ref H, ref L, SP, 0x01, ref flags);

            Assert.Equal(0x0101, BitUtils.BytesToUshort(H, L));
            Assert.Equal(0x0, flags);

            SP = 0x0F00;
            flags = 0x0;

            loadUnit.LoadAdjusted(ref H, ref L, SP, 0x01, ref flags);

            Assert.Equal(0x0F01, BitUtils.BytesToUshort(H, L));
            Assert.Equal(0x0, flags);

            SP = 0x1F00;
            flags = 0x0;

            loadUnit.LoadAdjusted(ref H, ref L, SP, 0x01, ref flags);

            Assert.Equal(0x1F01, BitUtils.BytesToUshort(H, L));
            Assert.Equal(0x0, flags);

            SP = 0x1000;
            flags = 0x0;

            loadUnit.LoadAdjusted(ref H, ref L, SP, 0x01, ref flags);

            Assert.Equal(0x1001, BitUtils.BytesToUshort(H, L));
            Assert.Equal(0x0, flags);

            SP = 0x7FFF;
            flags = 0x0;

            loadUnit.LoadAdjusted(ref H, ref L, SP, 0x01, ref flags);

            Assert.Equal(0x8000, BitUtils.BytesToUshort(H, L));
            Assert.Equal(0b00110000, flags);

            SP = 0x8000;
            flags = 0x0;

            loadUnit.LoadAdjusted(ref H, ref L, SP, 0x01, ref flags);

            Assert.Equal(0x8001, BitUtils.BytesToUshort(H, L));
            Assert.Equal(0x0, flags);

            SP = 0xFFFF;
            flags = 0x0;

            loadUnit.LoadAdjusted(ref H, ref L, SP, 0x01, ref flags);

            Assert.Equal(0x0000, BitUtils.BytesToUshort(H, L));
            Assert.Equal(0b00110000, flags);
        }

        [Fact()]
        public void LoadAdjustedNegativeTest()
        {
            var memory = new Memory();
            var loadUnit = new LoadUnit(memory);
            byte H = 0xF0;
            byte L = 0xF0;
            byte flags = 0x0;
             ushort SP = 0x0000;

            loadUnit.LoadAdjusted(ref H, ref L, SP, 0xFF, ref flags);

            Assert.Equal(0xFFFF, BitUtils.BytesToUshort(H, L));
            Assert.Equal(0x0, flags);

            SP = 0x0001;
            flags = 0x0;

            loadUnit.LoadAdjusted(ref H, ref L, SP, 0xFF, ref flags);

            Assert.Equal(0x0000, BitUtils.BytesToUshort(H, L));
            Assert.Equal(0b00110000, flags);

            SP = 0x000F;
            flags = 0x0;

            loadUnit.LoadAdjusted(ref H, ref L, SP, 0xFF, ref flags);

            Assert.Equal(0x000E, BitUtils.BytesToUshort(H, L));
            Assert.Equal(0b00110000, flags);

            SP = 0x0010;
            flags = 0x0;

            loadUnit.LoadAdjusted(ref H, ref L, SP, 0xFF, ref flags);

            Assert.Equal(0x000F, BitUtils.BytesToUshort(H, L));
            Assert.Equal(0b00010000, flags);

            SP = 0x001F;
            flags = 0x0;

            loadUnit.LoadAdjusted(ref H, ref L, SP, 0xFF, ref flags);

            Assert.Equal(0x001E, BitUtils.BytesToUshort(H, L));
            Assert.Equal(0b00110000, flags);

            SP = 0x007F;
            flags = 0x0;

            loadUnit.LoadAdjusted(ref H, ref L, SP, 0xFF, ref flags);

            Assert.Equal(0x007E, BitUtils.BytesToUshort(H, L));
            Assert.Equal(0b00110000, flags);

            SP = 0x0080;
            flags = 0x0;

            loadUnit.LoadAdjusted(ref H, ref L, SP, 0xFF, ref flags);

            Assert.Equal(0x007F, BitUtils.BytesToUshort(H, L));
            Assert.Equal(0b00010000, flags);

            SP = 0x00FF;
            flags = 0x0;

            loadUnit.LoadAdjusted(ref H, ref L, SP, 0xFF, ref flags);

            Assert.Equal(0x00FE, BitUtils.BytesToUshort(H, L));
            Assert.Equal(0b00110000, flags);

            SP = 0x0100;
            flags = 0x0;

            loadUnit.LoadAdjusted(ref H, ref L, SP, 0xFF, ref flags);

            Assert.Equal(0x00FF, BitUtils.BytesToUshort(H, L));
            Assert.Equal(0x0, flags);

            SP = 0x0F00;
            flags = 0x0;

            loadUnit.LoadAdjusted(ref H, ref L, SP, 0xFF, ref flags);

            Assert.Equal(0x0EFF, BitUtils.BytesToUshort(H, L));
            Assert.Equal(0x0, flags);

            SP = 0x1F00;
            flags = 0x0;

            loadUnit.LoadAdjusted(ref H, ref L, SP, 0xFF, ref flags);

            Assert.Equal(0x1EFF, BitUtils.BytesToUshort(H, L));
            Assert.Equal(0x0, flags);

            SP = 0x1000;
            flags = 0x0;

            loadUnit.LoadAdjusted(ref H, ref L, SP, 0xFF, ref flags);

            Assert.Equal(0x0FFF, BitUtils.BytesToUshort(H, L));
            Assert.Equal(0x0, flags);

            SP = 0x7FFF;
            flags = 0x0;

            loadUnit.LoadAdjusted(ref H, ref L, SP, 0xFF, ref flags);

            Assert.Equal(0x7FFE, BitUtils.BytesToUshort(H, L));
            Assert.Equal(0b00110000, flags);

            SP = 0x8000;
            flags = 0x0;

            loadUnit.LoadAdjusted(ref H, ref L, SP, 0xFF, ref flags);

            Assert.Equal(0x7FFF, BitUtils.BytesToUshort(H, L));
            Assert.Equal(0x0, flags);

            SP = 0xFFFF;
            flags = 0x0;

            loadUnit.LoadAdjusted(ref H, ref L, SP, 0xFF, ref flags);

            Assert.Equal(0xFFFE, BitUtils.BytesToUshort(H, L));
            Assert.Equal(0b00110000, flags);
        }

        [Fact()]
        public void LoadFromCombinedAddressTest()
        {
            var memory = new Memory();
            var loadUnit = new LoadUnit(memory);
            byte to = 0x00;
            memory.WriteByte(0xABBA, 0x10);

            loadUnit.LoadFromAddress(ref to, 0xAB, 0xBA);

            Assert.Equal(0x10, to);
        }

        [Fact()]
        public void LoadFromAddressTest()
        {
            var memory = new Memory();
            var loadUnit = new LoadUnit(memory);
            byte to = 0x00;
            memory.WriteByte(0xABBA, 0x10);

            loadUnit.LoadFromAddress(ref to, 0xAB, 0xBA);

            Assert.Equal(0x10, to);
        }

        [Fact()]
        public void WriteToCombinedAddressTest()
        {
            var memory = new Memory();
            var loadUnit = new LoadUnit(memory);
            byte source = 0x66;

            loadUnit.WriteToAddress(0xAB, 0xBA, source);

            Assert.Equal(0x66, memory.ReadByte(0xABBA));
        }

        [Fact()]
        public void WriteToAddressTest()
        {
            var memory = new Memory();
            var loadUnit = new LoadUnit(memory);
            byte source = 0x66;

            loadUnit.WriteToAddress(0xABBA, source);

            Assert.Equal(0x66, memory.ReadByte(0xABBA));
        }

        [Fact()]
        public void WriteWordToAddressTest()
        {
            var memory = new Memory();
            var loadUnit = new LoadUnit(memory);
            ushort value = 0xABCD;

            loadUnit.WriteToAddress(0xABBA, value);

            Assert.Equal(0xCD, memory.ReadByte(0xABBA));
            Assert.Equal(0xAB, memory.ReadByte(0xABBB));
        }

        [Fact()]
        public void LoadFromAddressAndDecrementTest()
        {
            var memory = new Memory();
            var loadUnit = new LoadUnit(memory);

            byte dest = 0x00;
            byte addrHigh = 0xAB;
            byte addrLow = 0x00;
            memory.WriteByte(0xAB00, 0xEE);

            loadUnit.LoadFromAddressAndIncrement(ref dest, ref addrHigh, ref addrLow, -1);

            Assert.Equal(0xEE, dest);
            Assert.Equal(0xAA, addrHigh);
            Assert.Equal(0xFF, addrLow);
        }

        [Fact()]
        public void LoadFromAddressAndIncrementTest()
        {
            var memory = new Memory();
            var loadUnit = new LoadUnit(memory);

            byte dest = 0x00;
            byte addrHigh = 0xAA;
            byte addrLow = 0xFF;
            memory.WriteByte(0xAAFF, 0xEE);

            loadUnit.LoadFromAddressAndIncrement(ref dest, ref addrHigh, ref addrLow, 1);

            Assert.Equal(0xEE, dest);
            Assert.Equal(0xAB, addrHigh);
            Assert.Equal(0x00, addrLow);
        }

        [Fact()]
        public void WriteToAddressAndDecrementTest()
        {
            var memory = new Memory();
            var loadUnit = new LoadUnit(memory);

            byte source = 0xDD;
            byte addrHigh = 0xAB;
            byte addrLow = 0x00;
            loadUnit.WriteToAddressAndIncrement(ref addrHigh, ref addrLow, source, -1);

            Assert.Equal(0xDD, memory.ReadByte(0xAB00));
            Assert.Equal(0xAA, addrHigh);
            Assert.Equal(0xFF, addrLow);
        }

        [Fact()]
        public void WriteToAddressAndIncrementTest()
        {
            var memory = new Memory();
            var loadUnit = new LoadUnit(memory);

            byte source = 0xDD;
            byte addrHigh = 0xAA;
            byte addrLow = 0xFF;
            loadUnit.WriteToAddressAndIncrement(ref addrHigh, ref addrLow, source, 1);

            Assert.Equal(0xDD, memory.ReadByte(0xAAFF));
            Assert.Equal(0xAB, addrHigh);
            Assert.Equal(0x00, addrLow);
        }

        [Fact()]
        public void PushTest()
        {
            var memory = new Memory();
            var loadUnit = new LoadUnit(memory);
            ushort pointer = 0xDFFF;
            byte valueHigh = 0xAB;
            byte valueLow = 0xCD;

            loadUnit.Push(ref pointer, valueHigh, valueLow);

            Assert.Equal(0xABCD, memory.ReadWord(pointer));
            Assert.Equal(0xDFFD, pointer);
        }

        [Fact()]
        public void PopTest()
        {
            var memory = new Memory();
            var loadUnit = new LoadUnit(memory);
            byte destHigh = 0x00;
            byte destLow = 0x00;
            ushort value = 0xC000;
            memory.WriteWord(0xC000, 0x1234);

            loadUnit.Pop(ref destHigh, ref destLow, ref value);

            Assert.Equal(0x12, destHigh);
            Assert.Equal(0x34, destLow);
            Assert.Equal(0xC002, value);
        }
    }
}