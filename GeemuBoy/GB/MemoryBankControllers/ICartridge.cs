namespace GeemuBoy.GB.MemoryBankControllers
{
    public interface ICartridge
    {
        public byte ReadByte(ushort addr);
        public void WriteByte(ushort addr, byte data);
    }
}
