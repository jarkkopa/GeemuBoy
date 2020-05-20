using GeemuBoy.GB.MemoryBankControllers;
using System;
using System.Collections.Generic;

namespace GeemuBoy.GB
{
    public class Memory
    {
        public enum MapMode { Boot, Cartridge }

        public const ushort MAX_ADDR = 0xFFFF;

        private const ushort BOOT_ROM_LOCK_ADDRESS = 0xFF50;

        public MapMode RomMapMode { get; private set; }
        public List<byte> Serial { get; private set; } = new List<byte>();

        private readonly InputRegister inputRegister = new InputRegister();

        private readonly byte[]? bootRom;
        private readonly ICartridge cartridge;
        private readonly byte[] videoRam = new byte[0x2000];
        private readonly byte[] workRam = new byte[0x2000];
        private readonly byte[] oam = new byte[0xA0];
        private readonly byte[] ioRegisters = new byte[0x80];
        private readonly byte[] highRam = new byte[0x7F];
        private byte interruptEnableRegister;

        public delegate void DivCounterReset();
        public DivCounterReset? DivResetEvent;

        public Memory(byte[]? cartridge = null, byte[]? bootRom = null)
        {
            this.bootRom = bootRom;
            this.cartridge = CartridgeCreator.GetCartridge(cartridge);

            RomMapMode = bootRom != null ? MapMode.Boot : MapMode.Cartridge;
        }

        public ushort ReadWord(ushort addr)
        {
            var lsb = ReadByte(addr);
            var msb = ReadByte((ushort)(addr + 1));
            return BitUtils.BytesToUshort(msb, lsb);
        }

        public byte ReadByte(ushort addr)
        {
            if (addr < 0x8000)
            {
                if (RomMapMode == MapMode.Boot && addr < bootRom?.Length)
                {
                    return bootRom[addr];
                }
                else
                {
                    return cartridge.ReadByte(addr);
                }
            }
            else if (addr < 0xA000)
            {
                return videoRam[addr - 0x8000];
            }
            else if (addr < 0xC000)
            {
                return cartridge.ReadByte(addr);
            }
            else if (addr < 0xE000)
            {
                return workRam[addr - 0xC000];
            }
            else if (addr < 0xFE00)
            {
                // Echo of internal RAM
                return workRam[addr - 0xE000];
            }
            else if (addr < 0xFEA0)
            {
                return oam[addr - 0xFE00];
            }
            else if (addr < 0xFF00)
            {
                // Empty but unusable for I/O, return default bus value 0xFF
                return 0xFF;
            }
            else if (addr < 0xFF4C)
            {
                if (addr == 0xFF00)
                {
                    ioRegisters[0] = inputRegister.ReadValue(ioRegisters[0]);
                }
                return ioRegisters[addr - 0xFF00];
            }
            else if (addr < 0xFF80)
            {
                // Empty but unusable for I/O, return default bus value 0xFF
                return 0xFF;
            }
            else if (addr < 0xFFFF)
            {
                return highRam[addr - 0xFF80];
            }
            else if (addr == 0xFFFF)
            {
                return interruptEnableRegister;
            }
            else
            {
                throw new ArgumentException($"Could not read from illegal memory address: {addr:X4}");
            }
        }

        public void WriteWord(ushort addr, ushort data)
        {
            byte msb = BitUtils.MostSignificantByte(data);
            byte lsb = BitUtils.LeastSignificantByte(data);
            WriteByte(addr, lsb);
            WriteByte((ushort)(addr + 1), msb);
        }

        public void WriteByte(ushort addr, byte data, bool applySideEffects = true)
        {
            if (addr < 0x8000)
            {
                cartridge.WriteByte(addr, data);
            }
            else if (addr < 0xA000)
            {
                byte lcdControl = ioRegisters[0xFF40 - 0xFF00];
                int lcdMode = ioRegisters[0xFF41 - 0xFF00] & 3;
                if (!lcdControl.IsBitSet(7) || lcdMode != (int)PPU.Mode.PixelTransfer)
                {
                    // VRAM is not accessible during pixel transfer if lcd is enabled
                    videoRam[addr - 0x8000] = data;
                }
            }
            else if (addr < 0xC000)
            {
                cartridge.WriteByte(addr, data);
            }
            else if (addr < 0xE000)
            {
                workRam[addr - 0xC000] = data;
            }
            else if (addr < 0xFE00)
            {
                // Echo of internal RAM
                workRam[addr - 0xE000] = data;
            }
            else if (addr < 0xFEA0)
            {
                byte lcdControl = ioRegisters[0xFF40 - 0xFF00];
                int lcdMode = ioRegisters[0xFF41 - 0xFF00] & 3;
                if (!lcdControl.IsBitSet(7) || (lcdMode != (int)PPU.Mode.OamSearch && lcdMode != (int)PPU.Mode.PixelTransfer))
                {
                    // OAM not accessible during OAM search and pixel transfer
                    oam[addr - 0xFE00] = data;
                }
            }
            else if (addr < 0xFF00)
            {
                // Empty but unusable for I/O
            }
            else if (addr < 0xFF4C)
            {
                if (addr == 0xFF04 && applySideEffects)
                {
                    data = 0x00;
                    DivResetEvent?.Invoke();
                }

                if (addr == 0xFF02 && data == 0x81 && applySideEffects)
                {
                    Serial.Add(ioRegisters[1]);
                }

                if (addr == 0xFF00 && applySideEffects)
                {
                    data = (byte)((ioRegisters[0] & 0xF) | (data & 0x30));
                }
                if (addr == 0xFF46 && applySideEffects)
                {
                    LaunchDMATransfer(data);
                }
                ioRegisters[addr - 0xFF00] = data;
            }
            else if (addr < 0xFF80)
            {
                if (addr == BOOT_ROM_LOCK_ADDRESS && data == 0b1 && RomMapMode == MapMode.Boot && applySideEffects)
                {
                    RomMapMode = MapMode.Cartridge;
                }
                else
                {
                    // Empty but unusable for I/O
                }
            }
            else if (addr < 0xFFFF)
            {
                highRam[addr - 0xFF80] = data;
            }
            else if (addr == 0xFFFF)
            {
                interruptEnableRegister = data;
            }
            else
            {
                throw new ArgumentException($"Could not write to illegal memory address: {addr:X4}");
            }
        }

        public void UpdateInputRegister(InputRegister.Keys keys)
        {
            inputRegister.UpdateState(keys, this);
        }

        private void LaunchDMATransfer(byte registerValue)
        {
            for (ushort i = 0; i < 0xA0; i++)
            {
                ushort addr = (ushort)((registerValue << 8) + i);
                byte data = ReadByte(addr);
                WriteByte((ushort)(0xFE00 + i), data);

            }
        }
    }
}
