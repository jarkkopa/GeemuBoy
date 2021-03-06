﻿using Xunit;

namespace GeemuBoy.GB.CpuUnits.Tests
{
    public class JumpUnitTests
    {
        [Fact()]
        public void CallTest()
        {
            var memory = new Memory();
            var jumpUnit = new JumpUnit(memory);
            ushort pc = 0x0123;
            ushort sp = 0xFFFE;

            jumpUnit.Call(0xABCD, ref sp, ref pc);

            Assert.Equal(0x23, memory.ReadByte(0xFFFC));
            Assert.Equal(0x01, memory.ReadByte(0xFFFD));
            Assert.Equal(0xABCD, pc);
        }

        [Fact()]
        public void CallWhenSetConditionTrueTest()
        {
            var memory = new Memory();
            var jumpUnit = new JumpUnit(memory);
            ushort pc = 0x0123;
            ushort sp = 0xFFFE;
            byte flags = 0b10000000;

            jumpUnit.CallConditional(0xACDC, ref sp, ref pc, Flag.Z, true, flags);

            Assert.Equal(0x23, memory.ReadByte(0xFFFC));
            Assert.Equal(0x01, memory.ReadByte(0xFFFD));
            Assert.Equal(0xACDC, pc);
        }

        [Fact()]
        public void CallWhenResetConditionTrueTest()
        {
            var memory = new Memory();
            var jumpUnit = new JumpUnit(memory);
            ushort pc = 0x0123;
            ushort sp = 0xFFFE;
            byte flags = 0b01110000;

            jumpUnit.CallConditional(0xACDC, ref sp, ref pc, Flag.Z, false, flags);

            Assert.Equal(0x23, memory.ReadByte(0xFFFC));
            Assert.Equal(0x01, memory.ReadByte(0xFFFD));
            Assert.Equal(0xACDC, pc);
        }

        [Fact()]
        public void DoesNotCallWhenConditionFalseTest()
        {
            var memory = new Memory();
            var jumpUnit = new JumpUnit(memory);
            ushort pc = 0x0123;
            ushort sp = 0xFFFE;
            byte flags = 0b00000000;

            jumpUnit.CallConditional(0xACDC, ref sp, ref pc, Flag.Z, true, flags);

            Assert.Equal(0xFFFE, sp);
            Assert.Equal(0x0123, pc);
        }

        [Fact()]
        public void JumpToAddressTest()
        {
            var memory = new Memory();
            var jumpUnit = new JumpUnit(memory);
            ushort pc = 0x0000;

            jumpUnit.JumpToAddress(0xABCD, ref pc);

            Assert.Equal(0xABCD, pc);
        }

        [Fact()]
        public void JumpToAddressWhenSetConditionTrueTest()
        {
            var memory = new Memory();
            var jumpUnit = new JumpUnit(memory);
            ushort pc = 0x0123;
            byte flags = 0b10000000;

            jumpUnit.JumpToAddressConditional(0xACDC, ref pc, Flag.Z, true, flags);

            Assert.Equal(0xACDC, pc);
        }

        [Fact()]
        public void JumpToAddressWhenResetConditionTrueTest()
        {
            var memory = new Memory();
            var jumpUnit = new JumpUnit(memory);
            ushort pc = 0x0123;
            byte flags = 0b11100000;

            jumpUnit.JumpToAddressConditional(0xACDC, ref pc, Flag.C, false, flags);

            Assert.Equal(0xACDC, pc);
        }


        [Fact()]
        public void JumpToAddressWhenConditionTrueTest()
        {
            var memory = new Memory();
            var jumpUnit = new JumpUnit(memory);
            ushort pc = 0x0123;
            byte flags = 0b10000000;

            jumpUnit.JumpToAddressConditional(0xACDC, ref pc, Flag.Z, true, flags);

            Assert.Equal(0xACDC, pc);
        }

        [Fact()]
        public void DoesNotJumpToAddressWhenConditionFalseTest()
        {
            var memory = new Memory();
            var jumpUnit = new JumpUnit(memory);
            ushort pc = 0x0123;
            byte flags = 0b01110000;

            jumpUnit.JumpToAddressConditional(0xACDC, ref pc, Flag.Z, true, flags);

            Assert.Equal(0x0123, pc);
        }

        [Fact()]
        public void JumpRelativeNegativeTest()
        {
            var memory = new Memory();
            var jumpUnit = new JumpUnit(memory);
            ushort pc = 0x0A0B;

            jumpUnit.JumpRelative(0xE7, ref pc);

            Assert.Equal(0x9F2, pc);
        }

        [Fact()]
        public void JumpRelativePositiveTest()
        {
            var memory = new Memory();
            var jumpUnit = new JumpUnit(memory);
            ushort pc = 0x0A0B;

            jumpUnit.JumpRelative(0x78, ref pc);

            Assert.Equal(0xA83, pc);
        }

        [Fact()]
        public void JumpRelativeWhenSetConditionTrueTest()
        {
            var memory = new Memory();
            var jumpUnit = new JumpUnit(memory);
            ushort pc = 0x0123;
            byte flags = 0b10000000;

            jumpUnit.JumpRelativeConditional(0x02, ref pc, Flag.Z, true, flags);

            Assert.Equal(0x0125, pc);
        }

        [Fact()]
        public void JumpRelativeWhenResetConditionTrueTest()
        {
            var memory = new Memory();
            var jumpUnit = new JumpUnit(memory);
            ushort pc = 0x0123;
            byte flags = 0b01110000;

            jumpUnit.JumpRelativeConditional(0xFF, ref pc, Flag.Z, false, flags);

            Assert.Equal(0x0122, pc);
        }

        [Fact()]
        public void DontJumpRelativeWhenSetConditionFalseTest()
        {
            var memory = new Memory();
            var jumpUnit = new JumpUnit(memory);
            ushort pc = 0x0123;
            byte flags = 0b00000000;

            jumpUnit.JumpRelativeConditional(0x02, ref pc, Flag.Z, true, flags);

            Assert.Equal(0x0123, pc);
        }

        [Fact()]
        public void JumpToAddressCombinedTest()
        {
            var memory = new Memory();
            var jumpUnit = new JumpUnit(memory);
            ushort pc = 0x0000;
            byte high = 0xAB;
            byte low = 0xBA;

            jumpUnit.JumpToAddress(high, low, ref pc);

            Assert.Equal(0xABBA, pc);
        }

        [Fact()]
        public void ReturnTest()
        {
            var memory = new Memory();
            var jumpUnit = new JumpUnit(memory);
            ushort pc = 0x0100;
            ushort sp = 0xFFFA;
            memory.WriteByte(0xFFFA, 0x23);
            memory.WriteByte(0xFFFB, 0x01);

            jumpUnit.Return(ref sp, ref pc);

            Assert.Equal(0x0123, pc);
            Assert.Equal(0xFFFC, sp);
        }

        [Fact()]
        public void ReturnWhenSetConditionTrue()
        {
            var memory = new Memory();
            var jumpUnit = new JumpUnit(memory);
            ushort pc = 0x0100;
            ushort sp = 0xFFFA;
            byte flags = 0b10000000;
            memory.WriteByte(0xFFFA, 0x23);
            memory.WriteByte(0xFFFB, 0x01);

            jumpUnit.ReturnConditional(ref sp, ref pc, Flag.Z, true, flags);

            Assert.Equal(0x0123, pc);
            Assert.Equal(0xFFFC, sp);
        }

        [Fact()]
        public void ReturnWhenResetConditionTrue()
        {
            var memory = new Memory();
            var jumpUnit = new JumpUnit(memory);
            ushort pc = 0x0100;
            ushort sp = 0xFFFA;
            byte flags = 0b11100000;
            memory.WriteByte(0xFFFA, 0x23);
            memory.WriteByte(0xFFFB, 0x01);

            jumpUnit.ReturnConditional(ref sp, ref pc, Flag.C, false, flags);

            Assert.Equal(0x0123, pc);
            Assert.Equal(0xFFFC, sp);
        }

        [Fact()]
        public void DontReturnWhenSetConditionFalse()
        {
            var memory = new Memory();
            var jumpUnit = new JumpUnit(memory);
            ushort pc = 0x0100;
            ushort sp = 0xFFFA;
            byte flags = 0b10000000;
            memory.WriteByte(0xFFFA, 0x23);
            memory.WriteByte(0xFFFB, 0x01);

            jumpUnit.ReturnConditional(ref sp, ref pc, Flag.Z, false, flags);

            Assert.Equal(0x0100, pc);
            Assert.Equal(0xFFFA, sp);
        }

        [Fact()]
        public void ReturnAndEnableInterruptsTest()
        {
            var memory = new Memory();
            var jumpUnit = new JumpUnit(memory);
            ushort pc = 0x0100;
            ushort sp = 0xFFFA;
            memory.WriteByte(0xFFFA, 0x23);
            memory.WriteByte(0xFFFB, 0x01);
            int enableAfter = -1;

            jumpUnit.ReturnAndEnableInterrupts(ref sp, ref pc, ref enableAfter);

            Assert.Equal(0x0123, pc);
            Assert.Equal(0xFFFC, sp);
            Assert.Equal(0, enableAfter);
        }
    }
}