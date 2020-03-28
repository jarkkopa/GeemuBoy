using GameBoy.GB.CpuUnits;
using System;
using System.Collections.Generic;

namespace GameBoy.GB
{
    public class CPU
    {
        public const byte PREFIX_OPCODE = 0xCB;

        private readonly ILoadUnit loadUnit;
        private readonly IALU alu;
        private readonly IMiscUnit miscUnit;
        private readonly IJumpUnit jumpUnit;
        private readonly IBitUnit bitOpsUnit;

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

        public bool IME = false;

        public Memory Memory { get; private set; }

        public Dictionary<byte, OpCode> OpCodes { get; private set; } = new Dictionary<byte, OpCode>();
        public Dictionary<byte, OpCode> OpCodesPrefixed { get; private set; } = new Dictionary<byte, OpCode>();

        public CPU(Memory memory, ILoadUnit loadUnit, IALU alu, IMiscUnit miscUnit, IJumpUnit jumpUnit, IBitUnit bitOpsUnit)
        {
            this.loadUnit = loadUnit;
            this.alu = alu;
            this.miscUnit = miscUnit;
            this.jumpUnit = jumpUnit;
            this.bitOpsUnit = bitOpsUnit;

            CreateLoadUnitOpCodes();
            CreateALUOpCodes();
            CreateMiscOpCodes();
            CreateJumpOpCodes();
            CreateBitUnitOpCodes();

            Memory = memory;
        }

        public CPU(Memory memory) :
            this(memory, new LoadUnit(memory), new ALU(memory), new MiscUnit(memory), new JumpUnit(memory), new BitUnit())
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

        public void SetInitialStateAfterBootSequence()
        {
            A = 0x01; F = 0xB0;
            B = 0x00; C = 0x13;
            D = 0x00; E = 0xD8;
            H = 0x01; L = 0x4D;
            PC = 0x100;
            SP = 0xFFFE;

            Memory.WriteByte(0xFF05, 0x00);
            Memory.WriteByte(0xFF06, 0x00);
            Memory.WriteByte(0xFF07, 0x00);
            Memory.WriteByte(0xFF10, 0x80);
            Memory.WriteByte(0xFF11, 0xBF);
            Memory.WriteByte(0xFF12, 0xF3);
            Memory.WriteByte(0xFF14, 0xBF);
            Memory.WriteByte(0xFF16, 0x3F);
            Memory.WriteByte(0xFF17, 0x00);
            Memory.WriteByte(0xFF19, 0xBF);
            Memory.WriteByte(0xFF1A, 0x7F);
            Memory.WriteByte(0xFF1B, 0xFF);
            Memory.WriteByte(0xFF1C, 0x9F);
            Memory.WriteByte(0xFF1E, 0xBF);
            Memory.WriteByte(0xFF20, 0xFF);
            Memory.WriteByte(0xFF21, 0x00);
            Memory.WriteByte(0xFF22, 0x00);
            Memory.WriteByte(0xFF23, 0xBF);
            Memory.WriteByte(0xFF24, 0x77);
            Memory.WriteByte(0xFF25, 0xF3);
            Memory.WriteByte(0xFF26, 0xF1);
            Memory.WriteByte(0xFF40, 0x91);
            Memory.WriteByte(0xFF42, 0x00);
            Memory.WriteByte(0xFF43, 0x00);
            Memory.WriteByte(0xFF45, 0x00);
            Memory.WriteByte(0xFF47, 0xFC);
            Memory.WriteByte(0xFF48, 0xFF);
            Memory.WriteByte(0xFF49, 0xFF);
            Memory.WriteByte(0xFF4A, 0x00);
            Memory.WriteByte(0xFF4B, 0x00);
            Memory.WriteByte(0xFFFF, 0x00);
        }

        public void RunCommand()
        {
            var code = Memory.ReadByte(PC);
            PC++;
            if (code == PREFIX_OPCODE || OpCodes.ContainsKey(code))
            {
                OpCode opCode;
                if (code == PREFIX_OPCODE)
                {
                    code = Memory.ReadByte(PC);
                    PC++;
                    if (OpCodesPrefixed.ContainsKey(code))
                    {
                        opCode = OpCodesPrefixed[code];
                    }
                    else
                    {
                        throw new Exception($"Trying to run prefixed opcode 0x{PREFIX_OPCODE:x2} 0x{code:x2} that is not implemented.");
                    }

                }
                else
                {
                    opCode = OpCodes[code];
                }
                Cycles = opCode.Cycles;
                opCode.Instruction();
            }
            else
            {
                throw new Exception($"Trying to run opcode 0x{code:x2} that is not implemented.");
            }
        }

        public int ReadImmediateByte(out byte value)
        {
            value = Memory.ReadByte(PC);
            PC++;
            return 4;
        }

        public int ReadImmediateWord(out ushort value)
        {
            var lsb = Memory.ReadByte(PC);
            PC++;
            var msb = Memory.ReadByte(PC);
            PC++;
            value = BitUtils.BytesToUshort(msb, lsb);
            return 8;
        }

        public int ReadFromMemory(byte addrHigh, byte addrLow, out byte value)
        {
            value = Memory.ReadByte(BitUtils.BytesToUshort(addrHigh, addrLow));
            return 4;
        }

        private void CreateLoadUnitOpCodes()
        {
            CreateOpCode(0x06, () => { ReadImmediateByte(out var immediate); loadUnit.Load(ref B, immediate); }, 8, "LD B, d8");
            CreateOpCode(0x0E, () => { ReadImmediateByte(out var immediate); loadUnit.Load(ref C, immediate); }, 8, "LD C, d8");
            CreateOpCode(0x16, () => { ReadImmediateByte(out var immediate); loadUnit.Load(ref D, immediate); }, 8, "LD D, d8");
            CreateOpCode(0x1E, () => { ReadImmediateByte(out var immediate); loadUnit.Load(ref E, immediate); }, 8, "LD E, d8");
            CreateOpCode(0x26, () => { ReadImmediateByte(out var immediate); loadUnit.Load(ref H, immediate); }, 8, "LD H, d8");
            CreateOpCode(0x2E, () => { ReadImmediateByte(out var immediate); loadUnit.Load(ref L, immediate); }, 8, "LD L, d8");

            CreateOpCode(0x7F, () => loadUnit.Load(ref A, A), 4, "LD A, A");
            CreateOpCode(0x78, () => loadUnit.Load(ref A, B), 4, "LD A, B");
            CreateOpCode(0x79, () => loadUnit.Load(ref A, C), 4, "LD A, C");
            CreateOpCode(0x7A, () => loadUnit.Load(ref A, D), 4, "LD A, D");
            CreateOpCode(0x7B, () => loadUnit.Load(ref A, E), 4, "LD A, E");
            CreateOpCode(0x7C, () => loadUnit.Load(ref A, H), 4, "LD A, H");
            CreateOpCode(0x7D, () => loadUnit.Load(ref A, L), 4, "LD A, L");
            CreateOpCode(0x7E, () => loadUnit.LoadFromAddress(ref A, H, L), 8, "LD A, (HL)");

            CreateOpCode(0x40, () => loadUnit.Load(ref B, B), 4, "LD B, B");
            CreateOpCode(0x41, () => loadUnit.Load(ref B, C), 4, "LD B, C");
            CreateOpCode(0x42, () => loadUnit.Load(ref B, D), 4, "LD B, D");
            CreateOpCode(0x43, () => loadUnit.Load(ref B, E), 4, "LD B, E");
            CreateOpCode(0x44, () => loadUnit.Load(ref B, H), 4, "LD B, H");
            CreateOpCode(0x45, () => loadUnit.Load(ref B, L), 4, "LD B, L");
            CreateOpCode(0x46, () => loadUnit.LoadFromAddress(ref B, H, L), 8, "LD B, (HL)");

            CreateOpCode(0x48, () => loadUnit.Load(ref C, B), 4, "LD C, B");
            CreateOpCode(0x49, () => loadUnit.Load(ref C, C), 4, "LD C, C");
            CreateOpCode(0x4A, () => loadUnit.Load(ref C, D), 4, "LD C, D");
            CreateOpCode(0x4B, () => loadUnit.Load(ref C, E), 4, "LD C, E");
            CreateOpCode(0x4C, () => loadUnit.Load(ref C, H), 4, "LD C, H");
            CreateOpCode(0x4D, () => loadUnit.Load(ref C, L), 4, "LD C, L");
            CreateOpCode(0x4E, () => loadUnit.LoadFromAddress(ref C, H, L), 8, "LD C, (HL)");

            CreateOpCode(0x50, () => loadUnit.Load(ref D, B), 4, "LD D, B");
            CreateOpCode(0x51, () => loadUnit.Load(ref D, C), 4, "LD D, C");
            CreateOpCode(0x52, () => loadUnit.Load(ref D, D), 4, "LD D, D");
            CreateOpCode(0x53, () => loadUnit.Load(ref D, E), 4, "LD D, E");
            CreateOpCode(0x54, () => loadUnit.Load(ref D, H), 4, "LD D, H");
            CreateOpCode(0x55, () => loadUnit.Load(ref D, L), 4, "LD D, L");
            CreateOpCode(0x56, () => loadUnit.LoadFromAddress(ref D, H, L), 8, "LD D, (HL)");

            CreateOpCode(0x58, () => loadUnit.Load(ref E, B), 4, "LD E, B");
            CreateOpCode(0x59, () => loadUnit.Load(ref E, C), 4, "LD E, C");
            CreateOpCode(0x5A, () => loadUnit.Load(ref E, D), 4, "LD E, D");
            CreateOpCode(0x5B, () => loadUnit.Load(ref E, E), 4, "LD E, E");
            CreateOpCode(0x5C, () => loadUnit.Load(ref E, H), 4, "LD E, H");
            CreateOpCode(0x5D, () => loadUnit.Load(ref E, L), 4, "LD E, L");
            CreateOpCode(0x5E, () => loadUnit.LoadFromAddress(ref E, H, L), 8, "LD E, (HL)");

            CreateOpCode(0x60, () => loadUnit.Load(ref H, B), 4, "LD H, B");
            CreateOpCode(0x61, () => loadUnit.Load(ref H, C), 4, "LD H, C");
            CreateOpCode(0x62, () => loadUnit.Load(ref H, D), 4, "LD H, D");
            CreateOpCode(0x63, () => loadUnit.Load(ref H, E), 4, "LD H, E");
            CreateOpCode(0x64, () => loadUnit.Load(ref H, H), 4, "LD H, H");
            CreateOpCode(0x65, () => loadUnit.Load(ref H, L), 4, "LD H, L");
            CreateOpCode(0x66, () => loadUnit.LoadFromAddress(ref H, H, L), 8, "LD H, (HL)");

            CreateOpCode(0x68, () => loadUnit.Load(ref L, B), 4, "LD L, B");
            CreateOpCode(0x69, () => loadUnit.Load(ref L, C), 4, "LD L, C");
            CreateOpCode(0x6A, () => loadUnit.Load(ref L, D), 4, "LD L, D");
            CreateOpCode(0x6B, () => loadUnit.Load(ref L, E), 4, "LD L, E");
            CreateOpCode(0x6C, () => loadUnit.Load(ref L, H), 4, "LD L, H");
            CreateOpCode(0x6D, () => loadUnit.Load(ref L, L), 4, "LD L, L");
            CreateOpCode(0x6E, () => loadUnit.LoadFromAddress(ref L, H, L), 8, "LD L, (HL)");

            CreateOpCode(0x70, () => loadUnit.WriteToAddress(H, L, B), 8, "LD (HL), B");
            CreateOpCode(0x71, () => loadUnit.WriteToAddress(H, L, C), 8, "LD (HL), C");
            CreateOpCode(0x72, () => loadUnit.WriteToAddress(H, L, D), 8, "LD (HL), D");
            CreateOpCode(0x73, () => loadUnit.WriteToAddress(H, L, E), 8, "LD (HL), E");
            CreateOpCode(0x74, () => loadUnit.WriteToAddress(H, L, H), 8, "LD (HL), H");
            CreateOpCode(0x75, () => loadUnit.WriteToAddress(H, L, L), 8, "LD (HL), L");
            CreateOpCode(0x36, () => { ReadImmediateByte(out var immediate); loadUnit.WriteToAddress(H, L, immediate); }, 12, "LD (HL), d8");

            CreateOpCode(0x0A, () => loadUnit.LoadFromAddress(ref A, B, C), 8, "LD A, (BC)");
            CreateOpCode(0x1A, () => loadUnit.LoadFromAddress(ref A, D, E), 8, "LD A, (DE)");
            CreateOpCode(0xFA, () => { ReadImmediateWord(out var immediate); loadUnit.LoadFromAddress(ref A, immediate); }, 16, "LD A, (a16)");
            CreateOpCode(0x3E, () => { ReadImmediateByte(out var immediate); loadUnit.Load(ref A, immediate); }, 8, "LO A, d8");

            CreateOpCode(0x47, () => loadUnit.Load(ref B, A), 4, "LD B, A");
            CreateOpCode(0x4F, () => loadUnit.Load(ref C, A), 4, "LD C, A");
            CreateOpCode(0x57, () => loadUnit.Load(ref D, A), 4, "LD D, A");
            CreateOpCode(0x5F, () => loadUnit.Load(ref E, A), 4, "LD E, A");
            CreateOpCode(0x67, () => loadUnit.Load(ref H, A), 4, "LD H, A");
            CreateOpCode(0x6F, () => loadUnit.Load(ref L, A), 4, "LD L, A");
            CreateOpCode(0x02, () => loadUnit.WriteToAddress(B, C, A), 8, "LD (BC), A");
            CreateOpCode(0x12, () => loadUnit.WriteToAddress(D, E, A), 8, "LD (DE), A");
            CreateOpCode(0x77, () => loadUnit.WriteToAddress(H, L, A), 8, "LD (HL), A");
            CreateOpCode(0xEA, () => { ReadImmediateWord(out var immediate); loadUnit.WriteToAddress(immediate, A); }, 16, "LD (a16), A");

            CreateOpCode(0xF2, () => loadUnit.LoadFromAddress(ref A, (ushort)(0xFF00 + C)), 8, "LD A, (C)");
            CreateOpCode(0xE2, () => loadUnit.WriteToAddress((ushort)(0xFF00 + C), A), 8, "LD (C), A");

            CreateOpCode(0x3A, () => loadUnit.LoadFromAddressAndIncrement(ref A, ref H, ref L, -1), 8, "LD A, (HL-)");
            CreateOpCode(0x32, () => loadUnit.WriteToAddressAndIncrement(ref H, ref L, A, -1), 8, "LD (HL-), A");
            CreateOpCode(0x2A, () => loadUnit.LoadFromAddressAndIncrement(ref A, ref H, ref L, 1), 8, "LD A, (HL+)");
            CreateOpCode(0x22, () => loadUnit.WriteToAddressAndIncrement(ref H, ref L, A, 1), 8, "LD (HL+), A");
            CreateOpCode(0xE0, () => { ReadImmediateByte(out var immediate); loadUnit.WriteToAddress((ushort)(0xFF00 + immediate), A); }, 12, "LDH (a8), A");
            CreateOpCode(0xF0, () => { ReadImmediateByte(out var immediate); loadUnit.LoadFromAddress(ref A, (ushort)(0xFF00 + immediate)); }, 12, "LDH A, (a8)");

            CreateOpCode(0x01, () => { ReadImmediateWord(out var immediate); loadUnit.Load(ref B, ref C, immediate); }, 12, "LD BC, d16");
            CreateOpCode(0x11, () => { ReadImmediateWord(out var immediate); loadUnit.Load(ref D, ref E, immediate); }, 12, "LD DE, d16");
            CreateOpCode(0x21, () => { ReadImmediateWord(out var immediate); loadUnit.Load(ref H, ref L, immediate); }, 12, "LD HL, d16");
            CreateOpCode(0x31, () => { ReadImmediateWord(out var immediate); loadUnit.Load(ref SP, immediate); }, 12, "LD SP, d16");
            CreateOpCode(0xF9, () => loadUnit.Load(ref SP, H, L), 8, "LD SP, HL");
            CreateOpCode(0xF8, () => { ReadImmediateByte(out var immediate); loadUnit.LoadAdjusted(ref H, ref L, SP, immediate, ref F); }, 12, "LD HL, SP + r8");
            CreateOpCode(0x08, () => { ReadImmediateWord(out var immediate); loadUnit.WriteToAddress(immediate, SP); }, 20, "LD (a16), SP");

            CreateOpCode(0xF5, () => loadUnit.Push(ref SP, A, F), 16, "PUSH AF");
            CreateOpCode(0xC5, () => loadUnit.Push(ref SP, B, C), 16, "PUSH BC");
            CreateOpCode(0xD5, () => loadUnit.Push(ref SP, D, E), 16, "PUSH DE");
            CreateOpCode(0xE5, () => loadUnit.Push(ref SP, H, L), 16, "PUSH HL");
            CreateOpCode(0xF1, () => loadUnit.Pop(ref A, ref F, ref SP), 12, "POP AF");
            CreateOpCode(0xC1, () => loadUnit.Pop(ref B, ref C, ref SP), 12, "POP BC");
            CreateOpCode(0xD1, () => loadUnit.Pop(ref D, ref E, ref SP), 12, "POP DE");
            CreateOpCode(0xE1, () => loadUnit.Pop(ref H, ref L, ref SP), 12, "POP HL");
        }

        private void CreateALUOpCodes()
        {
            CreateOpCode(0x87, () => alu.Add(ref A, A, ref F), 4, "ADD A, A");
            CreateOpCode(0x80, () => alu.Add(ref A, B, ref F), 4, "ADD A, B");
            CreateOpCode(0x81, () => alu.Add(ref A, C, ref F), 4, "ADD A, C");
            CreateOpCode(0x82, () => alu.Add(ref A, D, ref F), 4, "ADD A, D");
            CreateOpCode(0x83, () => alu.Add(ref A, E, ref F), 4, "ADD A, E");
            CreateOpCode(0x84, () => alu.Add(ref A, H, ref F), 4, "ADD A, H");
            CreateOpCode(0x85, () => alu.Add(ref A, L, ref F), 4, "ADD A, L");
            CreateOpCode(0x86, () => { ReadFromMemory(H, L, out var memValue); alu.Add(ref A, memValue, ref F); }, 8, "ADD A, (HL)");
            CreateOpCode(0xC6, () => { ReadImmediateByte(out var immediate); alu.Add(ref A, immediate, ref F); }, 8, "ADD A, d8");

            CreateOpCode(0x8F, () => alu.Add(ref A, A, ref F, true), 4, "ADC A,A");
            CreateOpCode(0x88, () => alu.Add(ref A, B, ref F, true), 4, "ADC A,B");
            CreateOpCode(0x89, () => alu.Add(ref A, C, ref F, true), 4, "ADC A,C");
            CreateOpCode(0x8A, () => alu.Add(ref A, D, ref F, true), 4, "ADC A,D");
            CreateOpCode(0x8B, () => alu.Add(ref A, E, ref F, true), 4, "ADC A,E");
            CreateOpCode(0x8C, () => alu.Add(ref A, H, ref F, true), 4, "ADC A,H");
            CreateOpCode(0x8D, () => alu.Add(ref A, L, ref F, true), 4, "ADC A,L");
            CreateOpCode(0x8E, () => { ReadFromMemory(H, L, out var memValue); alu.Add(ref A, memValue, ref F, true); }, 8, "ADC A, (HL)");
            CreateOpCode(0xCE, () => { ReadImmediateByte(out var immediate); alu.Add(ref A, immediate, ref F, true); }, 8, "ADC A, d8");

            CreateOpCode(0x97, () => alu.Subtract(ref A, A, ref F), 4, "SUB A");
            CreateOpCode(0x90, () => alu.Subtract(ref A, B, ref F), 4, "SUB B");
            CreateOpCode(0x91, () => alu.Subtract(ref A, C, ref F), 4, "SUB C");
            CreateOpCode(0x92, () => alu.Subtract(ref A, D, ref F), 4, "SUB D");
            CreateOpCode(0x93, () => alu.Subtract(ref A, E, ref F), 4, "SUB E");
            CreateOpCode(0x94, () => alu.Subtract(ref A, H, ref F), 4, "SUB H");
            CreateOpCode(0x95, () => alu.Subtract(ref A, L, ref F), 4, "SUB L");
            CreateOpCode(0x96, () => { ReadFromMemory(H, L, out var memValue); alu.Subtract(ref A, memValue, ref F); }, 8, "SUB (HL)");
            CreateOpCode(0xD6, () => { ReadImmediateByte(out var immediate); alu.Subtract(ref A, immediate, ref F); }, 8, "SUB d8");

            CreateOpCode(0x9F, () => alu.Subtract(ref A, A, ref F, true), 4, "SBC A, A");
            CreateOpCode(0x98, () => alu.Subtract(ref A, B, ref F, true), 4, "SBC A, B");
            CreateOpCode(0x99, () => alu.Subtract(ref A, C, ref F, true), 4, "SBC A, C");
            CreateOpCode(0x9A, () => alu.Subtract(ref A, D, ref F, true), 4, "SBC A, D");
            CreateOpCode(0x9B, () => alu.Subtract(ref A, E, ref F, true), 4, "SBC A, E");
            CreateOpCode(0x9C, () => alu.Subtract(ref A, H, ref F, true), 4, "SBC A, H");
            CreateOpCode(0x9D, () => alu.Subtract(ref A, L, ref F, true), 4, "SBC A, L");
            CreateOpCode(0x9E, () => { ReadFromMemory(H, L, out var memValue); alu.Subtract(ref A, memValue, ref F, true); }, 8, "SBC A, (HL)");
            CreateOpCode(0xDE, () => { ReadImmediateByte(out var immediate); alu.Subtract(ref A, immediate, ref F, true); }, 8, "SBC A, d8");

            CreateOpCode(0xA7, () => alu.And(ref A, A, ref F), 4, "AND A");
            CreateOpCode(0xA0, () => alu.And(ref A, B, ref F), 4, "AND B");
            CreateOpCode(0xA1, () => alu.And(ref A, C, ref F), 4, "AND C");
            CreateOpCode(0xA2, () => alu.And(ref A, D, ref F), 4, "AND D");
            CreateOpCode(0xA3, () => alu.And(ref A, E, ref F), 4, "AND E");
            CreateOpCode(0xA4, () => alu.And(ref A, H, ref F), 4, "AND H");
            CreateOpCode(0xA5, () => alu.And(ref A, L, ref F), 4, "AND L");
            CreateOpCode(0xA6, () => { ReadFromMemory(H, L, out var memValue); alu.And(ref A, memValue, ref F); }, 8, "AND (HL)");
            CreateOpCode(0xE6, () => { ReadImmediateByte(out var immediate); alu.And(ref A, immediate, ref F); }, 8, "AND d8");

            CreateOpCode(0xB7, () => alu.Or(ref A, A, ref F), 4, "OR A");
            CreateOpCode(0xB0, () => alu.Or(ref A, B, ref F), 4, "OR B");
            CreateOpCode(0xB1, () => alu.Or(ref A, C, ref F), 4, "OR C");
            CreateOpCode(0xB2, () => alu.Or(ref A, D, ref F), 4, "OR D");
            CreateOpCode(0xB3, () => alu.Or(ref A, E, ref F), 4, "OR E");
            CreateOpCode(0xB4, () => alu.Or(ref A, H, ref F), 4, "OR H");
            CreateOpCode(0xB5, () => alu.Or(ref A, L, ref F), 4, "OR L");
            CreateOpCode(0xB6, () => { ReadFromMemory(H, L, out var memValue); alu.Or(ref A, memValue, ref F); }, 8, "OR (HL)");
            CreateOpCode(0xF6, () => { ReadImmediateByte(out var immediate); alu.Or(ref A, immediate, ref F); }, 8, "OR d8");

            CreateOpCode(0xAF, () => alu.Xor(ref A, A, ref F), 4, "XOR A");
            CreateOpCode(0xA8, () => alu.Xor(ref A, B, ref F), 4, "XOR B");
            CreateOpCode(0xA9, () => alu.Xor(ref A, C, ref F), 4, "XOR C");
            CreateOpCode(0xAA, () => alu.Xor(ref A, D, ref F), 4, "XOR D");
            CreateOpCode(0xAB, () => alu.Xor(ref A, E, ref F), 4, "XOR E");
            CreateOpCode(0xAC, () => alu.Xor(ref A, H, ref F), 4, "XOR H");
            CreateOpCode(0xAD, () => alu.Xor(ref A, L, ref F), 4, "XOR L");
            CreateOpCode(0xAE, () => { ReadFromMemory(H, L, out var memValue); alu.Xor(ref A, memValue, ref F); }, 8, "XOR (HL)");
            CreateOpCode(0xEE, () => { ReadImmediateByte(out var immediate); alu.Xor(ref A, immediate, ref F); }, 8, "XOR d8");

            CreateOpCode(0xBF, () => alu.Compare(A, A, ref F), 4, "CP A");
            CreateOpCode(0xB8, () => alu.Compare(A, B, ref F), 4, "CP B");
            CreateOpCode(0xB9, () => alu.Compare(A, C, ref F), 4, "CP C");
            CreateOpCode(0xBA, () => alu.Compare(A, D, ref F), 4, "CP D");
            CreateOpCode(0xBB, () => alu.Compare(A, E, ref F), 4, "CP E");
            CreateOpCode(0xBC, () => alu.Compare(A, H, ref F), 4, "CP H");
            CreateOpCode(0xBD, () => alu.Compare(A, L, ref F), 4, "CP L");
            CreateOpCode(0xBE, () => { ReadFromMemory(H, L, out var memValue); alu.Compare(A, memValue, ref F); }, 8, "CP (HL)");
            CreateOpCode(0xFE, () => { ReadImmediateByte(out var immediate); alu.Compare(A, immediate, ref F); }, 8, "CP d8");

            CreateOpCode(0x3C, () => alu.Increment(ref A, ref F), 4, "INC A");
            CreateOpCode(0x04, () => alu.Increment(ref B, ref F), 4, "INC B");
            CreateOpCode(0x0C, () => alu.Increment(ref C, ref F), 4, "INC C");
            CreateOpCode(0x14, () => alu.Increment(ref D, ref F), 4, "INC D");
            CreateOpCode(0x1C, () => alu.Increment(ref E, ref F), 4, "INC E");
            CreateOpCode(0x24, () => alu.Increment(ref H, ref F), 4, "INC H");
            CreateOpCode(0x2C, () => alu.Increment(ref L, ref F), 4, "INC L");
            CreateOpCode(0x34, () => alu.IncrementInMemory(H, L, ref F), 12, "INC (HL)");

            CreateOpCode(0x3D, () => alu.Decrement(ref A, ref F), 4, "DEC A");
            CreateOpCode(0x05, () => alu.Decrement(ref B, ref F), 4, "DEC B");
            CreateOpCode(0x0D, () => alu.Decrement(ref C, ref F), 4, "DEC C");
            CreateOpCode(0x15, () => alu.Decrement(ref D, ref F), 4, "DEC D");
            CreateOpCode(0x1D, () => alu.Decrement(ref E, ref F), 4, "DEC E");
            CreateOpCode(0x25, () => alu.Decrement(ref H, ref F), 4, "DEC H");
            CreateOpCode(0x2D, () => alu.Decrement(ref L, ref F), 4, "DEC L");
            CreateOpCode(0x35, () => alu.DecrementInMemory(H, L, ref F), 12, "DEC (HL)");

            CreateOpCode(0x09, () => alu.Add(ref H, ref L, B, C, ref F), 8, "ADD HL, BC");
            CreateOpCode(0x19, () => alu.Add(ref H, ref L, D, E, ref F), 8, "ADD HL, DE");
            CreateOpCode(0x29, () => alu.Add(ref H, ref L, H, L, ref F), 8, "ADD HL, HL");
            CreateOpCode(0x39, () => alu.Add(ref H, ref L, BitUtils.MostSignificantByte(SP), BitUtils.LeastSignificantByte(SP), ref F), 8, "ADD HL, SP");

            CreateOpCode(0xE8, () => { ReadImmediateByte(out var data); alu.AddSigned(ref SP, data, ref F); }, 16, "ADD SP, r8");

            CreateOpCode(0x03, () => alu.IncrementWord(ref B, ref C), 8, "INC BC");
            CreateOpCode(0x13, () => alu.IncrementWord(ref D, ref E), 8, "INC DE");
            CreateOpCode(0x23, () => alu.IncrementWord(ref H, ref L), 8, "INC HL");
            CreateOpCode(0x33, () => alu.IncrementWord(ref SP), 8, "INC SP");

            CreateOpCode(0x0B, () => alu.DecrementWord(ref B, ref C), 8, "DEC BC");
            CreateOpCode(0x1B, () => alu.DecrementWord(ref D, ref E), 8, "DEC DE");
            CreateOpCode(0x2B, () => alu.DecrementWord(ref H, ref L), 8, "DEC HL");
            CreateOpCode(0x3B, () => alu.DecrementWord(ref SP), 8, "DEC SP");
        }

        private void CreateMiscOpCodes()
        {
            CreateOpCode(0x00, () => miscUnit.Nop(), 4, "NOP");
            CreateOpCode(0xF3, () => miscUnit.SetInterruptMasterEnable(ref IME, false), 4, "DI");
            CreateOpCode(0xFB, () => miscUnit.SetInterruptMasterEnable(ref IME, true), 4, "EI");
        }

        private void CreateJumpOpCodes()
        {
            CreateOpCode(0xCD, () => { ReadImmediateWord(out var address); jumpUnit.Call(address, ref SP, ref PC); }, 24, "CALL a16");
            CreateOpCode(0xC4, () =>
            {
                ReadImmediateWord(out var address);
                jumpUnit.CallConditional(address, ref SP, ref PC, Flag.Z, false, F);
            }, 24, "CALL NZ, a16"); // TODO: Cycles 24/12 depending if condition is true

            CreateOpCode(0xCC, () =>
            {
                ReadImmediateWord(out var address);
                jumpUnit.CallConditional(address, ref SP, ref PC, Flag.Z, true, F);
            }, 24, "CALL Z, a16"); // TODO: Cycles 24/12 depending if condition is true

            CreateOpCode(0xD4, () =>
            {
                ReadImmediateWord(out var address);
                jumpUnit.CallConditional(address, ref SP, ref PC, Flag.C, false, F);
            }, 24, "CALL NC, a16"); // TODO: Cycles 24/12 depending if condition is true

            CreateOpCode(0xDC, () =>
            {
                ReadImmediateWord(out var address);
                jumpUnit.CallConditional(address, ref SP, ref PC, Flag.C, true, F);
            }, 24, "CALL C, a16"); // TODO: Cycles 24/12 depending if condition is true

            CreateOpCode(0xC3, () => { ReadImmediateWord(out var address); jumpUnit.JumpToAddress(address, ref PC); }, 16, "JP a16");
            CreateOpCode(0x18, () => { ReadImmediateByte(out var value); jumpUnit.JumpRelative(value, ref PC); }, 12, "JR a8");
            CreateOpCode(0xE9, () => jumpUnit.JumpToAddress(H, L, ref PC), 4, "JP (HL)");

            CreateOpCode(0xC2, () =>
            {
                ReadImmediateWord(out var address);
                jumpUnit.JumpToAddressConditional(address, ref PC, Flag.Z, false, F);
            }, 16, "JP NZ, a16"); // TODO: Cycles 16/12 depending if condition is true

            CreateOpCode(0xCA, () =>
            {
                ReadImmediateWord(out var address);
                jumpUnit.JumpToAddressConditional(address, ref PC, Flag.Z, true, F);
            }, 16, "JP Z, a16"); // TODO: Cycles 16/12 depending if condition is true

            CreateOpCode(0xD2, () =>
            {
                ReadImmediateWord(out var address);
                jumpUnit.JumpToAddressConditional(address, ref PC, Flag.C, false, F);
            }, 16, "JP NC, a16"); // TODO: Cycles 16/12 depending if condition is true

            CreateOpCode(0xDA, () =>
            {
                ReadImmediateWord(out var address);
                jumpUnit.JumpToAddressConditional(address, ref PC, Flag.C, true, F);
            }, 16, "JP C, a16"); // TODO: Cycles 16/12 depending if condition is true

            CreateOpCode(0x20, () =>
            {
                ReadImmediateByte(out var value);
                jumpUnit.JumpRelativeConditional(value, ref PC, Flag.Z, false, F);
            }, 12, "JR NZ, r8"); // TODO: Cycles 12/8 depending if condition is true

            CreateOpCode(0x28, () =>
            {
                ReadImmediateByte(out var value);
                jumpUnit.JumpRelativeConditional(value, ref PC, Flag.Z, true, F);
            }, 12, "JR Z, r8"); // TODO: Cycles 12/8 depending if condition is true

            CreateOpCode(0x30, () =>
            {
                ReadImmediateByte(out var value);
                jumpUnit.JumpRelativeConditional(value, ref PC, Flag.C, false, F);
            }, 12, "JR NC, r8"); // TODO: Cycles 12/8 depending if condition is true

            CreateOpCode(0x38, () =>
            {
                ReadImmediateByte(out var value);
                jumpUnit.JumpRelativeConditional(value, ref PC, Flag.C, true, F);
            }, 12, "JR C, r8"); // TODO: Cycles 12/8 depending if condition is true

            CreateOpCode(0xC9, () => jumpUnit.Return(ref SP, ref PC), 16, "RET");
        }

        private void CreateBitUnitOpCodes()
        {
            CreatePrefixedOpCode(0x47, () => bitOpsUnit.TestBit(A, 0, ref F), 12, "BIT 0, A");
            CreatePrefixedOpCode(0x4F, () => bitOpsUnit.TestBit(A, 1, ref F), 12, "BIT 1, A");
            CreatePrefixedOpCode(0x57, () => bitOpsUnit.TestBit(A, 2, ref F), 12, "BIT 2, A");
            CreatePrefixedOpCode(0x5F, () => bitOpsUnit.TestBit(A, 3, ref F), 12, "BIT 3, A");
            CreatePrefixedOpCode(0x67, () => bitOpsUnit.TestBit(A, 4, ref F), 12, "BIT 4, A");
            CreatePrefixedOpCode(0x6f, () => bitOpsUnit.TestBit(A, 5, ref F), 12, "BIT 5, A");
            CreatePrefixedOpCode(0x77, () => bitOpsUnit.TestBit(A, 6, ref F), 12, "BIT 6, A");
            CreatePrefixedOpCode(0x7F, () => bitOpsUnit.TestBit(A, 7, ref F), 12, "BIT 7, A");

            CreatePrefixedOpCode(0x40, () => bitOpsUnit.TestBit(B, 0, ref F), 12, "BIT 0, B");
            CreatePrefixedOpCode(0x48, () => bitOpsUnit.TestBit(B, 1, ref F), 12, "BIT 1, B");
            CreatePrefixedOpCode(0x50, () => bitOpsUnit.TestBit(B, 2, ref F), 12, "BIT 2, B");
            CreatePrefixedOpCode(0x58, () => bitOpsUnit.TestBit(B, 3, ref F), 12, "BIT 3, B");
            CreatePrefixedOpCode(0x60, () => bitOpsUnit.TestBit(B, 4, ref F), 12, "BIT 4, B");
            CreatePrefixedOpCode(0x68, () => bitOpsUnit.TestBit(B, 5, ref F), 12, "BIT 5, B");
            CreatePrefixedOpCode(0x70, () => bitOpsUnit.TestBit(B, 6, ref F), 12, "BIT 6, B");
            CreatePrefixedOpCode(0x78, () => bitOpsUnit.TestBit(B, 7, ref F), 12, "BIT 7, B");

            CreatePrefixedOpCode(0x41, () => bitOpsUnit.TestBit(C, 0, ref F), 12, "BIT 0, C");
            CreatePrefixedOpCode(0x49, () => bitOpsUnit.TestBit(C, 1, ref F), 12, "BIT 1, C");
            CreatePrefixedOpCode(0x51, () => bitOpsUnit.TestBit(C, 2, ref F), 12, "BIT 2, C");
            CreatePrefixedOpCode(0x59, () => bitOpsUnit.TestBit(C, 3, ref F), 12, "BIT 3, C");
            CreatePrefixedOpCode(0x61, () => bitOpsUnit.TestBit(C, 4, ref F), 12, "BIT 4, C");
            CreatePrefixedOpCode(0x69, () => bitOpsUnit.TestBit(C, 5, ref F), 12, "BIT 5, C");
            CreatePrefixedOpCode(0x71, () => bitOpsUnit.TestBit(C, 6, ref F), 12, "BIT 6, C");
            CreatePrefixedOpCode(0x79, () => bitOpsUnit.TestBit(C, 7, ref F), 12, "BIT 7, C");

            CreatePrefixedOpCode(0x42, () => bitOpsUnit.TestBit(D, 0, ref F), 12, "BIT 0, D");
            CreatePrefixedOpCode(0x4A, () => bitOpsUnit.TestBit(D, 1, ref F), 12, "BIT 1, D");
            CreatePrefixedOpCode(0x52, () => bitOpsUnit.TestBit(D, 2, ref F), 12, "BIT 2, D");
            CreatePrefixedOpCode(0x5A, () => bitOpsUnit.TestBit(D, 3, ref F), 12, "BIT 3, D");
            CreatePrefixedOpCode(0x62, () => bitOpsUnit.TestBit(D, 4, ref F), 12, "BIT 4, D");
            CreatePrefixedOpCode(0x6A, () => bitOpsUnit.TestBit(D, 5, ref F), 12, "BIT 5, D");
            CreatePrefixedOpCode(0x72, () => bitOpsUnit.TestBit(D, 6, ref F), 12, "BIT 6, D");
            CreatePrefixedOpCode(0x7A, () => bitOpsUnit.TestBit(D, 7, ref F), 12, "BIT 7, D");

            CreatePrefixedOpCode(0x43, () => bitOpsUnit.TestBit(E, 0, ref F), 12, "BIT 0, E");
            CreatePrefixedOpCode(0x4B, () => bitOpsUnit.TestBit(E, 1, ref F), 12, "BIT 1, E");
            CreatePrefixedOpCode(0x53, () => bitOpsUnit.TestBit(E, 2, ref F), 12, "BIT 2, E");
            CreatePrefixedOpCode(0x5B, () => bitOpsUnit.TestBit(E, 3, ref F), 12, "BIT 3, E");
            CreatePrefixedOpCode(0x63, () => bitOpsUnit.TestBit(E, 4, ref F), 12, "BIT 4, E");
            CreatePrefixedOpCode(0x6B, () => bitOpsUnit.TestBit(E, 5, ref F), 12, "BIT 5, E");
            CreatePrefixedOpCode(0x73, () => bitOpsUnit.TestBit(E, 6, ref F), 12, "BIT 6, E");
            CreatePrefixedOpCode(0x7B, () => bitOpsUnit.TestBit(E, 7, ref F), 12, "BIT 7, E");

            CreatePrefixedOpCode(0x44, () => bitOpsUnit.TestBit(H, 0, ref F), 12, "BIT 0, H");
            CreatePrefixedOpCode(0x4C, () => bitOpsUnit.TestBit(H, 1, ref F), 12, "BIT 1, H");
            CreatePrefixedOpCode(0x54, () => bitOpsUnit.TestBit(H, 2, ref F), 12, "BIT 2, H");
            CreatePrefixedOpCode(0x5C, () => bitOpsUnit.TestBit(H, 3, ref F), 12, "BIT 3, H");
            CreatePrefixedOpCode(0x64, () => bitOpsUnit.TestBit(H, 4, ref F), 12, "BIT 4, H");
            CreatePrefixedOpCode(0x6C, () => bitOpsUnit.TestBit(H, 5, ref F), 12, "BIT 5, H");
            CreatePrefixedOpCode(0x74, () => bitOpsUnit.TestBit(H, 6, ref F), 12, "BIT 6, H");
            CreatePrefixedOpCode(0x7C, () => bitOpsUnit.TestBit(H, 7, ref F), 12, "BIT 7, H");

            CreatePrefixedOpCode(0x45, () => bitOpsUnit.TestBit(L, 0, ref F), 12, "BIT 0, L");
            CreatePrefixedOpCode(0x4D, () => bitOpsUnit.TestBit(L, 1, ref F), 12, "BIT 1, L");
            CreatePrefixedOpCode(0x55, () => bitOpsUnit.TestBit(L, 2, ref F), 12, "BIT 2, L");
            CreatePrefixedOpCode(0x5D, () => bitOpsUnit.TestBit(L, 3, ref F), 12, "BIT 3, L");
            CreatePrefixedOpCode(0x65, () => bitOpsUnit.TestBit(L, 4, ref F), 12, "BIT 4, L");
            CreatePrefixedOpCode(0x6D, () => bitOpsUnit.TestBit(L, 5, ref F), 12, "BIT 5, L");
            CreatePrefixedOpCode(0x75, () => bitOpsUnit.TestBit(L, 6, ref F), 12, "BIT 6, L");
            CreatePrefixedOpCode(0x7D, () => bitOpsUnit.TestBit(L, 7, ref F), 12, "BIT 7, L");
        }

        private void CreateOpCode(byte command, Action instruction, int cycles, string name)
        {
            OpCodes.Add(command, new OpCode(instruction, cycles, name));
        }

        private void CreatePrefixedOpCode(byte command, Action instruction, int cycles, string name)
        {
            OpCodesPrefixed.Add(command, new OpCode(instruction, cycles, name));
        }
    }
}
