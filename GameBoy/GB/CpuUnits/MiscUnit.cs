namespace GameBoy.GB.CpuUnits
{
    public interface IMiscUnit
    {
        public void EnableInterruptMasterFlag(ref int enableAfter);
        public void DisableInterruptMasterFlag(ref bool flag);
        public void Nop();
    }

    public class MiscUnit : IMiscUnit
    {
        private readonly Memory memory;

        public MiscUnit(Memory memory)
        {
            this.memory = memory;
        }

        public void Nop() { }

        public void EnableInterruptMasterFlag(ref int enableAfter)
        {
            enableAfter = 1;
        }

        public void DisableInterruptMasterFlag(ref bool flag)
        {
            flag = false;
        }
    }
}
