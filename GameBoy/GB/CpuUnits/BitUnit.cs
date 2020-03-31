namespace GameBoy.GB.CpuUnits
{
    public interface IBitUnit
    {
        public void RotateLeft(ref byte register, ref byte flags);
        public void RotateLeft(byte addrHigh, byte addrLow, ref byte flags);
        public void TestBit(byte register, byte index, ref byte flags);
    }

    public class BitUnit : IBitUnit
    {
        private readonly Memory memory;

        public BitUnit(Memory memory)
        {
            this.memory = memory;
        }

        public void RotateLeft(ref byte register, ref byte flags)
        {
            FlagUtils.SetFlag(Flag.C, BitUtils.GetBit(register, 7), ref flags);

            register = (byte)(((register << 1) & 0xFF) | (FlagUtils.GetFlag(Flag.C, flags) ? 1 : 0));

            FlagUtils.SetFlag(Flag.Z, register == 0, ref flags);
            FlagUtils.SetFlag(Flag.N, false, ref flags);
            FlagUtils.SetFlag(Flag.H, false, ref flags);
        }

        public void RotateLeft(byte addrHigh, byte addrLow, ref byte flags)
        {
            ushort address = BitUtils.BytesToUshort(addrHigh, addrLow);
            byte data = memory.ReadByte(address);
            RotateLeft(ref data, ref flags);
            memory.WriteByte(address, data);
        }

        public void TestBit(byte register, byte index, ref byte flags)
        {
            FlagUtils.SetFlag(Flag.Z, BitUtils.GetBit(register, index) == false, ref flags);
            FlagUtils.SetFlag(Flag.N, false, ref flags);
            FlagUtils.SetFlag(Flag.H, true, ref flags);
        }
    }
}
