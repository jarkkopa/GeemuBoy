﻿using Xunit;

namespace GeemuBoy.GB.CpuUnits.Tests
{
    public class ALUTests
    {
        [Fact()]
        public void AddSetsHalfCarryTest()
        {
            var memory = new Memory();
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
            var memory = new Memory();
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
            var memory = new Memory();
            var alu = new ALU(memory);
            byte flags = 0x0;
            byte to = 0x0;

            alu.Add(ref to, 0x0, ref flags);

            Assert.Equal(0x0, to);
            Assert.Equal(0b10000000, flags);
        }

        [Fact()]
        public void AddSetsZeroWhenSumCarriesTest()
        {
            var memory = new Memory();
            var alu = new ALU(memory);
            byte flags = 0x0;
            byte to = 0xF5;

            alu.Add(ref to, 0x0B, ref flags);

            Assert.Equal(0x0, to);
            Assert.Equal(0b10110000, flags);
        }

        [Fact()]
        public void AddWithCarrySetsHalfCarryTest()
        {
            var memory = new Memory();
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
            var memory = new Memory();
            var alu = new ALU(memory);
            byte flags = 0b11010000;
            byte to = 0xFE;

            alu.Add(ref to, 0x03, ref flags, true);

            Assert.Equal(0x02, to);
            Assert.Equal(0b00110000, flags);
        }


        [Fact()]
        public void AddWordSetsCarryTest()
        {
            var memory = new Memory();
            var alu = new ALU(memory);
            byte flags = 0b00000000;
            byte toHigh = 0xFF;
            byte toLow = 0xFF;
            byte valueHigh = 0x10;
            byte valueLow = 0x00;

            alu.Add(ref toHigh, ref toLow, valueHigh, valueLow, ref flags);

            Assert.Equal(0x0F, toHigh);
            Assert.Equal(0xFF, toLow);
            Assert.Equal(0b00010000, flags);
        }

        [Fact()]
        public void AddWordSetsHalfCarryTest()
        {
            var memory = new Memory();
            var alu = new ALU(memory);
            byte flags = 0b00000000;
            byte toHigh = 0x0F;
            byte toLow = 0x12;
            byte valueHigh = 0x01;
            byte valueLow = 0x11;

            alu.Add(ref toHigh, ref toLow, valueHigh, valueLow, ref flags);

            Assert.Equal(0x10, toHigh);
            Assert.Equal(0x23, toLow);
            Assert.Equal(0b00100000, flags);
        }

        [Fact()]
        public void AddSignedPositiveTest()
        {
            var memory = new Memory();
            var alu = new ALU(memory);
            ushort to = 0x0000;
            byte flags = 0x0;

            alu.AddSigned(ref to, 0x01, ref flags);

            Assert.Equal(0x0001, to);
            Assert.Equal(0x0, flags);

            to = 0x0001;
            flags = 0x0;

            alu.AddSigned(ref to, 0x01, ref flags);

            Assert.Equal(0x0002, to);
            Assert.Equal(0x0, flags);

            to = 0x000F;
            flags = 0x0;

            alu.AddSigned(ref to, 0x01, ref flags);

            Assert.Equal(0x0010, to);
            Assert.Equal(0b00100000, flags);

            to = 0x0010;
            flags = 0x0;

            alu.AddSigned(ref to, 0x01, ref flags);

            Assert.Equal(0x0011, to);
            Assert.Equal(0x0, flags);

            to = 0x001F;
            flags = 0x0;

            alu.AddSigned(ref to, 0x01, ref flags);

            Assert.Equal(0x0020, to);
            Assert.Equal(0b00100000, flags);

            to = 0x007F;
            flags = 0x0;

            alu.AddSigned(ref to, 0x01, ref flags);

            Assert.Equal(0x0080, to);
            Assert.Equal(0b00100000, flags);

            to = 0x0080;
            flags = 0x0;

            alu.AddSigned(ref to, 0x01, ref flags);

            Assert.Equal(0x0081, to);
            Assert.Equal(0x0, flags);

            to = 0x00FF;
            flags = 0x0;

            alu.AddSigned(ref to, 0x01, ref flags);

            Assert.Equal(0x0100, to);
            Assert.Equal(0b00110000, flags);

            to = 0x0100;
            flags = 0x0;

            alu.AddSigned(ref to, 0x01, ref flags);

            Assert.Equal(0x0101, to);
            Assert.Equal(0x0, flags);

            to = 0x0F00;
            flags = 0x0;

            alu.AddSigned(ref to, 0x01, ref flags);

            Assert.Equal(0x0F01, to);
            Assert.Equal(0x0, flags);

            to = 0x1F00;
            flags = 0x0;

            alu.AddSigned(ref to, 0x01, ref flags);

            Assert.Equal(0x1F01, to);
            Assert.Equal(0x0, flags);

            to = 0x1000;
            flags = 0x0;

            alu.AddSigned(ref to, 0x01, ref flags);

            Assert.Equal(0x1001, to);
            Assert.Equal(0x0, flags);

            to = 0x7FFF;
            flags = 0x0;

            alu.AddSigned(ref to, 0x01, ref flags);

            Assert.Equal(0x8000, to);
            Assert.Equal(0b00110000, flags);

            to = 0x8000;
            flags = 0x0;

            alu.AddSigned(ref to, 0x01, ref flags);

            Assert.Equal(0x8001, to);
            Assert.Equal(0x0, flags);

            to = 0xFFFF;
            flags = 0x0;

            alu.AddSigned(ref to, 0x01, ref flags);

            Assert.Equal(0x0000, to);
            Assert.Equal(0b00110000, flags);
        }

        [Fact()]
        public void AddSignedNegativeTest()
        {
            var memory = new Memory();
            var alu = new ALU(memory);
            ushort to = 0x0000;
            byte flags = 0x0;

            alu.AddSigned(ref to, 0xFF, ref flags);

            Assert.Equal(0xFFFF, to);
            Assert.Equal(0x0, flags);

            to = 0x0001;
            flags = 0x0;

            alu.AddSigned(ref to, 0xFF, ref flags);

            Assert.Equal(0x0000, to);
            Assert.Equal(0b00110000, flags);

            to = 0x000F;
            flags = 0x0;

            alu.AddSigned(ref to, 0xFF, ref flags);

            Assert.Equal(0x000E, to);
            Assert.Equal(0b00110000, flags);

            to = 0x0010;
            flags = 0x0;

            alu.AddSigned(ref to, 0xFF, ref flags);

            Assert.Equal(0x000F, to);
            Assert.Equal(0b00010000, flags);

            to = 0x001F;
            flags = 0x0;

            alu.AddSigned(ref to, 0xFF, ref flags);

            Assert.Equal(0x001E, to);
            Assert.Equal(0b00110000, flags);

            to = 0x007F;
            flags = 0x0;

            alu.AddSigned(ref to, 0xFF, ref flags);

            Assert.Equal(0x007E, to);
            Assert.Equal(0b00110000, flags);

            to = 0x0080;
            flags = 0x0;

            alu.AddSigned(ref to, 0xFF, ref flags);

            Assert.Equal(0x007F, to);
            Assert.Equal(0b00010000, flags);

            to = 0x00FF;
            flags = 0x0;

            alu.AddSigned(ref to, 0xFF, ref flags);

            Assert.Equal(0x00FE, to);
            Assert.Equal(0b00110000, flags);

            to = 0x0100;
            flags = 0x0;

            alu.AddSigned(ref to, 0xFF, ref flags);

            Assert.Equal(0x00FF, to);
            Assert.Equal(0x0, flags);

            to = 0x0F00;
            flags = 0x0;

            alu.AddSigned(ref to, 0xFF, ref flags);

            Assert.Equal(0x0EFF, to);
            Assert.Equal(0x0, flags);

            to = 0x1F00;
            flags = 0x0;

            alu.AddSigned(ref to, 0xFF, ref flags);

            Assert.Equal(0x1EFF, to);
            Assert.Equal(0x0, flags);

            to = 0x1000;
            flags = 0x0;

            alu.AddSigned(ref to, 0xFF, ref flags);

            Assert.Equal(0x0FFF, to);
            Assert.Equal(0x0, flags);

            to = 0x7FFF;
            flags = 0x0;

            alu.AddSigned(ref to, 0xFF, ref flags);

            Assert.Equal(0x7FFE, to);
            Assert.Equal(0b00110000, flags);

            to = 0x8000;
            flags = 0x0;

            alu.AddSigned(ref to, 0xFF, ref flags);

            Assert.Equal(0x7FFF, to);
            Assert.Equal(0x0, flags);

            to = 0xFFFF;
            flags = 0x0;

            alu.AddSigned(ref to, 0xFF, ref flags);

            Assert.Equal(0xFFFE, to);
            Assert.Equal(0b00110000, flags);
        }

        [Fact()]
        public void SubtractTest()
        {
            var memory = new Memory();
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
            var memory = new Memory();
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
            var memory = new Memory();
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
            var memory = new Memory();
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
            var memory = new Memory();
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
            var memory = new Memory();
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
            var memory = new Memory();
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
            var memory = new Memory();
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
            var memory = new Memory();
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
            var memory = new Memory();
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
            var memory = new Memory();
            var alu = new ALU(memory);
            byte to = 0xAF;
            byte flags = 0b11110000;

            alu.Xor(ref to, 0xF3, ref flags);

            Assert.Equal(0x5C, to);
            Assert.Equal(0x00, flags);
        }

        [Fact()]
        public void CompareSetsZeroFlagTest()
        {
            var memory = new Memory();
            var alu = new ALU(memory);
            byte flags = 0b00000000;

            alu.Compare(0xFF, 0xFF, ref flags);

            Assert.Equal(0b11000000, flags);
        }

        [Fact()]
        public void CompareSetsHalfCarryFlagTest()
        {
            var memory = new Memory();
            var alu = new ALU(memory);
            byte flags = 0b00000000;

            alu.Compare(0xA2, 0x03, ref flags);

            Assert.Equal(0b01100000, flags);
        }

        [Fact()]
        public void CompareSetsCarryFlagTest()
        {
            var memory = new Memory();
            var alu = new ALU(memory);
            byte flags = 0b00000000;

            alu.Compare(0x12, 0x21, ref flags);

            Assert.Equal(0b01010000, flags);
        }

        [Fact()]
        public void IncrementSetsZeroTest()
        {
            var memory = new Memory();
            var alu = new ALU(memory);
            byte value = 0xFF;
            byte flags = 0b01010000;

            alu.Increment(ref value, ref flags);

            Assert.Equal(0, value);
            Assert.Equal(0b10110000, flags);
        }

        [Fact()]
        public void IncrementSetsHalfCarryTest()
        {
            var memory = new Memory();
            var alu = new ALU(memory);
            byte value = 0x0F;
            byte flags = 0b11000000;

            alu.Increment(ref value, ref flags);

            Assert.Equal(0x10, value);
            Assert.Equal(0b00100000, flags);
        }

        [Fact()]
        public void IncrementInMemoryTest()
        {
            var memory = new Memory();
            var alu = new ALU(memory);
            byte flags = 0b00000000;
            memory.WriteByte(0xACDC, 0xFF);

            alu.IncrementInMemory(0xAC, 0xDC, ref flags);

            Assert.Equal(0, memory.ReadByte(0xACDC));
            Assert.Equal(0b10100000, flags);
        }

        [Fact()]
        public void IncrementCombinedWordTest()
        {
            var memory = new Memory();
            var alu = new ALU(memory);
            byte high = 0x10;
            byte low = 0xFF;

            alu.IncrementWord(ref high, ref low);

            Assert.Equal(0x11, high);
            Assert.Equal(0x00, low);
        }

        [Fact()]
        public void IncrementWordTest()
        {
            var memory = new Memory();
            var alu = new ALU(memory);
            ushort value = 0x0123;

            alu.IncrementWord(ref value);

            Assert.Equal(0x0124, value);
        }

        [Fact()]
        public void DecrementSetsZeroTest()
        {
            var memory = new Memory();
            var alu = new ALU(memory);
            byte value = 0x01;
            byte flags = 0b00010000;

            alu.Decrement(ref value, ref flags);

            Assert.Equal(0, value);
            Assert.Equal(0b11010000, flags);
        }

        [Fact()]
        public void DecrementSetsHalfCarryTest()
        {
            var memory = new Memory();
            var alu = new ALU(memory);
            byte value = 0x10;
            byte flags = 0b11000000;

            alu.Decrement(ref value, ref flags);

            Assert.Equal(0x0F, value);
            Assert.Equal(0b01100000, flags);
        }

        [Fact()]
        public void DecrementInMemoryTest()
        {
            var memory = new Memory();
            var alu = new ALU(memory);
            byte flags = 0b00000000;
            memory.WriteByte(0xACDC, 0xF0);

            alu.DecrementInMemory(0xAC, 0xDC, ref flags);

            Assert.Equal(0xEF, memory.ReadByte(0xACDC));
            Assert.Equal(0b01100000, flags);
        }

        [Fact()]
        public void DecrementCombinedWordTest()
        {
            var memory = new Memory();
            var alu = new ALU(memory);
            byte high = 0x01;
            byte low = 0x23;

            alu.DecrementWord(ref high, ref low);

            Assert.Equal(0x01, high);
            Assert.Equal(0x22, low);
        }

        [Fact()]
        public void DecrementWordTest()
        {
            var memory = new Memory();
            var alu = new ALU(memory);
            ushort target = 0xFFFF;

            alu.DecrementWord(ref target);

            Assert.Equal(0xFFFE, target);
        }
    }
}