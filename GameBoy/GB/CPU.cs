using System;
using System.Collections.Generic;
using System.Text;

namespace GameBoy.GB
{
    public class CPU
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

        //private int cycles = 0;
        public int Cycles { get; set; }

        private Dictionary<byte, Action> _opCodes = new Dictionary<byte, Action>();
        private Dictionary<byte, string> _opCodeNames = new Dictionary<byte, string>();

        public CPU(byte[] memory)
        {
            CreateOpCodes();

            // TODO Implement memory in own class
            Memory = memory;
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
                var code = Memory[PC];
                PC++;
                _opCodes[code]();
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

        public void LoadImmediate8(ref byte dest, int cycles)
        {
            dest = Memory[PC];
            PC++;
            Cycles += cycles;
        }
    }
}
