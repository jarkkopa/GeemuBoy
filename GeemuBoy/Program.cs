namespace GeemuBoy
{
    class Program
    {
        private static readonly string romFolderPath = "C:/repos/GeemuBoy/GeemuBoy/Roms";

        static void Main(string[] args)
        {
            string cartridgePath = $"{romFolderPath}/Game.gb";
            //string cartridgePath = $"{romFolderPath}/test/acceptance/ppu/hblank_ly_scx_timing-GS.gb";
            //string cartridgePath = $"{romFolderPath}/03-op sp,hl.gb";
            string bootRomPath = $"{romFolderPath}/Boot.bin";
            new EmulatorSDL(cartridgePath, null);
        }
    }
}
