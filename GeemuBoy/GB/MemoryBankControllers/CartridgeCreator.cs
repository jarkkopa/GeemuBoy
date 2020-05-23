using System;

namespace GeemuBoy.GB.MemoryBankControllers
{
    public static class CartridgeCreator
    {
        public static ICartridge GetCartridge(byte[]? data)
        {
            if (data == null || data.Length < 0x8000)
            {
                return new RomWithExternalRam(data);
            }

            int ramSize = GetRamSize(data[0x149]);
            return CreateCartridge(data, data[0x147], ramSize);
        }

        private static int GetRamSize(byte ramSize)
        {
            return ramSize switch
            {
                0x01 => 0x800,
                0x02 => 0x2000,
                0x03 => 0x8000,
                0x04 => 0x20000,
                0x05 => 0x10000,
                _ => 0
            };
        }

        private static ICartridge CreateCartridge(byte[] cartridgeData, byte typeValue, int ramSize) =>
            typeValue switch
            {
                0x00 => new RomOnly(cartridgeData, ramSize),
                0x01 => new MBC1(cartridgeData, ramSize),
                0x02 => new MBC1(cartridgeData, ramSize),
                0x03 => new MBC1(cartridgeData, ramSize),
                _ => throw new Exception($"Cartridge type {typeValue:X2} not supported.")
            };
    }
}
