using GameBoy.GB.CpuUnits;
using System;
using System.Collections.Generic;

namespace GameBoy.GB
{
    public class CPU
    {
        private readonly ILoadUnit loadUnit;

        public byte A;
        public byte B;
        public byte C;
        public byte D;
        public byte E;
        public byte F;
        public byte H;
        public byte L;

        public ushort PC;

        public ushort SP { get; private set; }

        public int Cycles { get; private set; }

        public Memory Memory { get; private set; }

        public Dictionary<byte, Func<int>> OpCodes { get; private set; } = new Dictionary<byte, Func<int>>();

        private readonly Dictionary<byte, string> opCodeNames = new Dictionary<byte, string>();

        public CPU(Memory memory, ILoadUnit loadUnit)
        {
            this.loadUnit = loadUnit;

            CreateOpCodes();

            Memory = memory;
        }

        public CPU(Memory memory) : this(memory, new LoadUnit(memory))
        {
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
            if (OpCodes.ContainsKey(code))
            {
                Cycles = OpCodes[code]();
            }
            else
            {
                throw new Exception($"Trying to run opcode 0x{code:x2} that is not implemented.");
            }
        }

        public string GetOpCodeName(byte code)
        {
            if (opCodeNames.ContainsKey(code))
            {
                return opCodeNames[code];
            }
            return $"Unknown opcode: {code:x2}";
        }

        private void CreateOpCodes()
        {
            // LD nn, n
            CreateOpCode(0x06, () => loadUnit.LoadImmediateByte(ref B, ref PC), "LD B, n");
            CreateOpCode(0x0E, () => loadUnit.LoadImmediateByte(ref C, ref PC), "LD C, n");
            CreateOpCode(0x16, () => loadUnit.LoadImmediateByte(ref D, ref PC), "LD D, n");
            CreateOpCode(0x1E, () => loadUnit.LoadImmediateByte(ref E, ref PC), "LD E, n");
            CreateOpCode(0x26, () => loadUnit.LoadImmediateByte(ref H, ref PC), "LD H, n");
            CreateOpCode(0x2E, () => loadUnit.LoadImmediateByte(ref L, ref PC), "LD L, n");

            //LD r1, r2
            CreateOpCode(0x7F, () => loadUnit.Copy(ref A, ref A), "LD A, A");
            CreateOpCode(0x78, () => loadUnit.Copy(ref A, ref B), "LD A, B");
            CreateOpCode(0x79, () => loadUnit.Copy(ref A, ref C), "LD A, C");
            CreateOpCode(0x7A, () => loadUnit.Copy(ref A, ref D), "LD A, D");
            CreateOpCode(0x7B, () => loadUnit.Copy(ref A, ref E), "LD A, E");
            CreateOpCode(0x7C, () => loadUnit.Copy(ref A, ref H), "LD A, H");
            CreateOpCode(0x7D, () => loadUnit.Copy(ref A, ref L), "LD A, L");
            CreateOpCode(0x7E, () => loadUnit.LoadFromAddress(ref A, H, L), "LD A, (HL)");

            CreateOpCode(0x40, () => loadUnit.Copy(ref B, ref B), "LD B, B");
            CreateOpCode(0x41, () => loadUnit.Copy(ref B, ref C), "LD B, C");
            CreateOpCode(0x42, () => loadUnit.Copy(ref B, ref D), "LD B, D");
            CreateOpCode(0x43, () => loadUnit.Copy(ref B, ref E), "LD B, E");
            CreateOpCode(0x44, () => loadUnit.Copy(ref B, ref H), "LD B, H");
            CreateOpCode(0x45, () => loadUnit.Copy(ref B, ref L), "LD B, L");
            CreateOpCode(0x46, () => loadUnit.LoadFromAddress(ref B, H, L), "LD B, (HL)");

            CreateOpCode(0x48, () => loadUnit.Copy(ref C, ref B), "LD C, B");
            CreateOpCode(0x49, () => loadUnit.Copy(ref C, ref C), "LD C, C");
            CreateOpCode(0x4A, () => loadUnit.Copy(ref C, ref D), "LD C, D");
            CreateOpCode(0x4B, () => loadUnit.Copy(ref C, ref E), "LD C, E");
            CreateOpCode(0x4C, () => loadUnit.Copy(ref C, ref H), "LD C, H");
            CreateOpCode(0x4D, () => loadUnit.Copy(ref C, ref L), "LD C, L");
            CreateOpCode(0x4E, () => loadUnit.LoadFromAddress(ref C, H, L), "LD C, (HL)");

            CreateOpCode(0x50, () => loadUnit.Copy(ref D, ref B), "LD D, B");
            CreateOpCode(0x51, () => loadUnit.Copy(ref D, ref C), "LD D, C");
            CreateOpCode(0x52, () => loadUnit.Copy(ref D, ref D), "LD D, D");
            CreateOpCode(0x53, () => loadUnit.Copy(ref D, ref E), "LD D, E");
            CreateOpCode(0x54, () => loadUnit.Copy(ref D, ref H), "LD D, H");
            CreateOpCode(0x55, () => loadUnit.Copy(ref D, ref L), "LD D, L");
            CreateOpCode(0x56, () => loadUnit.LoadFromAddress(ref D, H, L), "LD D, (HL)");

            CreateOpCode(0x58, () => loadUnit.Copy(ref E, ref B), "LD E, B");
            CreateOpCode(0x59, () => loadUnit.Copy(ref E, ref C), "LD E, C");
            CreateOpCode(0x5A, () => loadUnit.Copy(ref E, ref D), "LD E, D");
            CreateOpCode(0x5B, () => loadUnit.Copy(ref E, ref E), "LD E, E");
            CreateOpCode(0x5C, () => loadUnit.Copy(ref E, ref H), "LD E, H");
            CreateOpCode(0x5D, () => loadUnit.Copy(ref E, ref L), "LD E, L");
            CreateOpCode(0x5E, () => loadUnit.LoadFromAddress(ref E, H, L), "LD E, (HL)");

            CreateOpCode(0x60, () => loadUnit.Copy(ref H, ref B), "LD H, B");
            CreateOpCode(0x61, () => loadUnit.Copy(ref H, ref C), "LD H, C");
            CreateOpCode(0x62, () => loadUnit.Copy(ref H, ref D), "LD H, D");
            CreateOpCode(0x63, () => loadUnit.Copy(ref H, ref E), "LD H, E");
            CreateOpCode(0x64, () => loadUnit.Copy(ref H, ref H), "LD H, H");
            CreateOpCode(0x65, () => loadUnit.Copy(ref H, ref L), "LD H, L");
            CreateOpCode(0x66, () => loadUnit.LoadFromAddress(ref H, H, L), "LD H, (HL)");

            CreateOpCode(0x68, () => loadUnit.Copy(ref L, ref B), "LD L, B");
            CreateOpCode(0x69, () => loadUnit.Copy(ref L, ref C), "LD L, C");
            CreateOpCode(0x6A, () => loadUnit.Copy(ref L, ref D), "LD L, D");
            CreateOpCode(0x6B, () => loadUnit.Copy(ref L, ref E), "LD L, E");
            CreateOpCode(0x6C, () => loadUnit.Copy(ref L, ref H), "LD L, H");
            CreateOpCode(0x6D, () => loadUnit.Copy(ref L, ref L), "LD L, L");
            CreateOpCode(0x6E, () => loadUnit.LoadFromAddress(ref L, H, L), "LD L, (HL)");

            CreateOpCode(0x70, () => loadUnit.WriteToAddress(H, L, ref B), "LD (HL), B");
            CreateOpCode(0x71, () => loadUnit.WriteToAddress(H, L, ref C), "LD (HL), C");
            CreateOpCode(0x72, () => loadUnit.WriteToAddress(H, L, ref D), "LD (HL), D");
            CreateOpCode(0x73, () => loadUnit.WriteToAddress(H, L, ref E), "LD (HL), E");
            CreateOpCode(0x74, () => loadUnit.WriteToAddress(H, L, ref H), "LD (HL), H");
            CreateOpCode(0x75, () => loadUnit.WriteToAddress(H, L, ref L), "LD (HL), L");
            CreateOpCode(0x36, () => loadUnit.LoadImmediateByteToAddress(H, L, ref PC), "LD (HL), n");

            CreateOpCode(0x0A, () => loadUnit.LoadFromAddress(ref A, B, C), "LD A, (BC)");
            CreateOpCode(0x1A, () => loadUnit.LoadFromAddress(ref A, D, E), "LD A, (DE)");
            CreateOpCode(0xFA, () => loadUnit.LoadFromImmediateAddress(ref A, ref PC), "LD A, (nn)");
            CreateOpCode(0x3E, () => loadUnit.LoadImmediateByte(ref A, ref PC), "LD A, n");

            CreateOpCode(0x47, () => loadUnit.Copy(ref B, ref A), "LD B, A");
            CreateOpCode(0x4F, () => loadUnit.Copy(ref C, ref A), "LD C, A");
            CreateOpCode(0x57, () => loadUnit.Copy(ref D, ref A), "LD D, A");
            CreateOpCode(0x5F, () => loadUnit.Copy(ref E, ref A), "LD E, A");
            CreateOpCode(0x67, () => loadUnit.Copy(ref H, ref A), "LD H, A");
            CreateOpCode(0x6F, () => loadUnit.Copy(ref L, ref A), "LD L, A");
            CreateOpCode(0x02, () => loadUnit.WriteToAddress(B, C, ref A), "LD (BC), A");
            CreateOpCode(0x12, () => loadUnit.WriteToAddress(D, E, ref A), "LD (DE), A");
            CreateOpCode(0x77, () => loadUnit.WriteToAddress(H, L, ref A), "LD (HL), A");
            CreateOpCode(0xEA, () => loadUnit.WriteToImmediateAddress(A, ref PC), "LD (nn), A");
        }

        private void CreateOpCode(byte command, Func<int> action, string name)
        {
            OpCodes.Add(command, action);
            opCodeNames.Add(command, name);
        }
    }
}
