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
        public ushort SP;

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

        public int ReadImmediateByte(out byte value)
        {
            value = Memory.ReadByte(PC);
            PC++;
            return 4;
        }

        public int ReadImmediateWord(out ushort value)
        {
            var msb = Memory.ReadByte(PC);
            PC++;
            var lsb = Memory.ReadByte(PC);
            PC++;
            value = BitUtils.BytesToUshort(msb, lsb);
            return 8;
        }

        private void CreateOpCodes()
        {
            CreateOpCode(0x06, () => ReadImmediateByte(out var immediate) + loadUnit.Load(ref B, immediate), "LD B, n");
            CreateOpCode(0x0E, () => ReadImmediateByte(out var immediate) + loadUnit.Load(ref C, immediate), "LD C, n");
            CreateOpCode(0x16, () => ReadImmediateByte(out var immediate) + loadUnit.Load(ref D, immediate), "LD D, n");
            CreateOpCode(0x1E, () => ReadImmediateByte(out var immediate) + loadUnit.Load(ref E, immediate), "LD E, n");
            CreateOpCode(0x26, () => ReadImmediateByte(out var immediate) + loadUnit.Load(ref H, immediate), "LD H, n");
            CreateOpCode(0x2E, () => ReadImmediateByte(out var immediate) + loadUnit.Load(ref L, immediate), "LD L, n");

            CreateOpCode(0x7F, () => loadUnit.Load(ref A, A), "LD A, A");
            CreateOpCode(0x78, () => loadUnit.Load(ref A, B), "LD A, B");
            CreateOpCode(0x79, () => loadUnit.Load(ref A, C), "LD A, C");
            CreateOpCode(0x7A, () => loadUnit.Load(ref A, D), "LD A, D");
            CreateOpCode(0x7B, () => loadUnit.Load(ref A, E), "LD A, E");
            CreateOpCode(0x7C, () => loadUnit.Load(ref A, H), "LD A, H");
            CreateOpCode(0x7D, () => loadUnit.Load(ref A, L), "LD A, L");
            CreateOpCode(0x7E, () => loadUnit.LoadFromAddress(ref A, H, L), "LD A, (HL)");

            CreateOpCode(0x40, () => loadUnit.Load(ref B, B), "LD B, B");
            CreateOpCode(0x41, () => loadUnit.Load(ref B, C), "LD B, C");
            CreateOpCode(0x42, () => loadUnit.Load(ref B, D), "LD B, D");
            CreateOpCode(0x43, () => loadUnit.Load(ref B, E), "LD B, E");
            CreateOpCode(0x44, () => loadUnit.Load(ref B, H), "LD B, H");
            CreateOpCode(0x45, () => loadUnit.Load(ref B, L), "LD B, L");
            CreateOpCode(0x46, () => loadUnit.LoadFromAddress(ref B, H, L), "LD B, (HL)");

            CreateOpCode(0x48, () => loadUnit.Load(ref C, B), "LD C, B");
            CreateOpCode(0x49, () => loadUnit.Load(ref C, C), "LD C, C");
            CreateOpCode(0x4A, () => loadUnit.Load(ref C, D), "LD C, D");
            CreateOpCode(0x4B, () => loadUnit.Load(ref C, E), "LD C, E");
            CreateOpCode(0x4C, () => loadUnit.Load(ref C, H), "LD C, H");
            CreateOpCode(0x4D, () => loadUnit.Load(ref C, L), "LD C, L");
            CreateOpCode(0x4E, () => loadUnit.LoadFromAddress(ref C, H, L), "LD C, (HL)");

            CreateOpCode(0x50, () => loadUnit.Load(ref D, B), "LD D, B");
            CreateOpCode(0x51, () => loadUnit.Load(ref D, C), "LD D, C");
            CreateOpCode(0x52, () => loadUnit.Load(ref D, D), "LD D, D");
            CreateOpCode(0x53, () => loadUnit.Load(ref D, E), "LD D, E");
            CreateOpCode(0x54, () => loadUnit.Load(ref D, H), "LD D, H");
            CreateOpCode(0x55, () => loadUnit.Load(ref D, L), "LD D, L");
            CreateOpCode(0x56, () => loadUnit.LoadFromAddress(ref D, H, L), "LD D, (HL)");

            CreateOpCode(0x58, () => loadUnit.Load(ref E, B), "LD E, B");
            CreateOpCode(0x59, () => loadUnit.Load(ref E, C), "LD E, C");
            CreateOpCode(0x5A, () => loadUnit.Load(ref E, D), "LD E, D");
            CreateOpCode(0x5B, () => loadUnit.Load(ref E, E), "LD E, E");
            CreateOpCode(0x5C, () => loadUnit.Load(ref E, H), "LD E, H");
            CreateOpCode(0x5D, () => loadUnit.Load(ref E, L), "LD E, L");
            CreateOpCode(0x5E, () => loadUnit.LoadFromAddress(ref E, H, L), "LD E, (HL)");

            CreateOpCode(0x60, () => loadUnit.Load(ref H, B), "LD H, B");
            CreateOpCode(0x61, () => loadUnit.Load(ref H, C), "LD H, C");
            CreateOpCode(0x62, () => loadUnit.Load(ref H, D), "LD H, D");
            CreateOpCode(0x63, () => loadUnit.Load(ref H, E), "LD H, E");
            CreateOpCode(0x64, () => loadUnit.Load(ref H, H), "LD H, H");
            CreateOpCode(0x65, () => loadUnit.Load(ref H, L), "LD H, L");
            CreateOpCode(0x66, () => loadUnit.LoadFromAddress(ref H, H, L), "LD H, (HL)");

            CreateOpCode(0x68, () => loadUnit.Load(ref L, B), "LD L, B");
            CreateOpCode(0x69, () => loadUnit.Load(ref L, C), "LD L, C");
            CreateOpCode(0x6A, () => loadUnit.Load(ref L, D), "LD L, D");
            CreateOpCode(0x6B, () => loadUnit.Load(ref L, E), "LD L, E");
            CreateOpCode(0x6C, () => loadUnit.Load(ref L, H), "LD L, H");
            CreateOpCode(0x6D, () => loadUnit.Load(ref L, L), "LD L, L");
            CreateOpCode(0x6E, () => loadUnit.LoadFromAddress(ref L, H, L), "LD L, (HL)");

            CreateOpCode(0x70, () => loadUnit.WriteToAddress(H, L, B), "LD (HL), B");
            CreateOpCode(0x71, () => loadUnit.WriteToAddress(H, L, C), "LD (HL), C");
            CreateOpCode(0x72, () => loadUnit.WriteToAddress(H, L, D), "LD (HL), D");
            CreateOpCode(0x73, () => loadUnit.WriteToAddress(H, L, E), "LD (HL), E");
            CreateOpCode(0x74, () => loadUnit.WriteToAddress(H, L, H), "LD (HL), H");
            CreateOpCode(0x75, () => loadUnit.WriteToAddress(H, L, L), "LD (HL), L");
            CreateOpCode(0x36, () => ReadImmediateByte(out var immediate) + loadUnit.WriteToAddress(H, L, immediate), "LD (HL), n");

            CreateOpCode(0x0A, () => loadUnit.LoadFromAddress(ref A, B, C), "LD A, (BC)");
            CreateOpCode(0x1A, () => loadUnit.LoadFromAddress(ref A, D, E), "LD A, (DE)");
            CreateOpCode(0xFA, () => ReadImmediateWord(out var immediate) + loadUnit.LoadFromAddress(ref A, immediate), "LD A, (nn)");
            CreateOpCode(0x3E, () => ReadImmediateByte(out var immediate) + loadUnit.Load(ref A, immediate), "LO A, n");

            CreateOpCode(0x47, () => loadUnit.Load(ref B, A), "LD B, A");
            CreateOpCode(0x4F, () => loadUnit.Load(ref C, A), "LD C, A");
            CreateOpCode(0x57, () => loadUnit.Load(ref D, A), "LD D, A");
            CreateOpCode(0x5F, () => loadUnit.Load(ref E, A), "LD E, A");
            CreateOpCode(0x67, () => loadUnit.Load(ref H, A), "LD H, A");
            CreateOpCode(0x6F, () => loadUnit.Load(ref L, A), "LD L, A");
            CreateOpCode(0x02, () => loadUnit.WriteToAddress(B, C, A), "LD (BC), A");
            CreateOpCode(0x12, () => loadUnit.WriteToAddress(D, E, A), "LD (DE), A");
            CreateOpCode(0x77, () => loadUnit.WriteToAddress(H, L, A), "LD (HL), A");
            CreateOpCode(0xEA, () => ReadImmediateWord(out var immediate) + loadUnit.WriteToAddress(immediate, A), "LD (nn), A");

            CreateOpCode(0xF2, () => loadUnit.LoadFromAddress(ref A, (ushort)(0xFF00 + C)), "LD A, (C)");
            CreateOpCode(0xE2, () => loadUnit.WriteToAddress((ushort)(0xFF00 + C), A), "LD (C), A");

            CreateOpCode(0x3A, () => loadUnit.LoadFromAddressAndIncrement(ref A, ref H, ref L, -1), "LD A, (HL-)");
            CreateOpCode(0x32, () => loadUnit.WriteToAddressAndIncrement(ref H, ref L, A, -1), "LD (HL-), A");
            CreateOpCode(0x2A, () => loadUnit.LoadFromAddressAndIncrement(ref A, ref H, ref L, 1), "LD A, (HL+)");
            CreateOpCode(0x22, () => loadUnit.WriteToAddressAndIncrement(ref H, ref L, A, 1), "LD (HL+), A");
            CreateOpCode(0xE0, () => ReadImmediateByte(out var immediate) + loadUnit.WriteToAddress((ushort)(0xFF00 + immediate), A), "LDH (n), A");
            CreateOpCode(0xF0, () => ReadImmediateByte(out var immediate) + loadUnit.LoadFromAddress(ref A, (ushort)(0xFF00 + immediate)), "LDH A, (n)");

            CreateOpCode(0x01, () => ReadImmediateWord(out var immediate) + loadUnit.Load(ref B, ref C, immediate), "LD BC, nn");
            CreateOpCode(0x11, () => ReadImmediateWord(out var immediate) + loadUnit.Load(ref D, ref E, immediate), "LD DE, nn");
            CreateOpCode(0x21, () => ReadImmediateWord(out var immediate) + loadUnit.Load(ref H, ref L, immediate), "LD HL, nn");
            CreateOpCode(0x31, () => ReadImmediateWord(out var immediate) + loadUnit.Load(ref SP, immediate), "LD SP, nn");
            CreateOpCode(0xF9, () => loadUnit.Load(ref SP, H, L), "LD SP, HL");
        }

        private void CreateOpCode(byte command, Func<int> action, string name)
        {
            OpCodes.Add(command, action);
            opCodeNames.Add(command, name);
        }
    }
}
