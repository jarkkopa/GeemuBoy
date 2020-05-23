namespace GeemuBoy.GB
{
    public static class BitUtils
    {
        public static bool IsBitSet(this byte value, int index)
        {
            return GetBit(value, index);
        }

        public static bool IsBitSet(this ushort value, int index)
        {
            return GetBit(value, index);
        }

        public static ushort BytesToUshort(byte mostSignificant, byte leastSignificant)
        {
            return (ushort)(mostSignificant << 8 | leastSignificant);
        }

        public static byte MostSignificantByte(ushort value)
        {
            return (byte)(value >> 8);
        }

        public static byte LeastSignificantByte(ushort value)
        {
            return (byte)(value & 0xFF);
        }

        public static bool GetBit(ushort value, int index)
        {
            return (value & 1 << index) > 0;
        }

        public static byte SetBit(byte value, int index, bool bit)
        {
            if (bit)
            {
                return (byte)(value | (1 << index));
            }
            else
            {
                return (byte)(value & ~(1 << index));
            }
        }

        public static void SetBit(ref byte value, int index, bool bit)
        {
            if (bit)
            {
                value = (byte)(value | (1 << index));
            }
            else
            {
                value = (byte)(value & ~(1 << index));
            }
        }
    }
}
