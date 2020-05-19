namespace GeemuBoy.GB.CpuUnits
{
    public interface IMiscUnit
    {
        public int EnableInterruptMasterFlag(ref int enableAfter);
        public int DisableInterruptMasterFlag(ref bool flag);
        public int Nop();
        public int SetCarry(ref byte flags);
        public int DecimalAdjust(ref byte register);
    }

    public class MiscUnit : IMiscUnit
    {
        private readonly Memory memory;

        public MiscUnit(Memory memory)
        {
            this.memory = memory;
        }

        public int Nop()
        {
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

        public int SetCarry(ref byte flags)
        {
            FlagUtils.SetFlag(Flag.C, true, ref flags);
            FlagUtils.SetFlag(Flag.N, false, ref flags);
            FlagUtils.SetFlag(Flag.H, false, ref flags);
            return 4;
        }

        public int DecimalAdjust(ref byte register)
        {
            // TODO
            return 4;
        }
    }
}
