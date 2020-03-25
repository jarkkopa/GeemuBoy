using System;
using System.Collections.Generic;
using System.Text;

namespace GameBoy.GB.CpuUnits
{
    public interface IMiscUnit
    {
        public void SetInterruptMasterEnable(ref bool master, bool enabled);
        public void Nop();
    }

    public class MiscUnit : IMiscUnit
    {
        private readonly Memory memory;

        public MiscUnit(Memory memory)
        {
            this.memory = memory;
        }

        public void SetInterruptMasterEnable(ref bool master, bool enabled)
        {
            // TODO: Don't set ime immediately. Set it after the next intruction
            master = enabled;
        }

        public void Nop() { }
    }
}
