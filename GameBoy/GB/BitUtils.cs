using System;
using System.Collections.Generic;
using System.Text;

namespace GameBoy.GB
{
    public static class BitUtils
    {
        public static ushort BytesToUshort(byte mostSignificant, byte leastSignificant)
        {
            return (ushort)(mostSignificant << 8 | leastSignificant);
        }
    }
}
