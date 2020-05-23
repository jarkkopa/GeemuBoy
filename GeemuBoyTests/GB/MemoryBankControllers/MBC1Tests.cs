using Xunit;

namespace GeemuBoy.GB.MemoryBankControllers.Tests
{
    public class MBC1Tests
    {
        [Fact()]
        public void RomBanksCanBeSwitched()
        {
            byte[] cartridge = new byte[0x200000];
            cartridge[0x147] = 0x2; // MBC1 + RAM
            cartridge[0x148] = 0x6; // MBC1 max ROM 2MB
            cartridge[0x149] = 0x3; // MBC1 max RAM 32kB

            cartridge[0x0] = 0xF0;
            cartridge[0x3FFF] = 0xF0;

            ushort bankSize = 0x4000;
            for (byte i = 0x1; i <= 0x7F; i++)
            {
                cartridge[i * bankSize] = i;
                cartridge[(i + 1) * bankSize - 1] = i;
            }

            var memory = new Memory(cartridge);

            Assert.Equal(0xF0, memory.ReadByte(0x0));
            Assert.Equal(0xF0, memory.ReadByte(0x3FFF));

            // Bank #1 selected by default
            Assert.Equal(0x01, memory.ReadByte(0x4000));
            Assert.Equal(0x01, memory.ReadByte(0x7FFF));

            // External bank #0 should be mapped to #1
            memory.WriteByte(0x2000, 0x0);
            Assert.Equal(0x01, memory.ReadByte(0x4000));
            Assert.Equal(0x01, memory.ReadByte(0x7FFF));

            for (byte i = 0x1; i <= 0x1F; i++)
            {
                memory.WriteByte(0x2000, i);

                Assert.Equal(i, memory.ReadByte(0x4000));
                Assert.Equal(i, memory.ReadByte(0x7FFF));
            }

            // Rom bank select mode
            memory.WriteByte(0x6000, 0x0);

            memory.WriteByte(0x2000, 0x1);
            memory.WriteByte(0x4000, 0x1);
            // Bank #0x20 maps to #0x21
            Assert.Equal(0x21, memory.ReadByte(0x4000));
            Assert.Equal(0x21, memory.ReadByte(0x7FFF));

            for (byte i = 0x21; i <= 0x3F; i++)
            {
                memory.WriteByte(0x2000, i);

                Assert.Equal(i, memory.ReadByte(0x4000));
                Assert.Equal(i, memory.ReadByte(0x7FFF));
            }

            memory.WriteByte(0x2000, 0x1);
            memory.WriteByte(0x4000, 0x2);
            // Bank #0x40 maps to #0x41
            Assert.Equal(0x41, memory.ReadByte(0x4000));
            Assert.Equal(0x41, memory.ReadByte(0x7FFF));

            for (byte i = 0x41; i <= 0x5F; i++)
            {
                memory.WriteByte(0x2000, i);

                Assert.Equal(i, memory.ReadByte(0x4000));
                Assert.Equal(i, memory.ReadByte(0x7FFF));
            }

            memory.WriteByte(0x2000, 0x1);
            memory.WriteByte(0x4000, 0x3);
            // Bank #0x60 maps to #0x61
            Assert.Equal(0x61, memory.ReadByte(0x4000));
            Assert.Equal(0x61, memory.ReadByte(0x7FFF));

            for (byte i = 0x61; i <= 0x7F; i++)
            {
                memory.WriteByte(0x2000, i);

                Assert.Equal(i, memory.ReadByte(0x4000));
                Assert.Equal(i, memory.ReadByte(0x7FFF));
            }
        }

        [Fact()]
        public void RamBankCanBeSwitched()
        {
            byte[] cartridge = new byte[0x200000];
            cartridge[0x147] = 0x2;
            cartridge[0x148] = 0x6;
            cartridge[0x149] = 0x3;
            var memory = new Memory(cartridge);

            //Enable RAM and RAM switch mode
            memory.WriteByte(0x0, 0x0A);
            memory.WriteByte(0x6000, 0x1);

            memory.WriteByte(0xA000, 0xF0);
            memory.WriteByte(0xBFFF, 0xF0);

            memory.WriteByte(0x4000, 0x01);
            memory.WriteByte(0xA000, 0x01);
            memory.WriteByte(0xBFFF, 0x01);

            memory.WriteByte(0x4000, 0x02);
            memory.WriteByte(0xA000, 0x02);
            memory.WriteByte(0xBFFF, 0x02);

            memory.WriteByte(0x4000, 0x03);
            memory.WriteByte(0xA000, 0x03);
            memory.WriteByte(0xBFFF, 0x03);

            memory.WriteByte(0x4000, 0x00);
            Assert.Equal(0xF0, memory.ReadByte(0xA000));
            Assert.Equal(0xF0, memory.ReadByte(0xBFFF));

            memory.WriteByte(0x4000, 0x01);
            Assert.Equal(0x01, memory.ReadByte(0xA000));
            Assert.Equal(0x01, memory.ReadByte(0xBFFF));

            memory.WriteByte(0x4000, 0x02);
            Assert.Equal(0x02, memory.ReadByte(0xA000));
            Assert.Equal(0x02, memory.ReadByte(0xBFFF));

            memory.WriteByte(0x4000, 0x03);
            Assert.Equal(0x03, memory.ReadByte(0xA000));
            Assert.Equal(0x03, memory.ReadByte(0xBFFF));
        }

        [Fact()]
        public void RomAndRamSelectModeCanBeChanged()
        {
            byte[] cartridge = new byte[0x200000];
            cartridge[0x147] = 0x2;
            cartridge[0x148] = 0x6;
            cartridge[0x149] = 0x3;

            cartridge[0x4000] = 0x01;
            cartridge[0x7FFF] = 0x01;
            cartridge[0x4000 * 0x21] = 0x21;
            cartridge[0x4000 * 0x21 + 0x3FFF] = 0x21;

            var memory = new Memory(cartridge);

            // Enable RAM
            memory.WriteByte(0x0, 0x0A);

            // RAM bank #0
            memory.WriteByte(0xA000, 0xF0);
            memory.WriteByte(0xBFFF, 0xF0);

            // Select ROM bank #1
            memory.WriteByte(0x2000, 0x01);

            Assert.Equal(0x01, memory.ReadByte(0x4000));
            Assert.Equal(0x01, memory.ReadByte(0x7FFF));

            // Switch to RAM select and select bank #1.
            memory.WriteByte(0x6000, 0x01);
            memory.WriteByte(0x4000, 0x01);

            memory.WriteByte(0xA000, 0x01);
            memory.WriteByte(0xBFFF, 0x01);
            Assert.Equal(0x01, memory.ReadByte(0xA000));
            Assert.Equal(0x01, memory.ReadByte(0xBFFF));
            Assert.Equal(0x01, memory.ReadByte(0x4000));
            Assert.Equal(0x01, memory.ReadByte(0x7FFF));

            // Switch to RAM select and select #0x21 by writing to upper two bits
            memory.WriteByte(0x6000, 0x00);
            memory.WriteByte(0x4000, 0x01);
            // ROM bank #0x21 selected, RAM bank is default #0
            Assert.Equal(0x21, memory.ReadByte(0x4000));
            Assert.Equal(0x21, memory.ReadByte(0x7FFF));
            Assert.Equal(0xF0, memory.ReadByte(0xA000));
            Assert.Equal(0xF0, memory.ReadByte(0xBFFF));

            // Switch back to RAM select and RAM #1 and ROM #1 should be enabled
            memory.WriteByte(0x6000, 0x01);
            Assert.Equal(0x01, memory.ReadByte(0x4000));
            Assert.Equal(0x01, memory.ReadByte(0x7FFF));
            Assert.Equal(0x01, memory.ReadByte(0xA000));
            Assert.Equal(0x01, memory.ReadByte(0xBFFF));
        }
    }
}