namespace GameBoy.GB.CpuUnits
{
    public interface IALU
    {
        public void Add(ref byte to, byte value, ref byte flags, bool addCarryFlag = false);
        public void Subtract(ref byte from, byte value, ref byte flags, bool subtractCarryFlag = false);
        public void And(ref byte to, byte value, ref byte flags);
        public void Or(ref byte to, byte value, ref byte flags);
        public void Xor(ref byte to, byte value, ref byte flags);
        public void Compare(byte to, byte value, ref byte flags);
    }

    public class ALU : IALU
    {
        private readonly Memory memory;

        public ALU(Memory memory)
        {
            this.memory = memory;
        }

        public void Add(ref byte to, byte value, ref byte flags, bool addCarryFlag = false)
        {
            int additionalValue = addCarryFlag && FlagUtils.GetFlag(Flag.C, flags) ? 1 : 0;
            byte origValue = to;
            int newValue = to + value + additionalValue;
            to = (byte)(newValue & 0xFF);
            FlagUtils.SetFlags(ref flags,
                newValue == 0, false,
                (origValue & 0x0F) + (value & 0x0F) > 0x0F,
                newValue > 0xFF);
        }

        public void Subtract(ref byte from, byte value, ref byte flags, bool subtractCarryFlag = false)
        {
            int additionalValue = subtractCarryFlag && FlagUtils.GetFlag(Flag.C, flags) ? 1 : 0;
            byte origValue = from;
            int newValue = from - value - additionalValue;
            from = (byte)(newValue & 0xFF);
            FlagUtils.SetFlags(ref flags,
                from == 0,
                true,
                (origValue & 0x0F) < ((value + additionalValue) & 0x0F),
                origValue < (value + additionalValue));
        }

        public void And(ref byte to, byte value, ref byte flags)
        {
            to = (byte)(to & value);
            FlagUtils.SetFlags(ref flags,
                to == 0,
                false,
                true,
                false);
        }

        public void Or(ref byte to, byte value, ref byte flags)
        {
            to = (byte)(to | value);
            FlagUtils.SetFlags(
                ref flags,
                to == 0,
                false,
                false,
                false);
        }

        public void Xor(ref byte to, byte value, ref byte flags)
        {
            to = (byte)(to ^ value);
            FlagUtils.SetFlags(ref flags,
                to == 0,
                false,
                false,
                false);
        }

        public void Compare(byte to, byte value, ref byte flags)
        {
            byte result = (byte)((to - value) & 0xFF);
            FlagUtils.SetFlags(ref flags,
                result == 0,
                true,
                (to & 0x0F) < (value & 0x0F),
                to < value);
        }
    }
}
