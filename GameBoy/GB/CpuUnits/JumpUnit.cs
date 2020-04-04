namespace GameBoy.GB.CpuUnits
{
    public interface IJumpUnit
    {
        public int Call(ushort address, ref ushort sp, ref ushort pc);
        public int CallConditional(ushort address, ref ushort sp, ref ushort pc, Flag flag, bool condition, byte flags);
        public int JumpToAddress(ushort address, ref ushort pc);
        public int JumpToAddress(byte addrHigh, byte addrLow, ref ushort pc);
        public int JumpToAddressConditional(ushort address, ref ushort pc, Flag flag, bool condition, byte flags);
        public int JumpRelative(byte value, ref ushort pc);
        public int JumpRelativeConditional(byte value, ref ushort pc, Flag flag, bool condition, byte flags);
        public int Return(ref ushort sp, ref ushort pc);
    }

    public class JumpUnit : IJumpUnit
    {
        private readonly Memory memory;

        public JumpUnit(Memory memory)
        {
            this.memory = memory;
        }

        public int Call(ushort address, ref ushort sp, ref ushort pc)
        {
            sp = (ushort)(sp - 2);
            memory.WriteWord(sp, pc);
            pc = address;
            return 24;
        }

        public int CallConditional(ushort address, ref ushort sp, ref ushort pc, Flag flag, bool condition, byte flags)
        {
            if(FlagUtils.GetFlag(flag, flags) == condition)
            {
                Call(address, ref sp, ref pc);
                return 24;
            }
            return 12;
        }

        public int JumpRelative(byte value, ref ushort pc)
        {
            sbyte signed = unchecked((sbyte)value);
            pc = (ushort)(pc + signed);
            return 12;
        }

        public int JumpRelativeConditional(byte value, ref ushort pc, Flag flag, bool condition, byte flags)
        {
            if (FlagUtils.GetFlag(flag, flags) == condition)
            {
                JumpRelative(value, ref pc);
                return 12;
            }
            return 8;
        }

        public int JumpToAddress(ushort address, ref ushort pc)
        {
            pc = address;
            return 16;
        }

        public int JumpToAddress(byte addrHigh, byte addrLow, ref ushort pc)
        {
            ushort address = BitUtils.BytesToUshort(addrHigh, addrLow);
            JumpToAddress(address, ref pc);
            return 4;
        }

        public int JumpToAddressConditional(ushort address, ref ushort pc, Flag flag, bool condition, byte flags)
        {
            if (FlagUtils.GetFlag(flag, flags) == condition)
            {
                JumpToAddress(address, ref pc);
                return 16;
            }
            return 12;
        }

        public int Return(ref ushort sp, ref ushort pc)
        {
            pc = memory.ReadWord(sp);
            sp = (ushort)(sp + 2);
            return 16;
        }
    }
}
