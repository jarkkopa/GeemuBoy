﻿using System;
using System.Collections.Generic;
using System.Text;

namespace GameBoy.GB.CpuUnits
{
    public interface IJumpUnit
    {
        public void Call(ushort address, ref ushort sp, ref ushort pc);
        public void JumpToAddress(ushort address, ref ushort pc);
        public void JumpToAddressConditional(ushort address, ref ushort pc, Flag flag, bool condition, byte flags);
    }

    public class JumpUnit : IJumpUnit
    {
        private readonly Memory memory;

        public JumpUnit(Memory memory)
        {
            this.memory = memory;
        }

        public void Call(ushort address, ref ushort sp, ref ushort pc)
        {
            sp = (ushort)(sp - 2);
            memory.WriteWord(sp, pc);
            pc = address;
        }

        public void JumpToAddress(ushort address, ref ushort pc)
        {
            pc = address;
        }

        public void JumpToAddressConditional(ushort address, ref ushort pc, Flag flag, bool condition, byte flags)
        {
            if (FlagUtils.GetFlag(flag, flags) == condition)
            {
                JumpToAddress(address, ref pc);
            }
        }
    }
}
