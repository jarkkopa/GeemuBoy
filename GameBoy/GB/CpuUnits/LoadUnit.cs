namespace GameBoy.GB.CpuUnits
{
    public interface ILoadUnit
    {
        public int Copy(ref byte dest, byte source);
        public int LoadFromAddress(ref byte dest, byte addrHigh, byte addrLow);
        public int LoadFromAddress(ref byte dest, ushort address);
        public int LoadFromAddressAndIncrement(ref byte dest, ref byte addrHigh, ref byte addrLow, short value);
        public int WriteToAddress(byte addrHigh, byte addrLow, byte source);
        public int WriteToAddress(ushort address, byte source);
        public int WriteToAddressAndIncrement(ref byte addrHigh, ref byte addrLow, byte source, short value);
        public int WriteToImmediateAddress(byte source, ref ushort PC);
    }

    public class LoadUnit : ILoadUnit
    {
        private readonly Memory memory;

        public LoadUnit(Memory memory)
        {
            this.memory = memory;
        }

        public int Copy(ref byte dest, byte source)
        {
            dest = source;
            return 4;
        }

        public int LoadFromAddress(ref byte dest, byte addrHigh, byte addrLow)
        {
            return LoadFromAddress(ref dest, BitUtils.BytesToUshort(addrHigh, addrLow));
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

        public int WriteToAddress(byte addrHigh, byte addrLow, byte source)
        {
            return WriteToAddress(BitUtils.BytesToUshort(addrHigh, addrLow), source);
        }

        public int WriteToAddress(ushort address, byte source)
        {
            memory.WriteByte(address, source);
            return 8;
        }

        public int WriteToAddressAndIncrement(ref byte addrHigh, ref byte addrLow, byte source, short value)
        {
            ushort address = BitUtils.BytesToUshort(addrHigh, addrLow);
            memory.WriteByte(address, source);
            address = (ushort)(address + value);
            addrHigh = BitUtils.MostSignificantByte(address);
            addrLow = BitUtils.LeastSignificantByte(address);
            return 8;
        }

        public int WriteToImmediateAddress(byte source, ref ushort PC)
        {
            byte addrHigh = memory.ReadByte(PC);
            PC++;
            byte addrLow = memory.ReadByte(PC);
            PC++;
            memory.WriteByte(BitUtils.BytesToUshort(addrHigh, addrLow), source);
            return 16;
        }
    }
}
