namespace GameBoy.GB
{
    public static class BitUtils
    {
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
    }
}
