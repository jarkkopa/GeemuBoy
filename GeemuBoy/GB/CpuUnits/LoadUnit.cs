﻿namespace GeemuBoy.GB.CpuUnits
{
    public interface ILoadUnit
    {
        public int Load(ref byte dest, byte value);
        public void Load(ref byte destHigh, ref byte destLow, ushort value);
        public void Load(ref ushort dest, ushort value);
        public int Load(ref ushort dest, byte valueHigh, byte valueLow);
        public void LoadAdjusted(ref byte destHigh, ref byte destLow, ushort value, byte signedValue, ref byte flags);
        public int LoadFromAddress(ref byte dest, byte addrHigh, byte addrLow);
        public int LoadFromAddress(ref byte dest, ushort address);
        public int LoadFromAddressAndIncrement(ref byte dest, ref byte addrHigh, ref byte addrLow, short value);
        public int WriteToAddress(byte addrHigh, byte addrLow, byte value);
        public int WriteToAddress(ushort address, byte value);
        public void WriteToAddress(ushort address, ushort value);
        public int WriteToAddressAndIncrement(ref byte addrHigh, ref byte addrLow, byte value, short addValue);
        public void WriteToImmediateAddress(byte value, ref ushort PC);
        public int Push(ref ushort pointer, byte valueHigh, byte valueLow);
        public int Pop(ref byte valueHigh, ref byte valuelow, ref ushort pointer);
    }

    public class LoadUnit : ILoadUnit
    {
        private readonly Memory memory;

        public LoadUnit(Memory memory)
        {
            this.memory = memory;
        }

        public int Load(ref byte dest, byte value)
        {
            dest = value;
            return 4;
        }

        public void Load(ref byte destHigh, ref byte destLow, ushort value)
        {
            destHigh = BitUtils.MostSignificantByte(value);
            destLow = BitUtils.LeastSignificantByte(value);
        }

        public void Load(ref ushort dest, ushort value)
        {
            dest = value;
        }

        public int Load(ref ushort dest, byte valueHigh, byte valueLow)
        {
            dest = BitUtils.BytesToUshort(valueHigh, valueLow);
            return 8;
        }

        public void LoadAdjusted(ref byte destHigh, ref byte destLow, ushort value, byte addValue, ref byte flags)
        {
            ushort originalValue = value;
            sbyte signed = unchecked((sbyte)addValue);
            if (signed > 0)
            {
                FlagUtils.SetFlag(Flag.C, (originalValue + signed) > 0xFF, ref flags);
                FlagUtils.SetFlag(Flag.H, (originalValue & 0x0F) + (signed & 0x0F) > 0xF, ref flags);
            }
            else
            {
                FlagUtils.SetFlag(Flag.C, originalValue < addValue, ref flags);
                FlagUtils.SetFlag(Flag.H, (originalValue & 0x0F) < (addValue & 0x0F), ref flags);
            }
            ushort result = (ushort)(value + signed);
            destHigh = BitUtils.MostSignificantByte(result);
            destLow = BitUtils.LeastSignificantByte(result);
            FlagUtils.SetFlag(Flag.Z, false, ref flags);
            FlagUtils.SetFlag(Flag.N, false, ref flags);
        }

        public int LoadFromAddress(ref byte dest, byte addrHigh, byte addrLow)
        {
            LoadFromAddress(ref dest, BitUtils.BytesToUshort(addrHigh, addrLow));
            return 8;
        }

        public int LoadFromAddress(ref byte dest, ushort address)
        {
            dest = memory.ReadByte(address);
            return 8;
        }

        public int LoadFromAddressAndIncrement(ref byte dest, ref byte addrHigh, ref byte addrLow, short value)
        {
            ushort address = BitUtils.BytesToUshort(addrHigh, addrLow);
            dest = memory.ReadByte(address);
            address = (ushort)(address + value);
            addrHigh = BitUtils.MostSignificantByte(address);
            addrLow = BitUtils.LeastSignificantByte(address);
            return 8;
        }

        public int WriteToAddress(byte addrHigh, byte addrLow, byte value)
        {
            WriteToAddress(BitUtils.BytesToUshort(addrHigh, addrLow), value);
            return 8;
        }

        public int WriteToAddress(ushort address, byte value)
        {
            memory.WriteByte(address, value);
            return 8;
        }

        public void WriteToAddress(ushort address, ushort value)
        {
            memory.WriteWord(address, value);
        }

        public int WriteToAddressAndIncrement(ref byte addrHigh, ref byte addrLow, byte value, short addValue)
        {
            ushort address = BitUtils.BytesToUshort(addrHigh, addrLow);
            memory.WriteByte(address, value);
            address = (ushort)(address + addValue);
            addrHigh = BitUtils.MostSignificantByte(address);
            addrLow = BitUtils.LeastSignificantByte(address);
            return 8;
        }

        public void WriteToImmediateAddress(byte value, ref ushort PC)
        {
            byte addrHigh = memory.ReadByte(PC);
            PC++;
            byte addrLow = memory.ReadByte(PC);
            PC++;
            memory.WriteByte(BitUtils.BytesToUshort(addrHigh, addrLow), value);
        }

        public int Push(ref ushort pointer, byte valueHigh, byte valueLow)
        {
            pointer -= 2;
            memory.WriteWord(pointer, BitUtils.BytesToUshort(valueHigh, valueLow));
            return 16;
        }

        public int Pop(ref byte valueHigh, ref byte valueLow, ref ushort pointer)
        {
            ushort value = memory.ReadWord(pointer);
            pointer += 2;
            valueHigh = BitUtils.MostSignificantByte(value);
            valueLow = BitUtils.LeastSignificantByte(value);
            return 12;
        }
    }
}