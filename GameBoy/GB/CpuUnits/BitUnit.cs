namespace GameBoy.GB.CpuUnits
{
    public interface IBitUnit
    {
        public void RotateLeft(ref byte register, ref byte flags, bool alwaysResetZero);
        public void RotateLeftThroughCarry(ref byte register, ref byte flags, bool alwaysResetZero);
        public void RotateLeftThroughCarry(byte addrHigh, byte addrLow, ref byte flags);
        public void TestBit(byte register, byte index, ref byte flags);
    }

    public class BitUnit : IBitUnit
    {
        private readonly Memory memory;

        public BitUnit(Memory memory)
        {
            this.memory = memory;
        }

        public void RotateLeft(ref byte register, ref byte flags, bool alwaysResetZero)
        {
            bool setCarry = BitUtils.GetBit(register, 7);

            //register = (byte)(((register << 1) & 0xFF) | ((register & 0x80) >> 7));
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
        }

        public void RotateLeftThroughCarry(ref byte register, ref byte flags, bool alwaysResetZero)
        {
            bool setCarry = BitUtils.GetBit(register, 7);

            // register = (byte)(((register << 1) & 0xFF) | (FlagUtils.GetFlag(Flag.C, flags) ? 1 : 0));
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
        }

        public void RotateLeftThroughCarry(byte addrHigh, byte addrLow, ref byte flags)
        {
            ushort address = BitUtils.BytesToUshort(addrHigh, addrLow);
            byte data = memory.ReadByte(address);
            RotateLeftThroughCarry(ref data, ref flags, false);
            memory.WriteByte(address, data);
        }

        public void TestBit(byte register, byte index, ref byte flags)
        {
            FlagUtils.SetFlag(Flag.Z, BitUtils.GetBit(register, index) == false, ref flags);
            FlagUtils.SetFlag(Flag.N, false, ref flags);
            FlagUtils.SetFlag(Flag.H, true, ref flags);
        }

        private void RotateLeft(ref byte register, bool setLastBit)
        {
            register = (byte)(((register << 1) & 0xFF) | (setLastBit ? 1 : 0));
        }
    }
}
