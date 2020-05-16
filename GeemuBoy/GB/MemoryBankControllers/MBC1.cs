using System;

namespace GeemuBoy.GB.MemoryBankControllers
{
    public class MBC1 : ICartridge
    {
        private enum BankSelectMode { Ram, Rom };

        private readonly byte[] cartridge;
        private readonly byte[] ram;

        private readonly int ramBankSize;
        private bool externalRamEnabled = false;
        private ushort romBankLower = 0x1;
        private byte sharedBankSelector = 0x0;
        private BankSelectMode bankSelectMode = BankSelectMode.Rom;

        public MBC1(byte[] cartridge, int ramSize)
        {
            this.cartridge = cartridge;
            ram = new byte[ramSize];
            ramBankSize = ramSize >= 0x2000 ? 0x2000 : ramSize;

        }

        public byte ReadByte(ushort addr)
        {
            if (addr < 0x4000)
            {
                return cartridge[addr];
            }
            else if (addr < 0x8000)
            {
                int bankNumber = romBankLower;
                if (bankSelectMode == BankSelectMode.Rom)
                {
                    bankNumber |= (sharedBankSelector << 5);
                }
                return cartridge[addr + ((bankNumber - 1) * 0x4000)];
            }
            else if (addr >= 0xA000 && addr < 0xC000)
            {
                if (!externalRamEnabled)
                {
                    // Reading from disabled external RAM returns arbitrary data
                    return 0xFF;
                }

                int bankNumber = bankSelectMode == BankSelectMode.Ram ? sharedBankSelector : 0x0;
                return ram[addr - 0xA000 + (bankNumber * ramBankSize)];
            }
            else
            {
                throw new ArgumentException($"Trying to read from invalid memory bank address {addr:X4}");
            }
        }

        public void WriteByte(ushort addr, byte data)
        {
            if (addr < 0x2000)
            {
                externalRamEnabled = (data & 0xF) == 0xA;
            }
            else if (addr < 0x4000)
            {
                data = data switch
                {
                    0x0 => data += 1,
                    0x20 => data += 1,
                    0x40 => data += 1,
                    0x60 => data += 1,
                    _ => data
                };
                romBankLower = (ushort)(data & 0x1F);
            }
            else if (addr < 0x6000)
            {
                sharedBankSelector = (byte)(data & 0x3);
            }
            else if (addr < 0x8000)
            {
                bankSelectMode = data == 0x00 ? BankSelectMode.Rom : BankSelectMode.Ram;
            }
            else if (addr >= 0xA000 && addr < 0xC000 && externalRamEnabled)
            {
                int bankNumber = bankSelectMode == BankSelectMode.Ram ? sharedBankSelector : 0x0; ;
                ram[addr - 0xA000 + (bankNumber * ramBankSize)] = data;
            }
        }
    }
}
