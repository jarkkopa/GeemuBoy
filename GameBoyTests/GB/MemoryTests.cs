using Xunit;
using GameBoy.GB;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace GameBoy.GB.Tests
{
    public class MemoryTests
    {
        [Fact()]
        public void RomReadWriteTest()
        {
            var memory = new Memory(new byte[0x80], new byte[0x4000], Memory.MapMode.Cartridge);
            ushort addrStart = 0x0000;
            ushort addrEnd = 0x3FFF;

            memory.WriteByte(addrStart, 0xFF, true);
            memory.WriteByte(addrEnd, 0xFF, true);

            Assert.Equal(0xFF, memory.ReadByte(addrStart));
            Assert.Equal(0xFF, memory.ReadByte(addrEnd));
        }

        [Fact]
        public void RomBankReadWriteTest()
        {
            var memory = new Memory(new byte[0x80], new byte[0x4000]);
            ushort addrStart = 0x4000;
            ushort addrEnd = 0x7FFF;

            memory.WriteByte(addrStart, 0xFF, true);
            memory.WriteByte(addrEnd, 0xFF, true);

            Assert.Equal(0xFF, memory.ReadByte(addrStart));
            Assert.Equal(0xFF, memory.ReadByte(addrEnd));
        }

        [Fact]
        public void VideoRamReadWriteTest()
        {
            var memory = new Memory(new byte[0x80], new byte[0x4000]);
            ushort addrStart = 0x8000;
            ushort addrEnd = 0x9FFF;

            memory.WriteByte(addrStart, 0xFF);
            memory.WriteByte(addrEnd, 0xFF);

            Assert.Equal(0xFF, memory.ReadByte(addrStart));
            Assert.Equal(0xFF, memory.ReadByte(addrEnd));
        }

        [Fact]
        public void RamBankReadWriteTest()
        {
            var memory = new Memory(new byte[0x80], new byte[0x4000]);
            ushort addrStart = 0xA000;
            ushort addrEnd = 0xBFFF;

            memory.WriteByte(addrStart, 0xFF);
            memory.WriteByte(addrEnd, 0xFF);

            Assert.Equal(0xFF, memory.ReadByte(addrStart));
            Assert.Equal(0xFF, memory.ReadByte(addrEnd));
        }

        [Fact]
        public void WorkRamReadWriteTest()
        {
            var memory = new Memory(new byte[0x80], new byte[0x4000]);
            ushort addrStart = 0xC000;
            ushort addrEnd = 0xDFFF;

            memory.WriteByte(addrStart, 0xFF);
            memory.WriteByte(addrEnd, 0xFF);

            Assert.Equal(0xFF, memory.ReadByte(addrStart));
            Assert.Equal(0xFF, memory.ReadByte(addrEnd));
        }

        [Fact]
        public void OamReadWriteTest()
        {
            var memory = new Memory(new byte[0x80], new byte[0x4000]);
            ushort addrStart = 0xFE00;
            ushort addrEnd = 0xFE9F;

            memory.WriteByte(addrStart, 0xFF);
            memory.WriteByte(addrEnd, 0xFF);

            Assert.Equal(0xFF, memory.ReadByte(addrStart));
            Assert.Equal(0xFF, memory.ReadByte(addrEnd));
        }

        [Fact]
        public void ioRegistersReadWriteTest()
        {
            var memory = new Memory(new byte[0x80], new byte[0x4000]);
            ushort addrStart = 0xFF00;
            ushort addrEnd = 0xFF4B;

            memory.WriteByte(addrStart, 0xFF);
            memory.WriteByte(addrEnd, 0xFF);

            Assert.Equal(0xFF, memory.ReadByte(addrStart));
            Assert.Equal(0xFF, memory.ReadByte(addrEnd));
        }

        [Fact]
        public void highRamReadWriteTest()
        {
            var memory = new Memory(new byte[0x80], new byte[0x4000]);
            ushort addrStart = 0xFF80;
            ushort addrEnd = 0xFFFE;

            memory.WriteByte(addrStart, 0xFF);
            memory.WriteByte(addrEnd, 0xFF);

            Assert.Equal(0xFF, memory.ReadByte(addrStart));
            Assert.Equal(0xFF, memory.ReadByte(addrEnd));
        }

        [Fact]
        public void InterruptEnableRegisterReadWriteTest()
        {
            var memory = new Memory(new byte[0x80], new byte[0x4000]);
            ushort addr = 0xFFFF;

            memory.WriteByte(addr, 0xFF);

            Assert.Equal(0xFF, memory.ReadByte(addr));
        }

        [Fact]
        public void SettingBootRomLockAddressShouldChangeRomMapModeToCartridge()
        {
            byte[] bootRom = Enumerable.Repeat((byte)0x10, 0x80).ToArray();
            byte[] cartridge = Enumerable.Repeat((byte)0xFF, 0x4000).ToArray();

            var memory = new Memory(bootRom, cartridge, Memory.MapMode.Boot);

            // Should read from boot rom
            Assert.Equal(0x10, memory.ReadByte(0x7F));

            memory.WriteByte(0xFF50, 0x01);

            Assert.Equal(Memory.MapMode.Cartridge, memory.RomMapMode);
            // Should now read from cartridge
            Assert.Equal(0xFF, memory.ReadByte(0x7F));
        }

    }
}