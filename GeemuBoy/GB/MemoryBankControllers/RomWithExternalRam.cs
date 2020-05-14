namespace GeemuBoy.GB.MemoryBankControllers
{
    public class RomWithExternalRam : ICartridge
    {
        private readonly byte[] rom;
        private readonly byte[] ram;

        public RomWithExternalRam(byte[]? rom)
        {
            this.rom = new byte[0x8000];
            if (rom != null)
            {
                for (int i = 0; i < rom.Length; i++)
                {
                    this.rom[i] = rom[i];
                }
            }

            this.ram = new byte[0x2000];
        }

        public byte ReadByte(ushort addr)
        {
            if (addr < 0x8000)
            {
                return rom[addr];
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
