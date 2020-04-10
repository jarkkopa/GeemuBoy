namespace GameBoy
{
    class Program
    {
        private static readonly string romFolderPath = "C:/repos/GameBoy/GameBoy/Roms";

        static void Main(string[] args)
        {
            //string cartridgePath = "Roms/cpu_instrs.gb";
            string cartridgePath = $"{romFolderPath}/06-ld r,r.gb";
            //string cartridgePath = "Roms/01-special.gb";
            string bootRomPath = $"{romFolderPath }/DMG_ROM.bin";
            new Emulator(cartridgePath, bootRomPath);
            //new EmulatorSDL(cartridgePath, bootRomPath);
        }
    }
}
