using System.Runtime.CompilerServices;

namespace GeemuBoy.GB.CpuUnits
{
    public class MiscUnit
    {
        private readonly Memory memory;

        public MiscUnit(Memory memory)
        {
            this.memory = memory;
        }

        public void SetCarry(ref byte flags)
        {
            FlagUtils.SetFlag(Flag.C, true, ref flags);
            FlagUtils.SetFlag(Flag.N, false, ref flags);
            FlagUtils.SetFlag(Flag.H, false, ref flags);
        }

        public void DecimalAdjust(ref byte register, ref byte flags)
        {
            bool subtract = FlagUtils.GetFlag(Flag.N, flags);
            bool carry = FlagUtils.GetFlag(Flag.C, flags);
            bool halfCarry = FlagUtils.GetFlag(Flag.H, flags);

            if (subtract)
            {
                if (carry)
                {
                    register -= 0x60;
                }
                if (halfCarry)
                {
                    register -= 0x6;
                }
            }
            else
            {
                if (register > 0x99 || carry)
                {
                    register += 0x60;
                    FlagUtils.SetFlag(Flag.C, true, ref flags);
                }
                if ((register & 0xF) > 0x9 || halfCarry)
                {
                    register += 0x6;
                }
            }
            FlagUtils.SetFlag(Flag.Z, register == 0x00, ref flags);
            FlagUtils.SetFlag(Flag.H, false, ref flags);
        }

        public void Halt(ref CPU.PowerMode mode, bool ime)
        {
            if (ime)
            {
                mode = CPU.PowerMode.Halt;
            }
            else
            {
                // Halt only takes four ticks even though it technically reads IF and IE from memory
                byte enabledInterrupts = memory.ReadByte(0xFFFF);
                byte requestedInterrupts = memory.ReadByte(0xFF0F);
                if ((enabledInterrupts & requestedInterrupts & 0x1F) == 0x0)
                {
                    mode = CPU.PowerMode.Halt;
                }
                else
                {
                    mode = CPU.PowerMode.HaltBug;
                }
            }
        }
    }
}
