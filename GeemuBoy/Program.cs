namespace GeemuBoy
{
    class Program
    {
        private static readonly string romFolderPath = "C:/repos/GeemuBoy/GeemuBoy/Roms";

        static void Main(string[] args)
        {
            string cartridgePath = $"{romFolderPath}/06-ld r,r.gb";
            //string cartridgePath = "Roms/01-special.gb";
            new EmulatorSDL(cartridgePath, null);
        }
    }
}
