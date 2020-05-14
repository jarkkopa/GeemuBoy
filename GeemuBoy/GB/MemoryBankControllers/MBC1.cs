namespace GeemuBoy.GB.MemoryBankControllers
{
    public class MBC1: ICartridge
    {
        private enum BankSelectMode { Ram, Rom };

        private readonly byte[] cartridge;

        private bool externalRamEnabled = false;
        private ushort romBankNumber = 0x1;
        private byte ramBankNumber = 0x0;
        private BankSelectMode bankSelectMode = BankSelectMode.Rom;

        public MBC1(byte[] cartridge, int ramSize)
        {
            this.cartridge = cartridge;
        }

        public byte ReadByte(ushort addr)
        {
            throw new System.NotImplementedException();
        }

        public void WriteByte(ushort addr, byte data)
        {
            throw new System.NotImplementedException();
        }
    }
}
