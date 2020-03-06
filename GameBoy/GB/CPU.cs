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
                throw new Exception($"Trying to run opcode 0x{code:x2} that is not implemented.");
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
            CreateOpCode(0x7E, () => LoadFromAddress(ref A, BitUtils.BytesToUshort(H, L)), "LD A, (HL)");

            CreateOpCode(0x40, () => Copy(ref B, ref B), "LD B, B");
            CreateOpCode(0x41, () => Copy(ref B, ref C), "LD B, C");
            CreateOpCode(0x42, () => Copy(ref B, ref D), "LD B, D");
            CreateOpCode(0x43, () => Copy(ref B, ref E), "LD B, E");
            CreateOpCode(0x44, () => Copy(ref B, ref H), "LD B, H");
            CreateOpCode(0x45, () => Copy(ref B, ref L), "LD B, L");
            CreateOpCode(0x46, () => LoadFromAddress(ref B, BitUtils.BytesToUshort(H, L)), "LD B, (HL)");

            CreateOpCode(0x48, () => Copy(ref C, ref B), "LD C, B");
            CreateOpCode(0x49, () => Copy(ref C, ref C), "LD C, C");
            CreateOpCode(0x4A, () => Copy(ref C, ref D), "LD C, D");
            CreateOpCode(0x4B, () => Copy(ref C, ref E), "LD C, E");
            CreateOpCode(0x4C, () => Copy(ref C, ref H), "LD C, H");
            CreateOpCode(0x4D, () => Copy(ref C, ref L), "LD C, L");
            CreateOpCode(0x4E, () => LoadFromAddress(ref C, BitUtils.BytesToUshort(H, L)), "LD C, (HL)");

            CreateOpCode(0x50, () => Copy(ref D, ref B), "LD D, B");
            CreateOpCode(0x51, () => Copy(ref D, ref C), "LD D, C");
            CreateOpCode(0x52, () => Copy(ref D, ref D), "LD D, D");
            CreateOpCode(0x53, () => Copy(ref D, ref E), "LD D, E");
            CreateOpCode(0x54, () => Copy(ref D, ref H), "LD D, H");
            CreateOpCode(0x55, () => Copy(ref D, ref L), "LD D, L");
            CreateOpCode(0x56, () => LoadFromAddress(ref D, BitUtils.BytesToUshort(H, L)), "LD D, (HL)");

            CreateOpCode(0x58, () => Copy(ref E, ref B), "LD E, B");
            CreateOpCode(0x59, () => Copy(ref E, ref C), "LD E, C");
            CreateOpCode(0x5A, () => Copy(ref E, ref D), "LD E, D");
            CreateOpCode(0x5B, () => Copy(ref E, ref E), "LD E, E");
            CreateOpCode(0x5C, () => Copy(ref E, ref H), "LD E, H");
            CreateOpCode(0x5D, () => Copy(ref E, ref L), "LD E, L");
            CreateOpCode(0x5E, () => LoadFromAddress(ref E, BitUtils.BytesToUshort(H, L)), "LD E, (HL)");

            CreateOpCode(0x60, () => Copy(ref H, ref B), "LD H, B");
            CreateOpCode(0x61, () => Copy(ref H, ref C), "LD H, C");
            CreateOpCode(0x62, () => Copy(ref H, ref D), "LD H, D");
            CreateOpCode(0x63, () => Copy(ref H, ref E), "LD H, E");
            CreateOpCode(0x64, () => Copy(ref H, ref H), "LD H, H");
            CreateOpCode(0x65, () => Copy(ref H, ref L), "LD H, L");
            CreateOpCode(0x66, () => LoadFromAddress(ref H, BitUtils.BytesToUshort(H, L)), "LD H, (HL)");

            CreateOpCode(0x68, () => Copy(ref L, ref B), "LD L, B");
            CreateOpCode(0x69, () => Copy(ref L, ref C), "LD L, C");
            CreateOpCode(0x6A, () => Copy(ref L, ref D), "LD L, D");
            CreateOpCode(0x6B, () => Copy(ref L, ref E), "LD L, E");
            CreateOpCode(0x6C, () => Copy(ref L, ref H), "LD L, H");
            CreateOpCode(0x6D, () => Copy(ref L, ref L), "LD L, L");
            CreateOpCode(0x6E, () => LoadFromAddress(ref L, BitUtils.BytesToUshort(H, L)), "LD L, (HL)");

            CreateOpCode(0x70, () => CopyToAddress(BitUtils.BytesToUshort(H, L), ref B), "LD (HL), B");
            CreateOpCode(0x71, () => CopyToAddress(BitUtils.BytesToUshort(H, L), ref C), "LD (HL), C");
            CreateOpCode(0x72, () => CopyToAddress(BitUtils.BytesToUshort(H, L), ref D), "LD (HL), D");
            CreateOpCode(0x73, () => CopyToAddress(BitUtils.BytesToUshort(H, L), ref E), "LD (HL), E");
            CreateOpCode(0x74, () => CopyToAddress(BitUtils.BytesToUshort(H, L), ref H), "LD (HL), H");
            CreateOpCode(0x75, () => CopyToAddress(BitUtils.BytesToUshort(H, L), ref L), "LD (HL), L");
            CreateOpCode(0x36, () => LoadImmediate8ToAddress(BitUtils.BytesToUshort(H, L)), "LD (HL), ");

            CreateOpCode(0x0A, () => LoadFromAddress(ref A, BitUtils.BytesToUshort(B, C)), "LD A, (BC)");
            CreateOpCode(0x1A, () => LoadFromAddress(ref A, BitUtils.BytesToUshort(D, E)), "LD A, (DE)");
            CreateOpCode(0xFA, () => LoadFromImmediateAddress(ref A), "LD A, (nn)");
            CreateOpCode(0x3E, () => LoadImmediate8(ref A), "LD A, n");
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
            Cycles = 4;
        }

        public void LoadFromAddress(ref byte dest, ushort address)
        {
            byte value = Memory.ReadByte(address);
            dest = value;
            Cycles = 8;
        }

        public void CopyToAddress(ushort address, ref byte source)
        {
            Memory.WriteByte(address, source);
            Cycles = 8;
        }

        public void LoadImmediate8ToAddress(ushort address)
        {
            byte data = Memory.ReadByte(PC);
            PC++;
            Memory.WriteByte(address, data);
            Cycles = 12;
        }

        public void LoadFromImmediateAddress(ref byte dest)
        {
            byte addressHigh = Memory.ReadByte(PC);
            PC++;
            byte addressLow = Memory.ReadByte(PC);
            PC++;
            dest = Memory.ReadByte(BitUtils.BytesToUshort(addressHigh, addressLow));
            Cycles = 16;
        }
    }
}
