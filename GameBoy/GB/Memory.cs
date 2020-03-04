using System;

namespace GameBoy.GB
{
    public class Memory
    {
        public enum State { Boot, Normal }

        private const ushort BOOT_ROM_LOCK_ADDRESS = 0xFF50;

        private State state;

        private byte[] bootRom;
        private byte[] cartridge;

        /// <summary>
        /// ROM bank #0
        /// Address: 0000-3FFF
        /// 16 kB
        /// </summary>
        private byte[] rom = new byte[0x4000];
        /// <summary>
        /// ROM bank 01-0F
        /// Address: 4000-7FFF
        /// 16 kB
        /// </summary>
        private byte[] romBank = new byte[0x4000];
        /// <summary>
        /// Video RAM
        /// Address: 8000-9FFF
        /// 8 kB
        /// </summary>
        private byte[] videoRam = new byte[0x2000];
        /// <summary>
        /// External switchable RAM bank
        /// Address: A000-BFFF
        /// 8 kB
        /// </summary>
        private byte[] ramBank = new byte[0x2000];
        /// <summary>
        /// Work RAM bank 0
        /// Address: C000-DFFF
        /// 8 kB
        /// </summary>
        private byte[] workRam = new byte[0x2000];
        /// <summary>
        /// Sprite attribute table (OAM)
        /// Address: FE00-FE9F
        /// 160 B
        /// </summary>
        private byte[] oam = new byte[0xA0];
        /// <summary>
        /// I/O Registers
        /// Address: FF00-FF7F
        /// 128 B
        /// </summary>
        private byte[] ioRegisters = new byte[0x80];
        /// <summary>
        /// High RAM
        /// Address: FF80-FFFE
        /// 127 B
        /// </summary>
        private byte[] highRam = new byte[0x7F];
        /// <summary>
        /// Interrupts Enable register
        /// Address: FFFF-FFFF
        /// 1 B
        /// </summary>
        private byte interruptEnableRegister;

        public Memory(byte[] bootRom, byte[] cartridge, State initialState = State.Boot)
        {
            this.bootRom = bootRom;
            this.cartridge = cartridge;

            state = initialState;

            for (ushort i = 0; i < cartridge.Length; i++)
            {
                WriteByte(i, cartridge[i], true);
            }
        }

        public byte ReadByte(ushort addr)
        {
            if (addr < 0x4000)
            {
                if (state == State.Boot)
                {
                    return bootRom[addr];
                }
                else
                {
                    return rom[addr];
                }
            }
            else if (addr < 0x8000)
            {
                return romBank[addr - 0x4000];
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
                // Empty but unusable for I/O
                return 0;
            }
            else if (addr < 0xFF4C)
            {
                return ioRegisters[addr - 0xFF00];
            }
            else if (addr < 0xFF80)
            {
                // Empty but unusable for I/O
                return 0;
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
                throw new ArgumentException($"Could not read from illegal Game Boy memory address: {addr}");
            }
        }

        public void WriteByte(ushort addr, byte data)
        {
            WriteByte(addr, data, false);
        }

        private void WriteByte(ushort addr, byte data, bool canWriteRom)
        {
            if (addr < 0x4000 && canWriteRom)
            {
                rom[addr] = data;
            }
            else if (addr < 0x8000 && canWriteRom)
            {
                romBank[addr - 0x4000] = data;
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
                throw new ArgumentException($"Could not write to unusable Game Boy memory address: {addr}");
            }
            else if (addr < 0xFF4C)
            {
                ioRegisters[addr - 0xFF00] = data;
            }
            else if (addr < 0xFF80)
            {
                // Empty but unusable for I/O
                throw new ArgumentException($"Could not write to unusable Game Boy memory address: {addr}");
            }
            else if (addr < 0xFFFF)
            {
                highRam[addr - 0xFF80] = data;
                if (addr == BOOT_ROM_LOCK_ADDRESS && data == 0b1 && state == State.Boot)
                {
                    state = State.Normal;
                }
            }
            else if (addr == 0xFFFF)
            {
                interruptEnableRegister = data;
            }
            else
            {
                throw new ArgumentException($"Could not write to illegal Game Boy memory address: {addr}");
            }
        }
    }
}
