namespace GeemuBoy
{
    class Program
    {
        private static readonly string romFolderPath = "C:/repos/GeemuBoy/GeemuBoy/Roms";

        static void Main(string[] args)
        {
            string cartridgePath = $"{romFolderPath}/06-ld r,r.gb";
            new EmulatorSDL(cartridgePath, null);
        }
    }
}
