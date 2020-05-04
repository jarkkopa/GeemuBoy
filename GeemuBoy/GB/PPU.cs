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

        private const ushort LCD_STAT_ADDR = 0xFF41;

        private const int WIDTH = 160;
        private const int HEIGHT = 144;

        private readonly Memory memory;
        private readonly IDisplay display;

        private Mode currentMode;

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

                            display.Render();

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
                // Render background
                RenderBackgroundLine(controlRegister);
            }

            if (controlRegister.IsBitSet(1))
            {
                // Render sprites
                RenderSpriteLine(controlRegister);
            }
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
            uint[] linePixels = new uint[160];
            for (int pixel = 0; pixel < WIDTH; pixel++)
            {
                // Find out which tile this pixel belongs to
                int x = renderWindow && pixel >= windowX ? pixel - windowX : pixel + scrollX;
                int tileX = x / 8;
                ushort tileAddress = (ushort)(tileMapAddress + tileX + (tileY * 32));
                byte tileNumber = memory.ReadByte(tileAddress);
                // Find out tile address
                ushort tileDataAddress;
                if (controlRegister.IsBitSet(4))
                {
                    tileDataAddress = (ushort)(0x8000 + (tileNumber * 16));
                }
                else
                {
                    // 0x8800 addressing mode uses signed tile numbers unlike 0x8000 address mode
                    tileDataAddress = (ushort)(0x8800 + (((sbyte)tileNumber) * 16));
                }

                // Find out which tile line are we in
                int tileLine = (y % 8) * 2;
                tileDataAddress = (ushort)(tileDataAddress + tileLine);
                // Fetch pixels from current line
                byte tileLow = memory.ReadByte(tileDataAddress);
                byte tileHigh = memory.ReadByte((ushort)(tileDataAddress + 1));

                linePixels[pixel] = GetPixel((7 - (x % 8)), memory.ReadByte(0xFF47), tileHigh, tileLow);
            }
            display.RenderLine(CurrentLine, linePixels);
        }

        private void RenderSpriteLine(byte controlRegister)
        {
            // TODO
        }

        private uint GetPixel(int index, byte palette, byte high, byte low)
        {
            int colorH = (high & 1 << index) >> (index - 1);
            int colorL = (low & 1 << index) >> index;
            int color = colorH | colorL;

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
                3 => 0xFF000000,
                2 => 0xFF606060,
                1 => 0xFFA0A0A0,
                0 => 0xFFFFFFFF,
                _ => 0xFFFFFFFF
            };
    }
}
