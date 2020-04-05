namespace GameBoy.GB
{
    public class PPU
    {
        private enum Mode
        {
            OAM,
            PixelTransfer,
            HBlank,
            VBlank
        }

        private readonly Memory memory;

        private Mode mode = Mode.OAM;
        private int cycles = 0;
        private int currentLine = 0;

        public PPU(Memory memory)
        {
            this.memory = memory;
        }

        public void Tick(int cpuCycles)
        {
            cycles += cpuCycles;
            switch (mode)
            {
                case Mode.OAM:
                    if (cycles >= 80)
                    {
                        mode = Mode.PixelTransfer;
                        cycles -= 80;
                    }
                    break;

                case Mode.PixelTransfer:
                    if (cycles >= 172)
                    {
                        mode = Mode.HBlank;
                        cycles -= 172;
                    }
                    break;

                case Mode.HBlank:
                    if (cycles >= 204)
                    {
                        currentLine++;
                        cycles -= 204;
                        if (currentLine > 144)
                        {
                            mode = Mode.VBlank;
                        }
                    }
                    break;

                case Mode.VBlank:
                    if (cycles >= 4560)
                    {
                        cycles -= 4560;
                        mode = Mode.OAM;
                    }
                    break;
            }
        }
    }
}
