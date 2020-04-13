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
        private int CurrentLine
        {
            get { return currentLine; }
            set
            {
                currentLine = value;
                memory.WriteByte(0xFF44, (byte)currentLine);
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
                        if (CurrentLine == 144)
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
            //if (currentLine % 2 == 0)
            //{
            //    line = Enumerable.Repeat(0xFF000000, 160).ToArray();
            //    line = Enumerable.Range(0, 160).Select(v => v % 2 == 0 ? 0xFF000000 : 0xFF00FF00).ToArray();
            //}
            //else
            //{
            //    line = Enumerable.Repeat(0xFF00FF00, 160).ToArray();
            //    line = Enumerable.Range(0, 160).Select(v => v % 2 == 0 ? 0xFF00FF00 : 0xFF000000).ToArray();
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

            bool windowEnabled = controlRegister.IsBitSet(5);
        }

        private void RenderSpriteLine(byte controlRegister)
        {

        }
    }
}
