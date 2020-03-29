using System;
using System.Collections.Generic;
using System.Text;

namespace GameBoy
{
    class Program
    {
        static void Main(string[] args)
        {
            //String path = "Roms/cpu_instrs.gb";
            //String path = "Roms/06-ld r,r.gb";
            //String path = "Roms/01-special.gb";
            string path = "Roms/DMG_ROM.bin";
            new GameBoyEmulator(path);
        }
    }
}
