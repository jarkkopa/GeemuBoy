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

        public void Load(ref byte dest, byte value)
        {
            dest = value;
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

        public void Load(ref ushort dest, byte valueHigh, byte valueLow)
        {
            dest = BitUtils.BytesToUshort(valueHigh, valueLow);
            TickEvent?.Invoke();
        }

        public void LoadAdjusted(ref byte destHigh, ref byte destLow, ushort value, byte addValue, ref byte flags)
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
        }

        public void LoadFromAddress(ref byte dest, byte addrHigh, byte addrLow)
        {
            LoadFromAddress(ref dest, BitUtils.BytesToUshort(addrHigh, addrLow));
        }

        public void LoadFromAddress(ref byte dest, ushort address)
        {
            dest = memory.ReadByte(address);
            TickEvent?.Invoke();
        }

        public void LoadFromAddressAndIncrement(ref byte dest, ref byte addrHigh, ref byte addrLow, short value)
        {
            ushort address = BitUtils.BytesToUshort(addrHigh, addrLow);
            dest = memory.ReadByte(address);
            TickEvent?.Invoke();
            address = (ushort)(address + value);
            addrHigh = BitUtils.MostSignificantByte(address);
            addrLow = BitUtils.LeastSignificantByte(address);
        }

        public void WriteToAddress(byte addrHigh, byte addrLow, byte value)
        {
            WriteToAddress(BitUtils.BytesToUshort(addrHigh, addrLow), value);
        }

        public void WriteToAddress(ushort address, byte value)
        {
            memory.WriteByte(address, value);
            TickEvent?.Invoke();
        }

        public void WriteToAddress(ushort address, ushort value)
        {
            memory.WriteWord(address, value);
            TickEvent?.Invoke();
            TickEvent?.Invoke();
        }

        public void WriteToAddressAndIncrement(ref byte addrHigh, ref byte addrLow, byte value, short addValue)
        {
            ushort address = BitUtils.BytesToUshort(addrHigh, addrLow);
            memory.WriteByte(address, value);
            TickEvent?.Invoke();
            address = (ushort)(address + addValue);
            addrHigh = BitUtils.MostSignificantByte(address);
            addrLow = BitUtils.LeastSignificantByte(address);
        }

        public void Push(ref ushort pointer, byte valueHigh, byte valueLow)
        {
            pointer -= 2;
            TickEvent?.Invoke();
            memory.WriteWord(pointer, BitUtils.BytesToUshort(valueHigh, valueLow));
            TickEvent?.Invoke();
            TickEvent?.Invoke();
        }

        public void Pop(ref byte valueHigh, ref byte valueLow, ref ushort pointer)
        {
            ushort value = memory.ReadWord(pointer);
            pointer += 2;
            TickEvent?.Invoke();
            TickEvent?.Invoke();
            valueHigh = BitUtils.MostSignificantByte(value);
            valueLow = BitUtils.LeastSignificantByte(value);
        }

        public void PopWithFlags(ref byte valueHigh, ref byte valueLow, ref ushort pointer, ref byte flags)
        {
            Pop(ref valueHigh, ref valueLow, ref pointer);
            FlagUtils.SetFlags(ref flags,
                valueLow.IsBitSet(7),
                valueLow.IsBitSet(6),
                valueLow.IsBitSet(5),
                valueLow.IsBitSet(4));
            flags = (byte)(flags & 0xF0);
        }
    }
}
