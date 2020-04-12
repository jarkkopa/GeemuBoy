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

        private readonly Memory memory;
        private readonly IDisplay display;

        public Mode CurrentMode { get; private set; } = Mode.OamSearch;
        private int cycles = 0;
        private int currentLine = 0;

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
                        // Write scanline
                        uint[] line;
                        if(currentLine%2==0)
                        {
                            line = Enumerable.Repeat(0xFF000000, 160).ToArray();
                        } else
                        {
                            line = Enumerable.Repeat(0xFF00FF00, 160).ToArray();
                        }
                        display.DrawLine(currentLine, line);
                    }
                    break;

                case Mode.HBlank:
                    if (cycles >= 204)
                    {
                        currentLine++;
                        cycles -= 204;
                        if (currentLine == 143)
                        {
                            CurrentMode = Mode.VBlank;
                            // Render picture
                            display.Render();
                        }
                        else
                        {
                            CurrentMode = Mode.OamSearch;
                        }
                    }
                    break;

                case Mode.VBlank:
                    if (cycles >= 4560)
                    {
                        cycles -= 4560;
                        CurrentMode = Mode.OamSearch;
                        currentLine = 0;
                    }
                    break;
            }
        }
    }
}
