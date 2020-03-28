namespace GameBoy.GB.CpuUnits
{
    public interface IBitUnit
    {
        public void TestBit(byte register, byte index, ref byte flags);
    }

    public class BitUnit : IBitUnit
    {
        public void TestBit(byte register, byte index, ref byte flags)
        {
            FlagUtils.SetFlag(Flag.Z, BitUtils.GetBit(register, index) == false, ref flags);
            FlagUtils.SetFlag(Flag.N, false, ref flags);
            FlagUtils.SetFlag(Flag.H, true, ref flags);
        }
    }
}
