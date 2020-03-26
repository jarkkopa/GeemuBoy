using GameBoy.GB.CpuUnits;
using Xunit;

namespace GameBoy.GB.CpuUnits.Tests
{
    public class JumpUnitTests
    {
        [Fact()]
        public void CallTest()
        {
            var memory = new Memory(new byte[0]);
            var jumpUnit = new JumpUnit(memory);
            ushort pc = 0x0123;
            ushort sp = 0xFFFE;

            jumpUnit.Call(0xABCD, ref sp, ref pc);

            Assert.Equal(0x23, memory.ReadByte(0xFFFC));
            Assert.Equal(0x01, memory.ReadByte(0xFFFD));
            Assert.Equal(0xABCD, pc);
        }

        [Fact()]
        public void JumpToAddressTest()
        {
            var memory = new Memory(new byte[0]);
            var jumpUnit = new JumpUnit(memory);
            ushort pc = 0x0000;

            jumpUnit.JumpToAddress(0xABCD, ref pc);

            Assert.Equal(0xABCD, pc);
        }

        [Fact()]
        public void JumpToAddressWhenSetConditionTrueTest()
        {
            var memory = new Memory(new byte[0]);
            var jumpUnit = new JumpUnit(memory);
            ushort pc = 0x0123;
            byte flags = 0b10000000;

            jumpUnit.JumpToAddressConditional(0xACDC, ref pc, Flag.Z, true, flags);

            Assert.Equal(0xACDC, pc);
        }

        [Fact()]
        public void JumpToAddressWhenResetConditionTrueTest()
        {
            var memory = new Memory(new byte[0]);
            var jumpUnit = new JumpUnit(memory);
            ushort pc = 0x0123;
            byte flags = 0b11100000;

            jumpUnit.JumpToAddressConditional(0xACDC, ref pc, Flag.C, false, flags);

            Assert.Equal(0xACDC, pc);
        }


        [Fact()]
        public void JumpToAddressWhenConditionTrueTest()
        {
            var memory = new Memory(new byte[0]);
            var jumpUnit = new JumpUnit(memory);
            ushort pc = 0x0123;
            byte flags = 0b10000000;

            jumpUnit.JumpToAddressConditional(0xACDC, ref pc, Flag.Z, true, flags);

            Assert.Equal(0xACDC, pc);
        }

        [Fact()]
        public void DoesNotJumpToAddressWhenConditionFalseTest()
        {
            var memory = new Memory(new byte[0]);
            var jumpUnit = new JumpUnit(memory);
            ushort pc = 0x0123;
            byte flags = 0b01110000;

            jumpUnit.JumpToAddressConditional(0xACDC, ref pc, Flag.Z, true, flags);

            Assert.Equal(0x0123, pc);
        }
    }
}