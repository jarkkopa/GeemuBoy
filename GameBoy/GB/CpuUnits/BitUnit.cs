namespace GeemuBoy.GB.CpuUnits
{
    public interface IBitUnit
    {
        public int RotateLeft(ref byte register, ref byte flags, bool alwaysResetZero);
        public int RotateLeft(byte addrHigh, byte addrLow, ref byte flags);
        public int RotateLeftThroughCarry(ref byte register, ref byte flags, bool alwaysResetZero);
        public int RotateLeftThroughCarry(byte addrHigh, byte addrLow, ref byte flags);
        public int TestBit(byte register, byte index, ref byte flags);
    }

    public class BitUnit : IBitUnit
    {
        private readonly Memory memory;

        public BitUnit(Memory memory)
        {
            this.memory = memory;
        }

        public int RotateLeft(ref byte register, ref byte flags, bool alwaysResetZero)
        {
            bool setCarry = register.IsBitSet(7);

            RotateLeft(ref register, (register & 0x80) > 0);

            if (alwaysResetZero)
            {
                FlagUtils.SetFlag(Flag.Z, false, ref flags);
            }
            else
            {
                FlagUtils.SetFlag(Flag.Z, register == 0, ref flags);
            }
            FlagUtils.SetFlag(Flag.N, false, ref flags);
            FlagUtils.SetFlag(Flag.H, false, ref flags);
            FlagUtils.SetFlag(Flag.C, setCarry, ref flags);
            return 12;
        }

        public int RotateLeft(byte addrHigh, byte addrLow, ref byte flags)
        {
            ushort address = BitUtils.BytesToUshort(addrHigh, addrLow);
            byte data = memory.ReadByte(address);
            RotateLeft(ref data, ref flags, false);
            memory.WriteByte(address, data);
            return 20;
        }

        public int RotateLeftThroughCarry(ref byte register, ref byte flags, bool alwaysResetZero)
        {
            bool setCarry = register.IsBitSet(7);

            RotateLeft(ref register, FlagUtils.GetFlag(Flag.C, flags));
            if (alwaysResetZero)
            {
                FlagUtils.SetFlag(Flag.Z, false, ref flags);
            }
            else
            {
                FlagUtils.SetFlag(Flag.Z, register == 0, ref flags);
            }
            FlagUtils.SetFlag(Flag.N, false, ref flags);
            FlagUtils.SetFlag(Flag.H, false, ref flags);
            FlagUtils.SetFlag(Flag.C, setCarry, ref flags);
            return 12;
        }

        public int RotateLeftThroughCarry(byte addrHigh, byte addrLow, ref byte flags)
        {
            ushort address = BitUtils.BytesToUshort(addrHigh, addrLow);
            byte data = memory.ReadByte(address);
            RotateLeftThroughCarry(ref data, ref flags, false);
            memory.WriteByte(address, data);
            return 20;
        }

        public int TestBit(byte register, byte index, ref byte flags)
        {
            FlagUtils.SetFlag(Flag.Z, register.IsBitSet(index) == false, ref flags);
            FlagUtils.SetFlag(Flag.N, false, ref flags);
            FlagUtils.SetFlag(Flag.H, true, ref flags);
            return 12;
        }

        private void RotateLeft(ref byte register, bool setLastBit)
        {
            register = (byte)(((register << 1) & 0xFF) | (setLastBit ? 1 : 0));
        }
    }
}
