namespace GameBoy.GB.CpuUnits
{
    public interface IALU
    {
        public int Add(ref byte to, byte value, ref byte flags, bool addCarryFlag = false);
        public int AddFromMemory(ref byte to, byte addrHigh, byte addrLow, ref byte flags, bool addCarryFlag = false);
        public int Subtract(ref byte from, byte value, ref byte flags, bool subtractCarryFlag = false);
        public int SubtractFromMemory(ref byte from, byte addrHigh, byte addrLow, ref byte flags, bool subtractCarryFlag = false);
    }

    public class ALU : IALU
    {
        private readonly Memory memory;

        public ALU(Memory memory)
        {
            this.memory = memory;
        }

        public int Add(ref byte to, byte value, ref byte flags, bool addCarryFlag = false)
        {
            int additionalValue = addCarryFlag && FlagUtils.GetFlag(Flag.C, flags) ? 1 : 0;
            byte origValue = to;
            int newValue = to + value + additionalValue;
            to = (byte)(newValue & 0xFF);
            FlagUtils.SetFlag(Flag.Z, newValue == 0, ref flags);
            FlagUtils.SetFlag(Flag.N, false, ref flags);
            FlagUtils.SetFlag(Flag.H, (origValue & 0x0F) + (value & 0xF) > 0xF, ref flags);
            FlagUtils.SetFlag(Flag.C, newValue > 0xFF, ref flags);
            return 4;
        }

        public int AddFromMemory(ref byte to, byte addrHigh, byte addrLow, ref byte flags, bool addCarryFlag = false)
        {
            ushort address = BitUtils.BytesToUshort(addrHigh, addrLow);
            Add(ref to, memory.ReadByte(address), ref flags, addCarryFlag);
            return 8;
        }

        public int Subtract(ref byte from, byte value, ref byte flags, bool subtractCarryFlag = false)
        {
            int additionalValue = subtractCarryFlag && FlagUtils.GetFlag(Flag.C, flags) ? 1 : 0;
            byte origValue = from;
            int newValue = from - value - additionalValue;
            from = (byte)(newValue & 0xFF);
            FlagUtils.SetFlag(Flag.Z, from == 0, ref flags);
            FlagUtils.SetFlag(Flag.N, true, ref flags);
            FlagUtils.SetFlag(Flag.H, (origValue & 0x0F) < ((value + additionalValue) & 0xF), ref flags);
            FlagUtils.SetFlag(Flag.C, origValue < (value+additionalValue), ref flags);
            return 4;
        }

        public int SubtractFromMemory(ref byte from, byte addrHigh, byte addrLow, ref byte flags, bool subtractCarryFlag = false)
        {
            ushort address = BitUtils.BytesToUshort(addrHigh, addrLow);
            Subtract(ref from, memory.ReadByte(address), ref flags, subtractCarryFlag);
            return 8;
        }
    }
}
