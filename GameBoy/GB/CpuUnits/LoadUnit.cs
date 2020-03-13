namespace GameBoy.GB.CpuUnits
{
    public interface ILoadUnit
    {
        public int Copy(ref byte dest, byte source);
        public int LoadImmediateByte(ref byte dest, ref ushort PC);
        public int LoadFromAddress(ref byte dest, byte addrHigh, byte addrLow);
        public int LoadFromAddress(ref byte dest, ushort address);
        public int LoadFromAddressAndDecrement(ref byte dest, ref byte addrHigh, ref byte addrLow);
        public int LoadImmediateByteToAddress(byte addrHigh, byte addrLow, ref ushort PC);
        public int LoadFromImmediateAddress(ref byte dest, ref ushort PC);
        public int WriteToAddress(byte addrHigh, byte addrLow, byte source);
        public int WriteToAddress(ushort address, byte source);
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

        public int LoadImmediateByte(ref byte dest, ref ushort PC)
        {
            dest = memory.ReadByte(PC);
            PC++;
            return 8;
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

        public int LoadFromAddressAndDecrement(ref byte dest, ref byte addrHigh, ref byte addrLow)
        {
            ushort address = BitUtils.BytesToUshort(addrHigh, addrLow);
            dest = memory.ReadByte(address);
            address -= 1;
            addrHigh = BitUtils.MostSignificantByte(address);
            addrLow = BitUtils.LeastSignificantByte(address);
            return 8;
        }

        public int LoadImmediateByteToAddress(byte addrHigh, byte addrLow, ref ushort PC)
        {
            byte data = memory.ReadByte(PC);
            PC++;
            memory.WriteByte(BitUtils.BytesToUshort(addrHigh, addrLow), data);
            return 12;
        }

        public int LoadFromImmediateAddress(ref byte dest, ref ushort PC)
        {
            byte addressHigh = memory.ReadByte(PC);
            PC++;
            byte addressLow = memory.ReadByte(PC);
            PC++;
            dest = memory.ReadByte(BitUtils.BytesToUshort(addressHigh, addressLow));
            return 16;
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
