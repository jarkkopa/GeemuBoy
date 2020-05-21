using System;

namespace GeemuBoy.GB
{
    public class PPU
    {
        public enum Mode : byte
        {
            OamSearch = 2,
            PixelTransfer = 3,
            HBlank = 0,
            VBlank = 1
        }

        private const uint BLACK = 0xFF000000;
        private const uint DARK_GRAY = 0xFF606060;
        private const uint LIGHT_GRAY = 0xFFA0A0A0;
        private const uint WHITE = 0xFFFFFFFF;

        private const ushort LCD_STAT_ADDR = 0xFF41;

        private const int WIDTH = 160;
        private const int HEIGHT = 144;

        private readonly Memory memory;
        private readonly IDisplay display;

        private Mode currentMode;

        private readonly uint[] currentDrawLine = new uint[WIDTH];

        public Mode CurrentMode
        {
            get { return currentMode; }
            private set
            {
                currentMode = value;
                byte lcdStat = memory.ReadByte(LCD_STAT_ADDR);
                lcdStat = (byte)((lcdStat & 0xFC) | (byte)currentMode);
                memory.WriteByte(LCD_STAT_ADDR, lcdStat);

                if (currentMode == Mode.PixelTransfer)
                {
                    // Pixel transfer doesn't cause LCD STAT interrupt
                    return;
                }

                var flagIndex = currentMode switch
                {
                    Mode.HBlank => 3,
                    Mode.OamSearch => 5,
                    Mode.VBlank => 4,
                    _ => throw new System.Exception("Invalid PPU mode")
                };

                if (lcdStat.IsBitSet(flagIndex))
                {
                    CPU.RequestInterrupt(memory, CPU.Interrupt.LCDStat);
                }
            }
        }
        private int cycles = 0;
        private byte currentLine = 0;
        private byte CurrentLine
        {
            get { return currentLine; }
            set
            {
                currentLine = value;
                memory.WriteByte(0xFF44, currentLine);

                // Set coincidence flag
                byte lcdStat = memory.ReadByte(LCD_STAT_ADDR);
                lcdStat = BitUtils.SetBit(lcdStat, 2, currentLine == memory.ReadByte(0xFF45));
                memory.WriteByte(LCD_STAT_ADDR, lcdStat);

                if (lcdStat.IsBitSet(6))
                {
                    CPU.RequestInterrupt(memory, CPU.Interrupt.LCDStat);
                }
            }
        }

        public delegate void RenderHandler();
        public RenderHandler? RenderEvent;

        public PPU(Memory memory, IDisplay display)
        {
            this.memory = memory;
            this.display = display;

            CurrentMode = Mode.OamSearch;
        }

        public void Update(int cpuCycles)
        {
            byte controlRegister = memory.ReadByte(0xFF40);
            if (controlRegister.IsBitSet(7) == false)
            {
                // Disabling LCD can be only done during VBlank
                CurrentMode = Mode.VBlank;
                cycles = 456;
                CurrentLine = 0;
                return;
            }

            cycles += cpuCycles;
            switch (CurrentMode)
            {
                case Mode.OamSearch:
                    if (cycles >= 80)
                    {
                        CurrentMode = Mode.PixelTransfer;
                        cycles -= 80;
                    }
                    break;

                case Mode.PixelTransfer:
                    if (cycles >= 172)
                    {
                        CurrentMode = Mode.HBlank;
                        cycles -= 172;

                        RenderScanLine(controlRegister);
                    }
                    break;

                case Mode.HBlank:
                    if (cycles >= 204)
                    {
                        CurrentLine++;

                        cycles -= 204;
                        if (CurrentLine == HEIGHT)
                        {
                            CurrentMode = Mode.VBlank;

                            CPU.RequestInterrupt(memory, CPU.Interrupt.VBlank);
                            RenderEvent?.Invoke();
                        }
                        else
                        {
                            CurrentMode = Mode.OamSearch;
                        }
                    }
                    break;

                case Mode.VBlank:
                    if (cycles >= 456)
                    {
                        // Not really rendering anything but the line counter keeps updating during the V-Blank period
                        CurrentLine++;
                    }

                    if (cycles >= 4560)
                    {
                        cycles -= 4560;
                        CurrentMode = Mode.OamSearch;
                        CurrentLine = 0;
                    }
                    break;
            }
        }

        private void RenderScanLine(byte controlRegister)
        {
            if (controlRegister.IsBitSet(0))
            {
                RenderBackgroundLine(controlRegister);
            }

            if (controlRegister.IsBitSet(1))
            {
                RenderSpriteLine(controlRegister);
            }

            display.RenderLine(CurrentLine, currentDrawLine);
        }

        private void RenderBackgroundLine(byte controlRegister)
        {
            int scrollY = memory.ReadByte(0xFF42);
            int scrollX = memory.ReadByte(0xFF43);
            int windowY = memory.ReadByte(0xFF4A);
            int windowX = memory.ReadByte(0xFF4B) - 7;

            bool renderWindow = controlRegister.IsBitSet(5) && windowY <= CurrentLine;

            ushort tileMapAddress = (ushort)(controlRegister.IsBitSet(renderWindow ? 6 : 3) ? 0x9C00 : 0x9800);

            int y = renderWindow ? CurrentLine - windowY : scrollY + CurrentLine;
            int tileY = (y / 8) % 32;
            for (int pixel = 0; pixel < WIDTH; pixel++)
            {
                // Find out which tile this pixel belongs to
                int x = renderWindow && pixel >= windowX ? pixel - windowX : pixel + scrollX;
                int tileX = x / 8;
                byte tileNumber = GetTileNumber(tileX, tileY, tileMapAddress);
                ushort tileDataAddress = GetTileDataAddress(tileNumber, controlRegister.IsBitSet(4));

                // Find out which tile line are we in
                int tileLine = (y % 8) * 2;
                tileDataAddress = (ushort)(tileDataAddress + tileLine);
                // Fetch pixels from current line
                byte tileLow = memory.ReadByte(tileDataAddress);
                byte tileHigh = memory.ReadByte((ushort)(tileDataAddress + 1));

                currentDrawLine[pixel] = GetPixel((7 - (x % 8)), memory.ReadByte(0xFF47), tileHigh, tileLow);
            }
        }

        private void RenderSpriteLine(byte controlRegister)
        {
            int spriteHeight = controlRegister.IsBitSet(2) ? 16 : 8;

            int spritesPerLine = 0;
            for (ushort oamAddr = 0xFE00; oamAddr < 0xFEA0; oamAddr += 4)
            {
                // Sprite position adjusted to screen coordinates
                byte spriteY = (byte)(memory.ReadByte(oamAddr) - 16);
                byte spriteX = (byte)(memory.ReadByte((ushort)(oamAddr + 1)) - 8);
                byte tileNum = memory.ReadByte((ushort)(oamAddr + 2));
                byte attributes = memory.ReadByte((ushort)(oamAddr + 3));

                if (CurrentLine < spriteY || CurrentLine >= spriteY + spriteHeight || spritesPerLine > 10)
                {
                    continue;
                }
                spritesPerLine++;

                int currentSpriteLine = CurrentLine - spriteY;
                if (attributes.IsBitSet(6))
                {
                    currentSpriteLine = spriteHeight - currentSpriteLine - 1;
                }

                ushort tileAddress = (ushort)(0x8000 + (tileNum * 16) + (currentSpriteLine * 2));
                byte high = memory.ReadByte(tileAddress);
                byte low = memory.ReadByte((ushort)(tileAddress + 1));

                uint[] pixels = new uint[8];
                byte palette = attributes.IsBitSet(4) ? memory.ReadByte(0xFF49) : memory.ReadByte(0xFF48);
                bool flipHorizontal = attributes.IsBitSet(5);
                for (int i = 0; i < 8; i++)
                {
                    int adjustedIndex = flipHorizontal ? i : 7 - i;
                    pixels[i] = GetPixel(adjustedIndex, palette, high, low);
                }

                int pixelIndex = 0;
                for (int x = spriteX; x < spriteX + 8 && x < WIDTH && pixelIndex < 8; x++, pixelIndex++)
                {
                    if (pixels[pixelIndex] == WHITE || (attributes.IsBitSet(7) && currentDrawLine[x] != WHITE))
                    {
                        continue;
                    }
                    currentDrawLine[x] = pixels[pixelIndex];
                }
            }
        }

        private uint GetPixel(int index, byte palette, byte high, byte low)
        {
            int colorH = (high & 1 << index) >> index;
            int colorL = (low & 1 << index) >> index;
            int color = (colorH << 1) | colorL;

            int paletteIndex = color switch
            {
                0 => (palette & 0x3),
                1 => (palette & 0xC) >> 2,
                2 => (palette & 0x30) >> 4,
                3 => (palette & 0xC0) >> 6,
                _ => 0
            };

            return GetColor(paletteIndex);
        }

        private uint GetColor(int index) =>
            index switch
            {
                3 => BLACK,
                2 => DARK_GRAY,
                1 => LIGHT_GRAY,
                0 => WHITE,
                _ => WHITE
            };

        private byte GetTileNumber(int column, int row, ushort tileMapAddress)
        {
            ushort tileAddress = (ushort)(tileMapAddress + column + (row * 32));
            return memory.ReadByte(tileAddress);
        }

        private ushort GetTileDataAddress(byte tileNumber, bool unsignedAddressingMode)
        {
            if (unsignedAddressingMode)
            {
                return (ushort)(0x8000 + (tileNumber * 16));
            }
            else
            {
                // 0x8800 addressing mode uses signed tile numbers and that's why +0x80
                return (ushort)(0x8800 + (((sbyte)tileNumber + 0x80) * 16));
            }
        }

        public void PrintBackgroundTileNumbers()
        {
            byte controlRegister = memory.ReadByte(0xFF40);
            int windowY = memory.ReadByte(0xFF4A);
            bool renderWindow = controlRegister.IsBitSet(5) && windowY <= CurrentLine;
            ushort mapAddr = (ushort)(controlRegister.IsBitSet(renderWindow ? 6 : 3) ? 0x9C00 : 0x9800);
            for (int y = 0; y < 0x11; y++)
            {
                for (int x = 0; x < 0x13; x++)
                {
                    Console.Write($" 0x{GetTileNumber(x, y, mapAddr):X2}");
                }
                Console.WriteLine("");
            }
        }

        public void PrintBackgroundTileAddresses()
        {
            byte controlRegister = memory.ReadByte(0xFF40);
            int windowY = memory.ReadByte(0xFF4A);
            bool renderWindow = controlRegister.IsBitSet(5) && windowY <= CurrentLine;
            ushort mapAddr = (ushort)(controlRegister.IsBitSet(renderWindow ? 6 : 3) ? 0x9C00 : 0x9800);
            for (int y = 0; y < 0x11; y++)
            {
                for (int x = 0; x < 0x13; x++)
                {
                    byte tileNumber = GetTileNumber(x, y, mapAddr);
                    ushort tileDataAddress = GetTileDataAddress(tileNumber, controlRegister.IsBitSet(4));
                    Console.Write($" 0x{tileDataAddress:X4}");
                }
                Console.WriteLine("");
            }
        }
    }
}
