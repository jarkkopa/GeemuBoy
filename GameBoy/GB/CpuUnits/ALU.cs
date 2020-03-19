namespace GameBoy.GB.CpuUnits
{
    public interface IALU
    {
        public int Add(ref byte to, byte value, ref byte flags, bool addCarryFlag = false);
        public int Subtract(ref byte from, byte value, ref byte flags, bool subtractCarryFlag = false);
        public int And(ref byte to, byte value, ref byte flags);
        public int Or(ref byte to, byte value, ref byte flags);
        public int Xor(ref byte to, byte value, ref byte flags);
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
            FlagUtils.SetFlags(ref flags,
                newValue == 0, false,
                (origValue & 0x0F) + (value & 0xF) > 0xF,
                newValue > 0xFF);
            return 4;
        }

        public int Subtract(ref byte from, byte value, ref byte flags, bool subtractCarryFlag = false)
        {
            int additionalValue = subtractCarryFlag && FlagUtils.GetFlag(Flag.C, flags) ? 1 : 0;
            byte origValue = from;
            int newValue = from - value - additionalValue;
            from = (byte)(newValue & 0xFF);
            FlagUtils.SetFlags(ref flags,
                from == 0,
                true,
                (origValue & 0x0F) < ((value + additionalValue) & 0xF),
                origValue < (value + additionalValue));
            return 4;
        }

        public int And(ref byte to, byte value, ref byte flags)
        {
            to = (byte)(to & value);
            FlagUtils.SetFlags(ref flags,
                to == 0,
                false,
                true,
                false);
            return 4;
        }

        public int Or(ref byte to, byte value, ref byte flags)
        {
            to = (byte)(to | value);
            FlagUtils.SetFlags(
                ref flags,
                to == 0,
                false,
                false,
                false);
            return 4;
        }

        public int Xor(ref byte to, byte value, ref byte flags)
        {
            to = (byte)(to ^ value);
            FlagUtils.SetFlags(ref flags,
                to == 0,
                false,
                false,
                false);
            return 4;
        }
    }
}
