﻿namespace GeemuBoy.GB.CpuUnits
{
    public interface IBitUnit
    {
        public int Complement(ref byte value, ref byte flags);
        public int ComplementCarry(ref byte flags);
        public int RotateLeft(ref byte register, ref byte flags, bool alwaysResetZero);
        public int RotateLeft(byte addrHigh, byte addrLow, ref byte flags);
        public int RotateLeftThroughCarry(ref byte register, ref byte flags, bool alwaysResetZero);
        public int RotateLeftThroughCarry(byte addrHigh, byte addrLow, ref byte flags);
        public int RotateRight(ref byte register, ref byte flags, bool alwaysResetZero);
        public int RotateRight(byte addrHigh, byte addrLow, ref byte flags);
        public int RotateRightThroughCarry(ref byte register, ref byte flags, bool alwaysResetZero);
        public int RotateRightThroughCarry(byte addrHigh, byte addrLow, ref byte flags);
        public int SetBit(ref byte target, int index, bool bit);
        public int SetBit(byte addrHigh, byte addrLow, int index, bool bit);
        public int ShiftLeftArithmetic(ref byte target, ref byte flags);
        public int ShiftLeftArithmetic(byte addrHigh, byte addrLow, ref byte flags);
        public int ShiftRightArithmetic(ref byte target, ref byte flags);
        public int ShiftRightArithmetic(byte addrHigh, byte addrLow, ref byte flags);
        public int ShiftRightLogic(ref byte target, ref byte flags);
        public int ShiftRightLogic(byte addrHigh, byte addrLow, ref byte flags);
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

        public int ComplementCarry(ref byte flags)
        {
            FlagUtils.SetFlag(Flag.C, !FlagUtils.GetFlag(Flag.C, flags), ref flags);
            FlagUtils.SetFlag(Flag.H, false, ref flags);
            FlagUtils.SetFlag(Flag.N, false, ref flags);
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
            return 12;
        }

        public int RotateRight(byte addrHigh, byte addrLow, ref byte flags)
        {
            ushort address = BitUtils.BytesToUshort(addrHigh, addrLow);
            byte data = memory.ReadByte(address);
            RotateRight(ref data, ref flags, false);
            memory.WriteByte(address, data);
            return 20;
        }

        public int RotateRightThroughCarry(ref byte register, ref byte flags, bool alwaysResetZero)
        {
            int carry = FlagUtils.GetFlag(Flag.C, flags) ? 0x80 : 0x0;
            bool setCarry = register.IsBitSet(0);
            register = (byte)((register >> 1) | carry);
            FlagUtils.SetFlags(ref flags, !alwaysResetZero && register == 0, false, false, setCarry);
            return 12;
        }

        public int RotateRightThroughCarry(byte addrHigh, byte addrLow, ref byte flags)
        {
            ushort address = BitUtils.BytesToUshort(addrHigh, addrLow);
            byte data = memory.ReadByte(address);
            RotateRightThroughCarry(ref data, ref flags, false);
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

        public int ShiftLeftArithmetic(ref byte target, ref byte flags)
        {
            bool setCarry = (target & 0x80) > 0;
            target = (byte)((target << 1) & 0xFF);
            FlagUtils.SetFlags(ref flags, target == 0, false, false, setCarry);
            return 12;
        }

        public int ShiftLeftArithmetic(byte addrHigh, byte addrLow, ref byte flags)
        {
            ushort address = BitUtils.BytesToUshort(addrHigh, addrLow);
            byte data = memory.ReadByte(address);
            ShiftLeftArithmetic(ref data, ref flags);
            memory.WriteByte(address, data);
            return 20;
        }

        public int ShiftRightArithmetic(ref byte target, ref byte flags)
        {
            byte msb = (byte)(target & 0x80);
            bool setCarry = (target & 1) > 0;
            target = (byte)((target >> 1) | msb);
            FlagUtils.SetFlags(ref flags, target == 0, false, false, setCarry);
            return 12;
        }

        public int ShiftRightArithmetic(byte addrHigh, byte addrLow, ref byte flags)
        {
            ushort address = BitUtils.BytesToUshort(addrHigh, addrLow);
            byte data = memory.ReadByte(address);
            ShiftRightArithmetic(ref data, ref flags);
            memory.WriteByte(address, data);
            return 20;
        }

        public int ShiftRightLogic(ref byte target, ref byte flags)
        {
            bool carry = (target & 1) > 0;
            target = (byte)(target >> 1);
            FlagUtils.SetFlags(ref flags, target == 0, false, false, carry);
            return 12;
        }

        public int ShiftRightLogic(byte addrHigh, byte addrLow, ref byte flags)
        {
            ushort address = BitUtils.BytesToUshort(addrHigh, addrLow);
            byte data = memory.ReadByte(address);
            ShiftRightLogic(ref data, ref flags);
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
