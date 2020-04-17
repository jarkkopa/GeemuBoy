namespace GeemuBoy.GB.CpuUnits
{
    public interface IMiscUnit
    {
        public int EnableInterruptMasterFlag(ref int enableAfter);
        public int DisableInterruptMasterFlag(ref bool flag);
        public int Nop();
    }

    public class MiscUnit : IMiscUnit
    {
        private readonly Memory memory;

        public MiscUnit(Memory memory)
        {
            this.memory = memory;
        }

        public int Nop() {
            return 4;
        }

        public int EnableInterruptMasterFlag(ref int enableAfter)
        {
            enableAfter = 1;
            return 4;
        }

        public int DisableInterruptMasterFlag(ref bool flag)
        {
            flag = false;
            return 4;
        }
    }
}
