namespace GeemuBoy
{
    class Program
    {
        private static readonly string romFolderPath = "C:/repos/GeemuBoy/GeemuBoy/Roms";

        static void Main(string[] args)
        {
            //string cartridgePath = $"{romFolderPath}/Game.gb";
            //string cartridgePath = $"{romFolderPath}/04-op r,imm.gb";
            string cartridgePath = $"{romFolderPath}/03-op sp,hl.gb";
            string bootRomPath = $"{romFolderPath}/Boot.bin";
            new EmulatorSDL(cartridgePath, null);
        }
    }
}
