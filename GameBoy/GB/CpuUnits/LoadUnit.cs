namespace GameBoy.GB.CpuUnits
{
    public interface ILoadUnit
    {
        public int Copy(ref byte dest, ref byte source);
        public int LoadImmediate8(ref byte dest, ref ushort PC);
        public int LoadFromAddress(ref byte dest, ushort address);
        public int LoadImmediate8ToAddress(ushort address, ref ushort PC);
        public int LoadFromImmediateAddress(ref byte dest, ref ushort PC);
        public int WriteToAddress(ushort address, ref byte source);
        public int WriteImmediateAddress(ref byte source, ref ushort PC);
    }

    public class LoadUnit : ILoadUnit
    {
        private readonly Memory _memory;

        public LoadUnit(Memory memory)
        {
            _memory = memory;
        }

        public int Copy(ref byte dest, ref byte source)
        {
            dest = source;
            return 4;
        }

        public int LoadImmediate8(ref byte dest, ref ushort PC)
        {
            dest = _memory.ReadByte(PC);
            PC++;
            return 8;
        }

        public int LoadFromAddress(ref byte dest, ushort address)
        {
            byte value = _memory.ReadByte(address);
            dest = value;
            return 8;
        }

        public int LoadImmediate8ToAddress(ushort address, ref ushort PC)
        {
            byte data = _memory.ReadByte(PC);
            PC++;
            _memory.WriteByte(address, data);
            return 12;
        }

        public int LoadFromImmediateAddress(ref byte dest, ref ushort PC)
        {
            byte addressHigh = _memory.ReadByte(PC);
            PC++;
            byte addressLow = _memory.ReadByte(PC);
            PC++;
            dest = _memory.ReadByte(BitUtils.BytesToUshort(addressHigh, addressLow));
            return 16;
        }

        public int WriteToAddress(ushort address, ref byte source)
        {
            _memory.WriteByte(address, source);
            return 8;
        }

        public int WriteImmediateAddress(ref byte source, ref ushort PC)
        {
            byte addrHigh = _memory.ReadByte(PC);
            PC++;
            byte addrLow = _memory.ReadByte(PC);
            PC++;
            _memory.WriteByte(BitUtils.BytesToUshort(addrHigh, addrLow), source);
            return 16;
        }
    }
}
