using System;
using System.Collections.Generic;
using System.Text;

namespace GameBoy.GB.CpuUnits
{
    public interface IJumpUnit
    {
        public void JumpToAddress(ushort address, ref ushort pc);
    }

    public class JumpUnit : IJumpUnit
    {
        private readonly Memory memory;

        public JumpUnit(Memory memory)
        {
            this.memory = memory;
        }

        public void JumpToAddress(ushort address, ref ushort pc)
        {
            pc = address;
        }
    }
}
