using System;
using System.Text;

namespace GeemuBoy.GB
{
    public class Memory
    {
        public enum MapMode { Boot, Cartridge }

        public const ushort MAX_ADDR = 0xFFFF;

        private const ushort BOOT_ROM_LOCK_ADDRESS = 0xFF50;

        public MapMode RomMapMode { get; private set; }

        private readonly byte[]? bootRom;
        private readonly byte[] cartridge;

        /// <summary>
        /// Video RAM
        /// Address: 8000-9FFF
        /// 8 kB
        /// </summary>
        private readonly byte[] videoRam = new byte[0x2000];
        /// <summary>
        /// External switchable RAM bank
        /// Address: A000-BFFF
        /// 8 kB
        /// </summary>
        private readonly byte[] ramBank = new byte[0x2000];
        /// <summary>
        /// Work RAM bank 0
        /// Address: C000-DFFF
        /// 8 kB
        /// </summary>
        private readonly byte[] workRam = new byte[0x2000];
        /// <summary>
        /// Sprite attribute table (OAM)
        /// Address: FE00-FE9F
        /// 160 B
        /// </summary>
        private readonly byte[] oam = new byte[0xA0];
        /// <summary>
        /// I/O Registers
        /// Address: FF00-FF4B
        /// 128 B
        /// </summary>
        private readonly byte[] ioRegisters = new byte[0x80];
        /// <summary>
        /// High RAM
        /// Address: FF80-FFFE
        /// 127 B
        /// </summary>
        private readonly byte[] highRam = new byte[0x7F];
        /// <summary>
        /// Interrupts Enable register
        /// Address: FFFF-FFFF
        /// 1 B
        /// </summary>
        private byte interruptEnableRegister;

        public StringBuilder Serial { get; private set; }

        public Memory(byte[]? cartridge = null, byte[]? bootRom = null)
        {
            this.cartridge = cartridge ?? new byte[0x8000];
            this.bootRom = bootRom;

            RomMapMode = bootRom != null ? MapMode.Boot : MapMode.Cartridge;

            Serial = new StringBuilder();
        }

        public ushort ReadWord(ushort addr)
        {
            var lsb = ReadByte(addr);
            var msb = ReadByte((ushort)(addr + 1));
            return BitUtils.BytesToUshort(msb, lsb);
        }

        public byte ReadByte(ushort addr)
        {
            if (addr < 0x4000)
            {
                if (RomMapMode == MapMode.Boot && addr < bootRom?.Length)
                {
                    return bootRom[addr];
                }
                else
                {
                    return cartridge[addr];
                }
            }
            else if (addr < 0x8000)
            {
                return cartridge[addr];
            }
            else if (addr < 0xA000)
            {
                return videoRam[addr - 0x8000];
            }
            else if (addr < 0xC000)
            {
                return ramBank[addr - 0xA000];
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

        public void WriteByte(ushort addr, byte data)
        {
            if (addr < 0x4000)
            {
                // throw new ArgumentException($"Could not write to read only memory address: {addr:x4}");
            }
            else if (addr < 0x8000)
            {
                // throw new ArgumentException($"Could not write to read only memory address: {addr}");
            }
            else if (addr < 0xA000)
            {
                videoRam[addr - 0x8000] = data;
            }
            else if (addr < 0xC000)
            {
                ramBank[addr - 0xA000] = data;
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
                oam[addr - 0xFE00] = data;
            }
            else if (addr < 0xFF00)
            {
                // Empty but unusable for I/O
                //throw new ArgumentException($"Could not write to unusable memory address: {addr}");
            }
            else if (addr < 0xFF4C)
            {
                ioRegisters[addr - 0xFF00] = data;

                if (addr == 0xFF02 && data == 0x81)
                {
                    Serial.Append((char)ioRegisters[1]);
                }
            }
            else if (addr < 0xFF80)
            {
                if (addr == BOOT_ROM_LOCK_ADDRESS && data == 0b1 && RomMapMode == MapMode.Boot)
                {
                    RomMapMode = MapMode.Cartridge;
                }
                else
                {
                    // Empty but unusable for I/O
                    //throw new ArgumentException($"Could not write to unusable memory address: 0x{addr:X4}");
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
    }
}
