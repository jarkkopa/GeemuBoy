namespace GeemuBoy.GB.CpuUnits
{
    public interface IBitUnit
    {
        public int Complement(ref byte value, ref byte flags);
        public int RotateLeft(ref byte register, ref byte flags, bool alwaysResetZero);
        public int RotateLeft(byte addrHigh, byte addrLow, ref byte flags);
        public int RotateLeftThroughCarry(ref byte register, ref byte flags, bool alwaysResetZero);
        public int RotateLeftThroughCarry(byte addrHigh, byte addrLow, ref byte flags);
        public int SetBit(ref byte target, int index, bool bit);
        public int SetBit(byte addrHigh, byte addrLow, int index, bool bit);
        public int ShiftRight(ref byte target, ref byte flags);
        public int ShiftRight(byte addrHigh, byte addrLow, ref byte flags);
        public int Swap(ref byte register, ref byte flags);
        public int Swap(byte addrHigh, byte addrLow, ref byte flags);
        public int TestBit(byte register, int index, ref byte flags);
    }

    public class BitUnit : IBitUnit
    {
        private readonly Memory memory;

        public BitUnit(Memory memory)
        {
            this.memory = memory;
        }

        public int Complement(ref byte value, ref byte flags)
        {
            value = (byte)~value;
            FlagUtils.SetFlag(Flag.N, true, ref flags);
            FlagUtils.SetFlag(Flag.H, true, ref flags);
            return 4;
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

        public int SetBit(ref byte target, int index, bool bit)
        {
            target = BitUtils.SetBit(target, index, bit);
            return 12;
        }

        public int SetBit(byte addrHigh, byte addrLow, int index, bool bit)
        {
            ushort address = BitUtils.BytesToUshort(addrHigh, addrLow);
            byte data = memory.ReadByte(address);
            SetBit(ref data, index, bit);
            memory.WriteByte(address, data);
            return 20;
        }

        public int ShiftRight(ref byte target, ref byte flags)
        {
            bool carry = (target & 1) > 0;
            target = (byte)(target >> 1);
            FlagUtils.SetFlags(ref flags, target == 0, false, false, carry);
            return 12;
        }

        public int ShiftRight(byte addrHigh, byte addrLow, ref byte flags)
        {
            ushort address = BitUtils.BytesToUshort(addrHigh, addrLow);
            byte data = memory.ReadByte(address);
            ShiftRight(ref data, ref flags);
            memory.WriteByte(address, data);
            return 20;
        }

        public int Swap(ref byte register, ref byte flags)
        {
            int lower = register & 0xF;
            int higher = register & 0xF0;
            register = (byte)(higher >> 4 | lower << 4);
            FlagUtils.SetFlags(ref flags, register == 0, false, false, false);
            return 12;
        }

        public int Swap(byte addrHigh, byte addrLow, ref byte flags)
        {
            ushort address = BitUtils.BytesToUshort(addrHigh, addrLow);
            byte data = memory.ReadByte(address);
            Swap(ref data, ref flags);
            memory.WriteByte(address, data);
            return 20;
        }

        public int TestBit(byte register, int index, ref byte flags)
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
