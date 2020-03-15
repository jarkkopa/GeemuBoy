﻿using System;

namespace GameBoy.GB
{
    /// <summary>
    /// Flag index bit
    /// </summary>
    [Flags]
    public enum Flag
    {
        Z = 1 << 7, // Zero
        N = 1 << 6, // Subtract
        H = 1 << 5, // Half carry
        C = 1 << 4  // Carry
    }

    public static class FlagUtils
    {
        public static bool GetFlag(Flag flagIndex, byte flags)
        {
            return (flags & (byte)(flagIndex)) > 0;
        }

        public static void SetFlag(Flag flagIndex, bool setBit, ref byte flags)
        {
            if (setBit)
            {
                flags = (byte)(flags | (byte)flagIndex);
            }
            else
            {
                flags = (byte)(flags & ~(byte)(flagIndex));
            }
        }
    }
}
