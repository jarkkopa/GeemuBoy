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
        private Dictionary<byte, string> _opCodeNames = new Dictionary<byte, string>();

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

        public string GetOpCodeName(byte code)
        {
            if (_opCodeNames.ContainsKey(code))
            {
                return _opCodeNames[code];
            }
            return "";
        }

        private void CreateOpCodes()
        {
            // 8-bit loads
            CreateOpCode(0x06, () => LoadImmediate8(ref b, 8), "LD b, n");
            CreateOpCode(0x0E, () => LoadImmediate8(ref c, 8), "LD c, n");
            CreateOpCode(0x16, () => LoadImmediate8(ref d, 8), "LD d, n");
            CreateOpCode(0x1E, () => LoadImmediate8(ref e, 8), "LD e, n");
            CreateOpCode(0x26, () => LoadImmediate8(ref h, 8), "LD h, n");
            CreateOpCode(0x2E, () => LoadImmediate8(ref l, 8), "LD l, n");
        }

        private void CreateOpCode(byte command, Action action, string name)
        {
            _opCodes.Add(command, action);
            _opCodeNames.Add(command, name);
        }

        private void LoadImmediate8(ref byte dest, int cycles)
        {
            PC++;
            dest = Memory[PC];
            PC++;
            this.cycles += cycles;
        }
    }
}
