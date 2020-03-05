using System;
using System.Collections.Generic;

namespace GameBoy.GB
{
    public class CPU
    {
        public byte A;
        public byte B;
        public byte C;
        public byte D;
        public byte E;
        public byte F;
        public byte H;
        public byte L;

        public ushort PC { get; private set; }
        public ushort SP { get; private set; }


        public int Cycles { get; private set; }

        public Memory Memory { get; private set; }

        private readonly Dictionary<byte, Action> _opCodes = new Dictionary<byte, Action>();
        private readonly Dictionary<byte, string> _opCodeNames = new Dictionary<byte, string>();

        public CPU(Memory memory)
        {
            CreateOpCodes();

            this.Memory = memory;
        }

        public void Reset()
        {
            PC = 0;
            SP = 0;

            A = 0;
            B = 0;
            C = 0;
            D = 0;
            E = 0;
            F = 0;
            H = 0;
            L = 0;
        }

        public void RunCommand()
        {
            var code = Memory.ReadByte(PC);
            PC++;
            if (_opCodes.ContainsKey(code))
            {
                _opCodes[code]();
            }
            else
            {
                throw new Exception($"Trying to run opcode 0x{code:4}that is not implemented.");
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
            // LD nn, n
            CreateOpCode(0x06, () => LoadImmediate8(ref B), "LD b, n");
            CreateOpCode(0x0E, () => LoadImmediate8(ref C), "LD c, n");
            CreateOpCode(0x16, () => LoadImmediate8(ref D), "LD d, n");
            CreateOpCode(0x1E, () => LoadImmediate8(ref E), "LD e, n");
            CreateOpCode(0x26, () => LoadImmediate8(ref H), "LD h, n");
            CreateOpCode(0x2E, () => LoadImmediate8(ref L), "LD l, n");

            //LD r1, r2
            CreateOpCode(0x7F, () => Copy(ref A, ref A), "LD A, A");
            CreateOpCode(0x78, () => Copy(ref A, ref B), "LD A, B");
            CreateOpCode(0x79, () => Copy(ref A, ref C), "LD A, C");
            CreateOpCode(0x7A, () => Copy(ref A, ref D), "LD A, D");
            CreateOpCode(0x7B, () => Copy(ref A, ref E), "LD A, E");
            CreateOpCode(0x7C, () => Copy(ref A, ref H), "LD A, H");
            CreateOpCode(0x7D, () => Copy(ref A, ref L), "LD A, L");
            CreateOpCode(0x7E, () => CopyFromAddress(ref A, BitUtils.BytesToUshort(H, L)), "LD A, (HL)");
        }

        private void CreateOpCode(byte command, Action action, string name)
        {
            _opCodes.Add(command, action);
            _opCodeNames.Add(command, name);
        }

        public void LoadImmediate8(ref byte dest)
        {
            dest = Memory.ReadByte(PC);
            PC++;
            Cycles = 8;
        }

        public void Copy(ref byte dest, ref byte source)
        {
            dest = source;
            PC++;
            Cycles = 4;
        }

        public void CopyFromAddress(ref byte dest, ushort address)
        {
            byte value = Memory.ReadByte(address);
            dest = value;
            PC++;
            Cycles = 8;
        }
    }
}
