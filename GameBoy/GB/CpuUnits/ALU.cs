namespace GameBoy.GB.CpuUnits
{
    public interface IALU
    {
        public int Add(ref byte to, byte value, ref byte flags);
        public int AddFromMemory(ref byte to, byte addrHigh, byte addrLow, ref byte flags);
    }

    public class ALU : IALU
    {
        private readonly Memory memory;

        public ALU(Memory memory)
        {
            this.memory = memory;
        }

        public int Add(ref byte to, byte value, ref byte flags)
        {
            byte origValue = to;
            int newValue = to + value;
            to = (byte)(newValue & 0xFF);
            FlagUtils.SetFlag(Flag.Z, newValue == 0, ref flags);
            FlagUtils.SetFlag(Flag.N, false, ref flags);
            FlagUtils.SetFlag(Flag.H, (origValue & 0x0F) + (value & 0xF) > 0xF, ref flags);
            FlagUtils.SetFlag(Flag.C, newValue > 0xFF, ref flags);
            return 4;
        }

        public int AddFromMemory(ref byte to, byte addrHigh, byte addrLow, ref byte flags)
        {
            ushort address = BitUtils.BytesToUshort(addrHigh, addrLow);
            Add(ref to, memory.ReadByte(address), ref flags);
            return 8;
        }
    }
}
