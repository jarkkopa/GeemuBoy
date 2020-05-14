namespace GeemuBoy.GB.MemoryBankControllers
{
    public class RomOnly : ICartridge
    {
        private readonly byte[] cartridge;
        private readonly byte[] ram;

        public RomOnly(byte[] cartridge, int ramSize)
        {
            this.cartridge = cartridge;
            ram = new byte[ramSize];
        }

        public byte ReadByte(ushort addr)
        {
            if (addr < 0x8000)
            {
                return cartridge[addr];
            }
            else if (addr >= 0xA000 && addr < 0xC000)
            {
                return ram[addr - 0xA000];
            }
            return 0xFF;
        }

        public void WriteByte(ushort addr, byte data)
        {
            if (addr >= 0xA000 && addr < 0xC000)
            {
                ram[addr - 0xA000] = data;
            }
        }
    }
}
