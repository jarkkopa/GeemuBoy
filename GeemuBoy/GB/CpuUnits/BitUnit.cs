namespace GeemuBoy.GB.CpuUnits
{
    public class BitUnit
    {
        private readonly Memory memory;

        public event CPU.TickHandler? TickEvent;

        public BitUnit(Memory memory)
        {
            this.memory = memory;
        }

        public int Complement(ref byte value, ref byte flags)
        {
            value = (byte)~value;
            FlagUtils.SetFlag(Flag.N, true, ref flags);
            FlagUtils.SetFlag(Flag.H, true, ref flags);
            return 0;
        }

        public int ComplementCarry(ref byte flags)
        {
            FlagUtils.SetFlag(Flag.C, !FlagUtils.GetFlag(Flag.C, flags), ref flags);
            FlagUtils.SetFlag(Flag.H, false, ref flags);
            FlagUtils.SetFlag(Flag.N, false, ref flags);
            return 0;
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
            return 0;
        }

        public int RotateLeft(byte addrHigh, byte addrLow, ref byte flags)
        {
            ushort address = BitUtils.BytesToUshort(addrHigh, addrLow);
            byte data = memory.ReadByte(address);
            TickEvent?.Invoke();
            RotateLeft(ref data, ref flags, false);
            memory.WriteByte(address, data);
            TickEvent?.Invoke();
            return 0;
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
            return 0;
        }

        public int RotateLeftThroughCarry(byte addrHigh, byte addrLow, ref byte flags)
        {
            ushort address = BitUtils.BytesToUshort(addrHigh, addrLow);
            byte data = memory.ReadByte(address);
            TickEvent?.Invoke();
            RotateLeftThroughCarry(ref data, ref flags, false);
            memory.WriteByte(address, data);
            TickEvent?.Invoke();
            return 0;
        }

        public int RotateRight(ref byte register, ref byte flags, bool alwaysResetZero)
        {
            bool setCarry = register.IsBitSet(0);
            register = (byte)((register >> 1) | (setCarry ? 1 << 7 : 0));
            if (alwaysResetZero)
            {
                FlagUtils.SetFlags(ref flags, false, false, false, setCarry);
            }
            else
            {
                FlagUtils.SetFlags(ref flags, register == 0, false, false, setCarry);
            }
            return 0;
        }

        public int RotateRight(byte addrHigh, byte addrLow, ref byte flags)
        {
            ushort address = BitUtils.BytesToUshort(addrHigh, addrLow);
            byte data = memory.ReadByte(address);
            TickEvent?.Invoke();
            RotateRight(ref data, ref flags, false);
            memory.WriteByte(address, data);
            TickEvent?.Invoke();
            return 0;
        }

        public int RotateRightThroughCarry(ref byte register, ref byte flags, bool alwaysResetZero)
        {
            int carry = FlagUtils.GetFlag(Flag.C, flags) ? 0x80 : 0x0;
            bool setCarry = register.IsBitSet(0);
            register = (byte)((register >> 1) | carry);
            FlagUtils.SetFlags(ref flags, !alwaysResetZero && register == 0, false, false, setCarry);
            return 0;
        }

        public int RotateRightThroughCarry(byte addrHigh, byte addrLow, ref byte flags)
        {
            ushort address = BitUtils.BytesToUshort(addrHigh, addrLow);
            byte data = memory.ReadByte(address);
            TickEvent?.Invoke();
            RotateRightThroughCarry(ref data, ref flags, false);
            memory.WriteByte(address, data);
            TickEvent?.Invoke();
            return 0;
        }

        public int SetBit(ref byte target, int index, bool bit)
        {
            target = BitUtils.SetBit(target, index, bit);
            return 0;
        }

        public int SetBit(byte addrHigh, byte addrLow, int index, bool bit)
        {
            ushort address = BitUtils.BytesToUshort(addrHigh, addrLow);
            byte data = memory.ReadByte(address);
            TickEvent?.Invoke();
            SetBit(ref data, index, bit);
            memory.WriteByte(address, data);
            TickEvent?.Invoke();
            return 0;
        }

        public int ShiftLeftArithmetic(ref byte target, ref byte flags)
        {
            bool setCarry = (target & 0x80) > 0;
            target = (byte)((target << 1) & 0xFF);
            FlagUtils.SetFlags(ref flags, target == 0, false, false, setCarry);
            return 0;
        }

        public int ShiftLeftArithmetic(byte addrHigh, byte addrLow, ref byte flags)
        {
            ushort address = BitUtils.BytesToUshort(addrHigh, addrLow);
            byte data = memory.ReadByte(address);
            TickEvent?.Invoke();
            ShiftLeftArithmetic(ref data, ref flags);
            memory.WriteByte(address, data);
            TickEvent?.Invoke();
            return 0;
        }

        public int ShiftRightArithmetic(ref byte target, ref byte flags)
        {
            byte msb = (byte)(target & 0x80);
            bool setCarry = (target & 1) > 0;
            target = (byte)((target >> 1) | msb);
            FlagUtils.SetFlags(ref flags, target == 0, false, false, setCarry);
            return 0;
        }

        public int ShiftRightArithmetic(byte addrHigh, byte addrLow, ref byte flags)
        {
            ushort address = BitUtils.BytesToUshort(addrHigh, addrLow);
            byte data = memory.ReadByte(address);
            TickEvent?.Invoke();
            ShiftRightArithmetic(ref data, ref flags);
            memory.WriteByte(address, data);
            TickEvent?.Invoke();
            return 0;
        }

        public int ShiftRightLogic(ref byte target, ref byte flags)
        {
            bool carry = (target & 1) > 0;
            target = (byte)(target >> 1);
            FlagUtils.SetFlags(ref flags, target == 0, false, false, carry);
            return 0;
        }

        public int ShiftRightLogic(byte addrHigh, byte addrLow, ref byte flags)
        {
            ushort address = BitUtils.BytesToUshort(addrHigh, addrLow);
            byte data = memory.ReadByte(address);
            TickEvent?.Invoke();
            ShiftRightLogic(ref data, ref flags);
            memory.WriteByte(address, data);
            TickEvent?.Invoke();
            return 0;
        }

        public int Swap(ref byte register, ref byte flags)
        {
            int lower = register & 0xF;
            int higher = register & 0xF0;
            register = (byte)(higher >> 4 | lower << 4);
            FlagUtils.SetFlags(ref flags, register == 0, false, false, false);
            return 0;
        }

        public int Swap(byte addrHigh, byte addrLow, ref byte flags)
        {
            ushort address = BitUtils.BytesToUshort(addrHigh, addrLow);
            byte data = memory.ReadByte(address);
            TickEvent?.Invoke();
            Swap(ref data, ref flags);
            memory.WriteByte(address, data);
            TickEvent?.Invoke();
            return 0;
        }

        public int TestBit(byte register, int index, ref byte flags)
        {
            FlagUtils.SetFlag(Flag.Z, register.IsBitSet(index) == false, ref flags);
            FlagUtils.SetFlag(Flag.N, false, ref flags);
            FlagUtils.SetFlag(Flag.H, true, ref flags);
            return 0;
        }

        private void RotateLeft(ref byte register, bool setLastBit)
        {
            register = (byte)(((register << 1) & 0xFF) | (setLastBit ? 1 : 0));
        }
    }
}
