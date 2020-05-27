namespace GeemuBoy.GB.CpuUnits
{
    public class LoadUnit
    {
        private readonly Memory memory;

        public event CPU.TickHandler? TickEvent;

        public LoadUnit(Memory memory)
        {
            this.memory = memory;
        }

        public int Load(ref byte dest, byte value)
        {
            dest = value;
            return 0;
        }

        public int Load(ref byte destHigh, ref byte destLow, ushort value)
        {
            destHigh = BitUtils.MostSignificantByte(value);
            destLow = BitUtils.LeastSignificantByte(value);
            return 0;
        }

        public int Load(ref ushort dest, ushort value)
        {
            dest = value;
            return 0;
        }

        public int Load(ref ushort dest, byte valueHigh, byte valueLow)
        {
            dest = BitUtils.BytesToUshort(valueHigh, valueLow);
            TickEvent?.Invoke();
            return 0;
        }

        public int LoadAdjusted(ref byte destHigh, ref byte destLow, ushort value, byte addValue, ref byte flags)
        {
            ushort originalValue = value;
            sbyte signed = unchecked((sbyte)addValue);
            FlagUtils.SetFlag(Flag.C, ((originalValue & 0xFF) + addValue) > 0xFF, ref flags);
            FlagUtils.SetFlag(Flag.H, (originalValue & 0x0F) + (addValue & 0x0F) > 0xF, ref flags);
            ushort result = (ushort)(value + signed);
            destHigh = BitUtils.MostSignificantByte(result);
            destLow = BitUtils.LeastSignificantByte(result);
            FlagUtils.SetFlag(Flag.Z, false, ref flags);
            FlagUtils.SetFlag(Flag.N, false, ref flags);
            TickEvent?.Invoke();
            return 0;
        }

        public int LoadFromAddress(ref byte dest, byte addrHigh, byte addrLow)
        {
            LoadFromAddress(ref dest, BitUtils.BytesToUshort(addrHigh, addrLow));
            return 0;
        }

        public int LoadFromAddress(ref byte dest, ushort address)
        {
            dest = memory.ReadByte(address);
            TickEvent?.Invoke();
            return 0;
        }

        public int LoadFromAddressAndIncrement(ref byte dest, ref byte addrHigh, ref byte addrLow, short value)
        {
            ushort address = BitUtils.BytesToUshort(addrHigh, addrLow);
            dest = memory.ReadByte(address);
            TickEvent?.Invoke();
            address = (ushort)(address + value);
            addrHigh = BitUtils.MostSignificantByte(address);
            addrLow = BitUtils.LeastSignificantByte(address);
            return 0;
        }

        public int WriteToAddress(byte addrHigh, byte addrLow, byte value)
        {
            WriteToAddress(BitUtils.BytesToUshort(addrHigh, addrLow), value);
            return 0;
        }

        public int WriteToAddress(ushort address, byte value)
        {
            memory.WriteByte(address, value);
            TickEvent?.Invoke();
            return 0;
        }

        public int WriteToAddress(ushort address, ushort value)
        {
            memory.WriteWord(address, value);
            TickEvent?.Invoke();
            TickEvent?.Invoke();
            return 0;
        }

        public int WriteToAddressAndIncrement(ref byte addrHigh, ref byte addrLow, byte value, short addValue)
        {
            ushort address = BitUtils.BytesToUshort(addrHigh, addrLow);
            memory.WriteByte(address, value);
            TickEvent?.Invoke();
            address = (ushort)(address + addValue);
            addrHigh = BitUtils.MostSignificantByte(address);
            addrLow = BitUtils.LeastSignificantByte(address);
            return 0;
        }

        public int Push(ref ushort pointer, byte valueHigh, byte valueLow)
        {
            pointer -= 2;
            TickEvent?.Invoke();
            memory.WriteWord(pointer, BitUtils.BytesToUshort(valueHigh, valueLow));
            TickEvent?.Invoke();
            TickEvent?.Invoke();
            return 0;
        }

        public int Pop(ref byte valueHigh, ref byte valueLow, ref ushort pointer)
        {
            ushort value = memory.ReadWord(pointer);
            pointer += 2;
            TickEvent?.Invoke();
            TickEvent?.Invoke();
            valueHigh = BitUtils.MostSignificantByte(value);
            valueLow = BitUtils.LeastSignificantByte(value);
            return 0;
        }

        public int PopWithFlags(ref byte valueHigh, ref byte valueLow, ref ushort pointer, ref byte flags)
        {
            Pop(ref valueHigh, ref valueLow, ref pointer);
            FlagUtils.SetFlags(ref flags,
                valueLow.IsBitSet(7),
                valueLow.IsBitSet(6),
                valueLow.IsBitSet(5),
                valueLow.IsBitSet(4));
            flags = (byte)(flags & 0xF0);
            return 0;
        }
    }
}
