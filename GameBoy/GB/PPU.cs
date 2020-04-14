using System.Linq;

namespace GameBoy.GB
{
    public class PPU
    {
        public enum Mode
        {
            OamSearch,
            PixelTransfer,
            HBlank,
            VBlank
        }

        private const int WIDTH = 160;
        private const int HEIGHT = 144;

        private readonly Memory memory;
        private readonly IDisplay display;

        public Mode CurrentMode { get; private set; } = Mode.OamSearch;
        private int cycles = 0;
        private byte currentLine = 0;
        private byte CurrentLine
        {
            get { return currentLine; }
            set
            {
                currentLine = value;
                memory.WriteByte(0xFF44, currentLine);
            }
        }

        public PPU(Memory memory, IDisplay display)
        {
            this.memory = memory;
            this.display = display;
        }

        public void Tick(int cpuCycles)
        {
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

                        RenderScanLine();
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

        private void RenderScanLine()
        {
            //uint[] line;
            //if (currentLine<70)
            //{
            //    //line = Enumerable.Repeat(0xFF000000, 160).ToArray();
            //    line = Enumerable.Range(0, 160).Select(v => v < 80  ? 0xFF000000 : 0xFFFFFFFF).ToArray();
            //}
            //else
            //{
            //    line = Enumerable.Repeat(0xFFFFFFFF, 160).ToArray();
            //    //line = Enumerable.Range(0, 160).Select(v => v %  ? 0xFF00FF00 : 0xFF000000).ToArray();
            //}
            //display.RenderLine(currentLine, line);
            //return;
            byte controlRegister = memory.ReadByte(0xFF40);
            if (controlRegister.IsBitSet(7) == false)
            {
                return;
            }

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
            int tileY = y / 8;
            uint[] linePixels = new uint[160];
            for (int pixel = 0; pixel < WIDTH; pixel++)
            {
                // Find out which tile this pixel belongs to
                int x = renderWindow && pixel >= windowX ? pixel - windowX : pixel + scrollX;
                int tileX = x / 8;
                ushort tileAddress = (ushort)(tileMapAddress + tileX + (tileY * 32));
                // Read tile data
                byte tileNumber = memory.ReadByte(tileAddress);
                // Find out tile address
                ushort tileDataAddress;
                if (controlRegister.IsBitSet(4))
                {
                    tileDataAddress = (ushort)(0x8000 + (tileNumber * 16));
                }
                else
                {
                    tileDataAddress = (ushort)(0x8800 + (((sbyte)tileNumber) * 16));
                }

                // Find out which tile line are we in
                int tileLine = (y % 8) * 2;
                tileDataAddress = (ushort)(tileDataAddress + tileLine);
                // Fetch pixels from current line
                byte tileLow = memory.ReadByte(tileDataAddress);
                byte tileHigh = memory.ReadByte((ushort)(tileDataAddress + 1));

                linePixels[pixel] = GetPixel((7 - (x % 8)), 0xFF, tileHigh, tileLow);
            }
            display.RenderLine(CurrentLine, linePixels);
        }

        private void RenderSpriteLine(byte controlRegister)
        {

        }

        private uint GetPixel(int index, byte palette, byte high, byte low)
        {
            int colorH = (high & 1 << index) >> (index - 1);
            int colorL = (low & 1 << index) >> index;
            int color = colorH | colorL;

            return GetColor(color);
        }

        private uint GetColor(int index)
        {
            switch (index)
            {
                case 0:
                    return 0xFF000000;
                case 1:
                    return 0xFF606060;
                case 2:
                    return 0xFFA0A0A0;
                case 3:
                    return 0xFFFFFFFF;
            }
            return 0xFFFFFFFF;
        }
    }
}
