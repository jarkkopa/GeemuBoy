using Xunit;

namespace GeemuBoy.GB.CpuUnits.Tests
{
    public class MiscUnitTests
    {
        [Fact()]
        public void SetCarryTest()
        {
            var miscUnit = new MiscUnit(new Memory());
            byte flags = 0b11100000;

            miscUnit.SetCarry(ref flags);

            Assert.Equal(0b10010000, flags);
        }

        [Fact()]
        public void DecimalAdjustTest()
        {
            var miscUnit = new MiscUnit(new Memory());
            byte flags = 0;

            byte value = 0x01;
            miscUnit.DecimalAdjust(ref value, ref flags);
            Assert.Equal(0x1, value);

            value = 0x0A;
            miscUnit.DecimalAdjust(ref value, ref flags);
            Assert.Equal(0x10, value);

            value = 0x0F;
            miscUnit.DecimalAdjust(ref value, ref flags);
            Assert.Equal(0x15, value);

            flags = 0b01100000;
            value = 0x1F;
            miscUnit.DecimalAdjust(ref value, ref flags);
            Assert.Equal(0x19, value);
        }

        [Fact()]
        public void HaltTest()
        {
            var memory = new Memory();
            var miscUnit = new MiscUnit(memory);
            var mode = CPU.PowerMode.Normal;

            miscUnit.Halt(ref mode, true);
            Assert.Equal(CPU.PowerMode.Halt, mode);

            // Interrupt pending causes halt bug
            memory.WriteByte(0xFFFF, 0x1);
            memory.WriteByte(0xFF0F, 0x1);
            miscUnit.Halt(ref mode, false);
            Assert.Equal(CPU.PowerMode.HaltBug, mode);

            memory.WriteByte(0xFFFF, 0x2);
            memory.WriteByte(0xFF0F, 0x1);
            miscUnit.Halt(ref mode, false);
            Assert.Equal(CPU.PowerMode.Halt, mode);
        }
    }
}