using System;
using System.Collections.Generic;
using System.Text;

namespace GameBoy.GB
{
    class CPU
    {
        public byte a;
        public byte b;
        public byte c;
        public byte d;
        public byte e;
        public byte f;
        public byte h;
        public byte l;

        public ushort PC { get; private set; }
        public ushort SP { get; private set; }

        public byte[] Memory { get; private set; }

        private int cycles = 0;

        private Dictionary<byte, Action> _opCodes = new Dictionary<byte, Action>();

        public string PreviousCommand { get; private set; }

        public CPU()
        {
            CreateOpCodes();

            // TODO Implement memory in own class
            Memory = new byte[]
            {
                0x06, // LD B, 0xFE
                0x06,
                0x0E, // LD C, 0x08
                0x0E,
                0x16,  // LD D, 0x16
                0x16,
                0x1E, // LD E, 0x1E
                0x1E,
                0x26, // LD H, 0x26
                0x26,
                0x2E, // LD L, 0x2E
                0x2E,
            };
        }

        public void Reset()
        {
            PC = 0;
            SP = 0;

            a = 0;
            b = 0;
            c = 0;
            d = 0;
            e = 0;
            f = 0;
            h = 0;
            l = 0;
        }

        public void RunCommand()
        {
            if (PC < Memory.Length)
            {
                _opCodes[Memory[PC]]();
            }
        }

        private void CreateOpCodes()
        {
            // 8-bit loads
            _opCodes.Add(0x06, () => LoadImmediate8(ref b, 8, "LD b, n"));
            _opCodes.Add(0x0E, () => LoadImmediate8(ref c, 8, "LD c, n"));
            _opCodes.Add(0x16, () => LoadImmediate8(ref d, 8, "LD d, n"));
            _opCodes.Add(0x1E, () => LoadImmediate8(ref e, 8, "LD e, n"));
            _opCodes.Add(0x26, () => LoadImmediate8(ref h, 8, "LD h, n"));
            _opCodes.Add(0x2E, () => LoadImmediate8(ref l, 8, "LD l, n"));
        }

        private void LoadImmediate8(ref byte dest, int cycles, string name)
        {
            PC++;
            dest = Memory[PC];
            PC++;
            this.cycles += cycles;
            PreviousCommand = name;
        }
    }
}
