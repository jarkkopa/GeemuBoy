﻿using GeemuBoy.GB.CpuUnits;
using System;
using System.Collections.Generic;

namespace GeemuBoy.GB
{
    public class CPU
    {
        public enum Interrupt
        {
            VBlank = 0,
            LCDStat = 1,
            Timer = 2,
            Serial = 3,
            Joypad = 4
        }

        public const byte PREFIX_OPCODE = 0xCB;

        private const ushort INTERRUPT_ENABLE_ADDR = 0xFFFF;
        private const ushort INTERRUPT_FLAG_ADDR = 0xFF0F;

        private readonly Memory memory;

        private readonly PPU ppu;

        private readonly ILoadUnit loadUnit;
        private readonly IALU alu;
        private readonly IMiscUnit miscUnit;
        private readonly IJumpUnit jumpUnit;
        private readonly IBitUnit bitUnit;

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

        public bool InterruptMasterEnableFlag = false;

        public Dictionary<byte, OpCode> OpCodes { get; private set; } = new Dictionary<byte, OpCode>();
        public Dictionary<byte, OpCode> OpCodesPrefixed { get; private set; } = new Dictionary<byte, OpCode>();

        private Dictionary<Interrupt, ushort> interruptVector;

        // EI enables interrupt master flag with delay. This variable is used to handle that delay.
        private int enableInterruptMasterAfter = -1;

        public CPU(Memory memory, PPU ppu, ILoadUnit loadUnit, IALU alu, IMiscUnit miscUnit, IJumpUnit jumpUnit, IBitUnit bitOpsUnit)
        {
            this.ppu = ppu;

            this.loadUnit = loadUnit;
            this.alu = alu;
            this.miscUnit = miscUnit;
            this.jumpUnit = jumpUnit;
            this.bitUnit = bitOpsUnit;

            CreateLoadUnitOpCodes();
            CreateALUOpCodes();
            CreateMiscOpCodes();
            CreateJumpOpCodes();
            CreateBitUnitOpCodes();

            this.memory = memory;

            interruptVector = new Dictionary<Interrupt, ushort>
            {
                { Interrupt.VBlank, 0x40 },
                { Interrupt.LCDStat, 0x48 },
                { Interrupt.Timer, 0x50 },
                { Interrupt.Serial, 0x58 },
                { Interrupt.Joypad, 0x60 }
            };
        }

        public CPU(Memory memory, PPU ppu) :
            this(memory,
                ppu,
                new LoadUnit(memory),
                new ALU(memory),
                new MiscUnit(memory),
                new JumpUnit(memory),
                new BitUnit(memory))
        { }

        public CPU(Memory memory, IDisplay display) : this(memory, new PPU(memory, display)) { }

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

            memory.WriteByte(0xFF05, 0x00);
            memory.WriteByte(0xFF06, 0x00);
            memory.WriteByte(0xFF07, 0x00);
            memory.WriteByte(0xFF10, 0x80);
            memory.WriteByte(0xFF11, 0xBF);
            memory.WriteByte(0xFF12, 0xF3);
            memory.WriteByte(0xFF14, 0xBF);
            memory.WriteByte(0xFF16, 0x3F);
            memory.WriteByte(0xFF17, 0x00);
            memory.WriteByte(0xFF19, 0xBF);
            memory.WriteByte(0xFF1A, 0x7F);
            memory.WriteByte(0xFF1B, 0xFF);
            memory.WriteByte(0xFF1C, 0x9F);
            memory.WriteByte(0xFF1E, 0xBF);
            memory.WriteByte(0xFF20, 0xFF);
            memory.WriteByte(0xFF21, 0x00);
            memory.WriteByte(0xFF22, 0x00);
            memory.WriteByte(0xFF23, 0xBF);
            memory.WriteByte(0xFF24, 0x77);
            memory.WriteByte(0xFF25, 0xF3);
            memory.WriteByte(0xFF26, 0xF1);
            memory.WriteByte(0xFF40, 0x91);
            memory.WriteByte(0xFF42, 0x00);
            memory.WriteByte(0xFF43, 0x00);
            memory.WriteByte(0xFF45, 0x00);
            memory.WriteByte(0xFF47, 0xFC);
            memory.WriteByte(0xFF48, 0xFF);
            memory.WriteByte(0xFF49, 0xFF);
            memory.WriteByte(0xFF4A, 0x00);
            memory.WriteByte(0xFF4B, 0x00);
            memory.WriteByte(0xFFFF, 0x00);
        }

        public void RunCommand()
        {
            var code = memory.ReadByte(PC);
            PC++;

            if (code == PREFIX_OPCODE)
            {
                RunPrefixedCommand();
            }
            else if (OpCodes.ContainsKey(code))
            {
                OpCode opCode = OpCodes[code];
                Cycles = opCode.Instruction();
            }
            else
            {
                throw new NotImplementedException($"Trying to run opcode 0x{code:X2} that is not implemented.");
            }

            ppu.Tick(Cycles);

            HandleInterrupts();
        }

        public void RunPrefixedCommand()
        {
            var code = memory.ReadByte(PC);
            PC++;
            if (OpCodesPrefixed.ContainsKey(code))
            {
                var opCode = OpCodesPrefixed[code];
                Cycles = opCode.Instruction();
            }
            else
            {
                throw new NotImplementedException($"Trying to run prefixed opcode 0x{PREFIX_OPCODE:X2} 0x{code:X2} that is not implemented.");
            }
        }

        public int ReadImmediateByte(out byte value)
        {
            value = memory.ReadByte(PC);
            PC++;
            return 4;
        }

        public int ReadImmediateWord(out ushort value)
        {
            var lsb = memory.ReadByte(PC);
            PC++;
            var msb = memory.ReadByte(PC);
            PC++;
            value = BitUtils.BytesToUshort(msb, lsb);
            return 8;
        }

        public int ReadFromMemory(byte addrHigh, byte addrLow, out byte value)
        {
            value = memory.ReadByte(BitUtils.BytesToUshort(addrHigh, addrLow));
            return 4;
        }

        public static void RequestInterrupt(Memory memory, Interrupt interrupt)
        {
            byte flag = memory.ReadByte(INTERRUPT_FLAG_ADDR);
            flag = BitUtils.SetBit(flag, (int)interrupt, true);
            memory.WriteByte(INTERRUPT_FLAG_ADDR, flag);
        }

        private void HandleInterrupts()
        {
            if (enableInterruptMasterAfter != -1)
            {
                InterruptMasterEnableFlag = enableInterruptMasterAfter == 0;
                enableInterruptMasterAfter--;
            }

            if (InterruptMasterEnableFlag)
            {
                byte enabledInterrupts = memory.ReadByte(INTERRUPT_ENABLE_ADDR);
                byte requestedInterrupts = memory.ReadByte(INTERRUPT_FLAG_ADDR);

                for (int i = 0; i < (int)Interrupt.Joypad && InterruptMasterEnableFlag; i++)
                {
                    if (requestedInterrupts.IsBitSet(i) && enabledInterrupts.IsBitSet(i))
                    {
                        InterruptMasterEnableFlag = false;

                        requestedInterrupts = BitUtils.SetBit(requestedInterrupts, i, false);
                        memory.WriteByte(INTERRUPT_FLAG_ADDR, requestedInterrupts);

                        jumpUnit.Call(interruptVector[(Interrupt)i], ref SP, ref PC);
                        // TODO: Add cycles (20?)
                    }
                }
            }
        }

        private void CreateLoadUnitOpCodes()
        {
            CreateOpCode(0x06, () => { ReadImmediateByte(out var immediate); loadUnit.Load(ref B, immediate); return 8; }, "LD B, d8");
            CreateOpCode(0x0E, () => { ReadImmediateByte(out var immediate); loadUnit.Load(ref C, immediate); return 8; }, "LD C, d8");
            CreateOpCode(0x16, () => { ReadImmediateByte(out var immediate); loadUnit.Load(ref D, immediate); return 8; }, "LD D, d8");
            CreateOpCode(0x1E, () => { ReadImmediateByte(out var immediate); loadUnit.Load(ref E, immediate); return 8; }, "LD E, d8");
            CreateOpCode(0x26, () => { ReadImmediateByte(out var immediate); loadUnit.Load(ref H, immediate); return 8; }, "LD H, d8");
            CreateOpCode(0x2E, () => { ReadImmediateByte(out var immediate); loadUnit.Load(ref L, immediate); return 8; }, "LD L, d8");

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
            CreateOpCode(0x36, () => { ReadImmediateByte(out var immediate); loadUnit.WriteToAddress(H, L, immediate); return 12; }, "LD (HL), d8");

            CreateOpCode(0x0A, () => loadUnit.LoadFromAddress(ref A, B, C), "LD A, (BC)");
            CreateOpCode(0x1A, () => loadUnit.LoadFromAddress(ref A, D, E), "LD A, (DE)");
            CreateOpCode(0xFA, () => { ReadImmediateWord(out var immediate); loadUnit.LoadFromAddress(ref A, immediate); return 16; }, "LD A, (a16)");
            CreateOpCode(0x3E, () => { ReadImmediateByte(out var immediate); loadUnit.Load(ref A, immediate); return 8; }, "LD A, d8");

            CreateOpCode(0x47, () => loadUnit.Load(ref B, A), "LD B, A");
            CreateOpCode(0x4F, () => loadUnit.Load(ref C, A), "LD C, A");
            CreateOpCode(0x57, () => loadUnit.Load(ref D, A), "LD D, A");
            CreateOpCode(0x5F, () => loadUnit.Load(ref E, A), "LD E, A");
            CreateOpCode(0x67, () => loadUnit.Load(ref H, A), "LD H, A");
            CreateOpCode(0x6F, () => loadUnit.Load(ref L, A), "LD L, A");
            CreateOpCode(0x02, () => loadUnit.WriteToAddress(B, C, A), "LD (BC), A");
            CreateOpCode(0x12, () => loadUnit.WriteToAddress(D, E, A), "LD (DE), A");
            CreateOpCode(0x77, () => loadUnit.WriteToAddress(H, L, A), "LD (HL), A");
            CreateOpCode(0xEA, () => { ReadImmediateWord(out var immediate); loadUnit.WriteToAddress(immediate, A); return 16; }, "LD (a16), A");

            CreateOpCode(0xF2, () => loadUnit.LoadFromAddress(ref A, (ushort)(0xFF00 + C)), "LD A, (C)");
            CreateOpCode(0xE2, () => loadUnit.WriteToAddress((ushort)(0xFF00 + C), A), "LD (C), A");

            CreateOpCode(0x3A, () => loadUnit.LoadFromAddressAndIncrement(ref A, ref H, ref L, -1), "LD A, (HL-)");
            CreateOpCode(0x32, () => loadUnit.WriteToAddressAndIncrement(ref H, ref L, A, -1), "LD (HL-), A");
            CreateOpCode(0x2A, () => loadUnit.LoadFromAddressAndIncrement(ref A, ref H, ref L, 1), "LD A, (HL+)");
            CreateOpCode(0x22, () => loadUnit.WriteToAddressAndIncrement(ref H, ref L, A, 1), "LD (HL+), A");
            CreateOpCode(0xE0, () => { ReadImmediateByte(out var immediate); loadUnit.WriteToAddress((ushort)(0xFF00 + immediate), A); return 12; }, "LDH (a8), A");
            CreateOpCode(0xF0, () => { ReadImmediateByte(out var immediate); loadUnit.LoadFromAddress(ref A, (ushort)(0xFF00 + immediate)); return 12; }, "LDH A, (a8)");

            CreateOpCode(0x01, () => { ReadImmediateWord(out var immediate); loadUnit.Load(ref B, ref C, immediate); return 12; }, "LD BC, d16");
            CreateOpCode(0x11, () => { ReadImmediateWord(out var immediate); loadUnit.Load(ref D, ref E, immediate); return 12; }, "LD DE, d16");
            CreateOpCode(0x21, () => { ReadImmediateWord(out var immediate); loadUnit.Load(ref H, ref L, immediate); return 12; }, "LD HL, d16");
            CreateOpCode(0x31, () => { ReadImmediateWord(out var immediate); loadUnit.Load(ref SP, immediate); return 12; }, "LD SP, d16");
            CreateOpCode(0xF9, () => loadUnit.Load(ref SP, H, L), "LD SP, HL");
            CreateOpCode(0xF8, () => { ReadImmediateByte(out var immediate); loadUnit.LoadAdjusted(ref H, ref L, SP, immediate, ref F); return 12; }, "LD HL, SP + r8");
            CreateOpCode(0x08, () => { ReadImmediateWord(out var immediate); loadUnit.WriteToAddress(immediate, SP); return 20; }, "LD (a16), SP");

            CreateOpCode(0xF5, () => loadUnit.Push(ref SP, A, F), "PUSH AF");
            CreateOpCode(0xC5, () => loadUnit.Push(ref SP, B, C), "PUSH BC");
            CreateOpCode(0xD5, () => loadUnit.Push(ref SP, D, E), "PUSH DE");
            CreateOpCode(0xE5, () => loadUnit.Push(ref SP, H, L), "PUSH HL");
            CreateOpCode(0xF1, () => loadUnit.Pop(ref A, ref F, ref SP), "POP AF");
            CreateOpCode(0xC1, () => loadUnit.Pop(ref B, ref C, ref SP), "POP BC");
            CreateOpCode(0xD1, () => loadUnit.Pop(ref D, ref E, ref SP), "POP DE");
            CreateOpCode(0xE1, () => loadUnit.Pop(ref H, ref L, ref SP), "POP HL");
        }

        private void CreateALUOpCodes()
        {
            CreateOpCode(0x87, () => alu.Add(ref A, A, ref F), "ADD A, A");
            CreateOpCode(0x80, () => alu.Add(ref A, B, ref F), "ADD A, B");
            CreateOpCode(0x81, () => alu.Add(ref A, C, ref F), "ADD A, C");
            CreateOpCode(0x82, () => alu.Add(ref A, D, ref F), "ADD A, D");
            CreateOpCode(0x83, () => alu.Add(ref A, E, ref F), "ADD A, E");
            CreateOpCode(0x84, () => alu.Add(ref A, H, ref F), "ADD A, H");
            CreateOpCode(0x85, () => alu.Add(ref A, L, ref F), "ADD A, L");
            CreateOpCode(0x86, () => { ReadFromMemory(H, L, out var memValue); alu.Add(ref A, memValue, ref F); return 8; }, "ADD A, (HL)");
            CreateOpCode(0xC6, () => { ReadImmediateByte(out var immediate); alu.Add(ref A, immediate, ref F); return 8; }, "ADD A, d8");

            CreateOpCode(0x8F, () => alu.Add(ref A, A, ref F, true), "ADC A,A");
            CreateOpCode(0x88, () => alu.Add(ref A, B, ref F, true), "ADC A,B");
            CreateOpCode(0x89, () => alu.Add(ref A, C, ref F, true), "ADC A,C");
            CreateOpCode(0x8A, () => alu.Add(ref A, D, ref F, true), "ADC A,D");
            CreateOpCode(0x8B, () => alu.Add(ref A, E, ref F, true), "ADC A,E");
            CreateOpCode(0x8C, () => alu.Add(ref A, H, ref F, true), "ADC A,H");
            CreateOpCode(0x8D, () => alu.Add(ref A, L, ref F, true), "ADC A,L");
            CreateOpCode(0x8E, () => { ReadFromMemory(H, L, out var memValue); alu.Add(ref A, memValue, ref F, true); return 8; }, "ADC A, (HL)");
            CreateOpCode(0xCE, () => { ReadImmediateByte(out var immediate); alu.Add(ref A, immediate, ref F, true); return 8; }, "ADC A, d8");

            CreateOpCode(0x97, () => alu.Subtract(ref A, A, ref F), "SUB A");
            CreateOpCode(0x90, () => alu.Subtract(ref A, B, ref F), "SUB B");
            CreateOpCode(0x91, () => alu.Subtract(ref A, C, ref F), "SUB C");
            CreateOpCode(0x92, () => alu.Subtract(ref A, D, ref F), "SUB D");
            CreateOpCode(0x93, () => alu.Subtract(ref A, E, ref F), "SUB E");
            CreateOpCode(0x94, () => alu.Subtract(ref A, H, ref F), "SUB H");
            CreateOpCode(0x95, () => alu.Subtract(ref A, L, ref F), "SUB L");
            CreateOpCode(0x96, () => { ReadFromMemory(H, L, out var memValue); alu.Subtract(ref A, memValue, ref F); return 8; }, "SUB (HL)");
            CreateOpCode(0xD6, () => { ReadImmediateByte(out var immediate); alu.Subtract(ref A, immediate, ref F); return 8; }, "SUB d8");

            CreateOpCode(0x9F, () => alu.Subtract(ref A, A, ref F, true), "SBC A, A");
            CreateOpCode(0x98, () => alu.Subtract(ref A, B, ref F, true), "SBC A, B");
            CreateOpCode(0x99, () => alu.Subtract(ref A, C, ref F, true), "SBC A, C");
            CreateOpCode(0x9A, () => alu.Subtract(ref A, D, ref F, true), "SBC A, D");
            CreateOpCode(0x9B, () => alu.Subtract(ref A, E, ref F, true), "SBC A, E");
            CreateOpCode(0x9C, () => alu.Subtract(ref A, H, ref F, true), "SBC A, H");
            CreateOpCode(0x9D, () => alu.Subtract(ref A, L, ref F, true), "SBC A, L");
            CreateOpCode(0x9E, () => { ReadFromMemory(H, L, out var memValue); alu.Subtract(ref A, memValue, ref F, true); return 8; }, "SBC A, (HL)");
            CreateOpCode(0xDE, () => { ReadImmediateByte(out var immediate); alu.Subtract(ref A, immediate, ref F, true); return 8; }, "SBC A, d8");

            CreateOpCode(0xA7, () => alu.And(ref A, A, ref F), "AND A");
            CreateOpCode(0xA0, () => alu.And(ref A, B, ref F), "AND B");
            CreateOpCode(0xA1, () => alu.And(ref A, C, ref F), "AND C");
            CreateOpCode(0xA2, () => alu.And(ref A, D, ref F), "AND D");
            CreateOpCode(0xA3, () => alu.And(ref A, E, ref F), "AND E");
            CreateOpCode(0xA4, () => alu.And(ref A, H, ref F), "AND H");
            CreateOpCode(0xA5, () => alu.And(ref A, L, ref F), "AND L");
            CreateOpCode(0xA6, () => { ReadFromMemory(H, L, out var memValue); alu.And(ref A, memValue, ref F); return 8; }, "AND (HL)");
            CreateOpCode(0xE6, () => { ReadImmediateByte(out var immediate); alu.And(ref A, immediate, ref F); return 8; }, "AND d8");

            CreateOpCode(0xB7, () => alu.Or(ref A, A, ref F), "OR A");
            CreateOpCode(0xB0, () => alu.Or(ref A, B, ref F), "OR B");
            CreateOpCode(0xB1, () => alu.Or(ref A, C, ref F), "OR C");
            CreateOpCode(0xB2, () => alu.Or(ref A, D, ref F), "OR D");
            CreateOpCode(0xB3, () => alu.Or(ref A, E, ref F), "OR E");
            CreateOpCode(0xB4, () => alu.Or(ref A, H, ref F), "OR H");
            CreateOpCode(0xB5, () => alu.Or(ref A, L, ref F), "OR L");
            CreateOpCode(0xB6, () => { ReadFromMemory(H, L, out var memValue); alu.Or(ref A, memValue, ref F); return 8; }, "OR (HL)");
            CreateOpCode(0xF6, () => { ReadImmediateByte(out var immediate); alu.Or(ref A, immediate, ref F); return 8; }, "OR d8");

            CreateOpCode(0xAF, () => alu.Xor(ref A, A, ref F), "XOR A");
            CreateOpCode(0xA8, () => alu.Xor(ref A, B, ref F), "XOR B");
            CreateOpCode(0xA9, () => alu.Xor(ref A, C, ref F), "XOR C");
            CreateOpCode(0xAA, () => alu.Xor(ref A, D, ref F), "XOR D");
            CreateOpCode(0xAB, () => alu.Xor(ref A, E, ref F), "XOR E");
            CreateOpCode(0xAC, () => alu.Xor(ref A, H, ref F), "XOR H");
            CreateOpCode(0xAD, () => alu.Xor(ref A, L, ref F), "XOR L");
            CreateOpCode(0xAE, () => { ReadFromMemory(H, L, out var memValue); alu.Xor(ref A, memValue, ref F); return 8; }, "XOR (HL)");
            CreateOpCode(0xEE, () => { ReadImmediateByte(out var immediate); alu.Xor(ref A, immediate, ref F); return 8; }, "XOR d8");

            CreateOpCode(0xBF, () => alu.Compare(A, A, ref F), "CP A");
            CreateOpCode(0xB8, () => alu.Compare(A, B, ref F), "CP B");
            CreateOpCode(0xB9, () => alu.Compare(A, C, ref F), "CP C");
            CreateOpCode(0xBA, () => alu.Compare(A, D, ref F), "CP D");
            CreateOpCode(0xBB, () => alu.Compare(A, E, ref F), "CP E");
            CreateOpCode(0xBC, () => alu.Compare(A, H, ref F), "CP H");
            CreateOpCode(0xBD, () => alu.Compare(A, L, ref F), "CP L");
            CreateOpCode(0xBE, () => { ReadFromMemory(H, L, out var memValue); alu.Compare(A, memValue, ref F); return 8; }, "CP (HL)");
            CreateOpCode(0xFE, () => { ReadImmediateByte(out var immediate); alu.Compare(A, immediate, ref F); return 8; }, "CP d8");

            CreateOpCode(0x3C, () => alu.Increment(ref A, ref F), "INC A");
            CreateOpCode(0x04, () => alu.Increment(ref B, ref F), "INC B");
            CreateOpCode(0x0C, () => alu.Increment(ref C, ref F), "INC C");
            CreateOpCode(0x14, () => alu.Increment(ref D, ref F), "INC D");
            CreateOpCode(0x1C, () => alu.Increment(ref E, ref F), "INC E");
            CreateOpCode(0x24, () => alu.Increment(ref H, ref F), "INC H");
            CreateOpCode(0x2C, () => alu.Increment(ref L, ref F), "INC L");
            CreateOpCode(0x34, () => alu.IncrementInMemory(H, L, ref F), "INC (HL)");

            CreateOpCode(0x3D, () => alu.Decrement(ref A, ref F), "DEC A");
            CreateOpCode(0x05, () => alu.Decrement(ref B, ref F), "DEC B");
            CreateOpCode(0x0D, () => alu.Decrement(ref C, ref F), "DEC C");
            CreateOpCode(0x15, () => alu.Decrement(ref D, ref F), "DEC D");
            CreateOpCode(0x1D, () => alu.Decrement(ref E, ref F), "DEC E");
            CreateOpCode(0x25, () => alu.Decrement(ref H, ref F), "DEC H");
            CreateOpCode(0x2D, () => alu.Decrement(ref L, ref F), "DEC L");
            CreateOpCode(0x35, () => alu.DecrementInMemory(H, L, ref F), "DEC (HL)");

            CreateOpCode(0x09, () => alu.Add(ref H, ref L, B, C, ref F), "ADD HL, BC");
            CreateOpCode(0x19, () => alu.Add(ref H, ref L, D, E, ref F), "ADD HL, DE");
            CreateOpCode(0x29, () => alu.Add(ref H, ref L, H, L, ref F), "ADD HL, HL");
            CreateOpCode(0x39, () => alu.Add(ref H, ref L, BitUtils.MostSignificantByte(SP), BitUtils.LeastSignificantByte(SP), ref F), "ADD HL, SP");

            CreateOpCode(0xE8, () => { ReadImmediateByte(out var data); alu.AddSigned(ref SP, data, ref F); return 16; }, "ADD SP, r8");

            CreateOpCode(0x03, () => alu.IncrementWord(ref B, ref C), "INC BC");
            CreateOpCode(0x13, () => alu.IncrementWord(ref D, ref E), "INC DE");
            CreateOpCode(0x23, () => alu.IncrementWord(ref H, ref L), "INC HL");
            CreateOpCode(0x33, () => alu.IncrementWord(ref SP), "INC SP");

            CreateOpCode(0x0B, () => alu.DecrementWord(ref B, ref C), "DEC BC");
            CreateOpCode(0x1B, () => alu.DecrementWord(ref D, ref E), "DEC DE");
            CreateOpCode(0x2B, () => alu.DecrementWord(ref H, ref L), "DEC HL");
            CreateOpCode(0x3B, () => alu.DecrementWord(ref SP), "DEC SP");
        }

        private void CreateMiscOpCodes()
        {
            CreateOpCode(0x00, () => miscUnit.Nop(), "NOP");
            CreateOpCode(0xF3, () => miscUnit.DisableInterruptMasterFlag(ref InterruptMasterEnableFlag), "DI");
            CreateOpCode(0xFB, () => miscUnit.EnableInterruptMasterFlag(ref enableInterruptMasterAfter), "EI");
        }

        private void CreateJumpOpCodes()
        {
            CreateOpCode(0xCD, () =>
            {
                ReadImmediateWord(out var address);
                return jumpUnit.Call(address, ref SP, ref PC);
            }, "CALL a16");
            CreateOpCode(0xC4, () =>
            {
                ReadImmediateWord(out var address);
                return jumpUnit.CallConditional(address, ref SP, ref PC, Flag.Z, false, F);
            }, "CALL NZ, a16");

            CreateOpCode(0xCC, () =>
            {
                ReadImmediateWord(out var address);
                return jumpUnit.CallConditional(address, ref SP, ref PC, Flag.Z, true, F);
            }, "CALL Z, a16");

            CreateOpCode(0xD4, () =>
            {
                ReadImmediateWord(out var address);
                return jumpUnit.CallConditional(address, ref SP, ref PC, Flag.C, false, F);
            }, "CALL NC, a16");

            CreateOpCode(0xDC, () =>
            {
                ReadImmediateWord(out var address);
                return jumpUnit.CallConditional(address, ref SP, ref PC, Flag.C, true, F);
            }, "CALL C, a16");

            CreateOpCode(0xC3, () =>
            {
                ReadImmediateWord(out var address);
                return jumpUnit.JumpToAddress(address, ref PC);
            }, "JP a16");
            CreateOpCode(0x18, () =>
            {
                ReadImmediateByte(out var value); return jumpUnit.JumpRelative(value, ref PC);
            }, "JR a8");
            CreateOpCode(0xE9, () => jumpUnit.JumpToAddress(H, L, ref PC), "JP (HL)");

            CreateOpCode(0xC2, () =>
            {
                ReadImmediateWord(out var address);
                return jumpUnit.JumpToAddressConditional(address, ref PC, Flag.Z, false, F);
            }, "JP NZ, a16");

            CreateOpCode(0xCA, () =>
            {
                ReadImmediateWord(out var address);
                return jumpUnit.JumpToAddressConditional(address, ref PC, Flag.Z, true, F);
            }, "JP Z, a16");

            CreateOpCode(0xD2, () =>
            {
                ReadImmediateWord(out var address);
                return jumpUnit.JumpToAddressConditional(address, ref PC, Flag.C, false, F);
            }, "JP NC, a16");

            CreateOpCode(0xDA, () =>
            {
                ReadImmediateWord(out var address);
                return jumpUnit.JumpToAddressConditional(address, ref PC, Flag.C, true, F);
            }, "JP C, a16");

            CreateOpCode(0x20, () =>
            {
                ReadImmediateByte(out var value);
                return jumpUnit.JumpRelativeConditional(value, ref PC, Flag.Z, false, F);
            }, "JR NZ, r8");

            CreateOpCode(0x28, () =>
            {
                ReadImmediateByte(out var value);
                return jumpUnit.JumpRelativeConditional(value, ref PC, Flag.Z, true, F);
            }, "JR Z, r8");

            CreateOpCode(0x30, () =>
            {
                ReadImmediateByte(out var value);
                return jumpUnit.JumpRelativeConditional(value, ref PC, Flag.C, false, F);
            }, "JR NC, r8");

            CreateOpCode(0x38, () =>
            {
                ReadImmediateByte(out var value);
                return jumpUnit.JumpRelativeConditional(value, ref PC, Flag.C, true, F);
            }, "JR C, r8");

            CreateOpCode(0xC9, () => jumpUnit.Return(ref SP, ref PC), "RET");
        }

        private void CreateBitUnitOpCodes()
        {
            CreateOpCode(0x07, () => { bitUnit.RotateLeft(ref A, ref F, true); return 4; }, "RLCA");
            CreateOpCode(0x17, () => { bitUnit.RotateLeftThroughCarry(ref A, ref F, true); return 4; }, "RLA");

            CreatePrefixedOpCode(0x47, () => bitUnit.TestBit(A, 0, ref F), "BIT 0, A");
            CreatePrefixedOpCode(0x4F, () => bitUnit.TestBit(A, 1, ref F), "BIT 1, A");
            CreatePrefixedOpCode(0x57, () => bitUnit.TestBit(A, 2, ref F), "BIT 2, A");
            CreatePrefixedOpCode(0x5F, () => bitUnit.TestBit(A, 3, ref F), "BIT 3, A");
            CreatePrefixedOpCode(0x67, () => bitUnit.TestBit(A, 4, ref F), "BIT 4, A");
            CreatePrefixedOpCode(0x6f, () => bitUnit.TestBit(A, 5, ref F), "BIT 5, A");
            CreatePrefixedOpCode(0x77, () => bitUnit.TestBit(A, 6, ref F), "BIT 6, A");
            CreatePrefixedOpCode(0x7F, () => bitUnit.TestBit(A, 7, ref F), "BIT 7, A");

            CreatePrefixedOpCode(0x40, () => bitUnit.TestBit(B, 0, ref F), "BIT 0, B");
            CreatePrefixedOpCode(0x48, () => bitUnit.TestBit(B, 1, ref F), "BIT 1, B");
            CreatePrefixedOpCode(0x50, () => bitUnit.TestBit(B, 2, ref F), "BIT 2, B");
            CreatePrefixedOpCode(0x58, () => bitUnit.TestBit(B, 3, ref F), "BIT 3, B");
            CreatePrefixedOpCode(0x60, () => bitUnit.TestBit(B, 4, ref F), "BIT 4, B");
            CreatePrefixedOpCode(0x68, () => bitUnit.TestBit(B, 5, ref F), "BIT 5, B");
            CreatePrefixedOpCode(0x70, () => bitUnit.TestBit(B, 6, ref F), "BIT 6, B");
            CreatePrefixedOpCode(0x78, () => bitUnit.TestBit(B, 7, ref F), "BIT 7, B");

            CreatePrefixedOpCode(0x41, () => bitUnit.TestBit(C, 0, ref F), "BIT 0, C");
            CreatePrefixedOpCode(0x49, () => bitUnit.TestBit(C, 1, ref F), "BIT 1, C");
            CreatePrefixedOpCode(0x51, () => bitUnit.TestBit(C, 2, ref F), "BIT 2, C");
            CreatePrefixedOpCode(0x59, () => bitUnit.TestBit(C, 3, ref F), "BIT 3, C");
            CreatePrefixedOpCode(0x61, () => bitUnit.TestBit(C, 4, ref F), "BIT 4, C");
            CreatePrefixedOpCode(0x69, () => bitUnit.TestBit(C, 5, ref F), "BIT 5, C");
            CreatePrefixedOpCode(0x71, () => bitUnit.TestBit(C, 6, ref F), "BIT 6, C");
            CreatePrefixedOpCode(0x79, () => bitUnit.TestBit(C, 7, ref F), "BIT 7, C");

            CreatePrefixedOpCode(0x42, () => bitUnit.TestBit(D, 0, ref F), "BIT 0, D");
            CreatePrefixedOpCode(0x4A, () => bitUnit.TestBit(D, 1, ref F), "BIT 1, D");
            CreatePrefixedOpCode(0x52, () => bitUnit.TestBit(D, 2, ref F), "BIT 2, D");
            CreatePrefixedOpCode(0x5A, () => bitUnit.TestBit(D, 3, ref F), "BIT 3, D");
            CreatePrefixedOpCode(0x62, () => bitUnit.TestBit(D, 4, ref F), "BIT 4, D");
            CreatePrefixedOpCode(0x6A, () => bitUnit.TestBit(D, 5, ref F), "BIT 5, D");
            CreatePrefixedOpCode(0x72, () => bitUnit.TestBit(D, 6, ref F), "BIT 6, D");
            CreatePrefixedOpCode(0x7A, () => bitUnit.TestBit(D, 7, ref F), "BIT 7, D");

            CreatePrefixedOpCode(0x43, () => bitUnit.TestBit(E, 0, ref F), "BIT 0, E");
            CreatePrefixedOpCode(0x4B, () => bitUnit.TestBit(E, 1, ref F), "BIT 1, E");
            CreatePrefixedOpCode(0x53, () => bitUnit.TestBit(E, 2, ref F), "BIT 2, E");
            CreatePrefixedOpCode(0x5B, () => bitUnit.TestBit(E, 3, ref F), "BIT 3, E");
            CreatePrefixedOpCode(0x63, () => bitUnit.TestBit(E, 4, ref F), "BIT 4, E");
            CreatePrefixedOpCode(0x6B, () => bitUnit.TestBit(E, 5, ref F), "BIT 5, E");
            CreatePrefixedOpCode(0x73, () => bitUnit.TestBit(E, 6, ref F), "BIT 6, E");
            CreatePrefixedOpCode(0x7B, () => bitUnit.TestBit(E, 7, ref F), "BIT 7, E");

            CreatePrefixedOpCode(0x44, () => bitUnit.TestBit(H, 0, ref F), "BIT 0, H");
            CreatePrefixedOpCode(0x4C, () => bitUnit.TestBit(H, 1, ref F), "BIT 1, H");
            CreatePrefixedOpCode(0x54, () => bitUnit.TestBit(H, 2, ref F), "BIT 2, H");
            CreatePrefixedOpCode(0x5C, () => bitUnit.TestBit(H, 3, ref F), "BIT 3, H");
            CreatePrefixedOpCode(0x64, () => bitUnit.TestBit(H, 4, ref F), "BIT 4, H");
            CreatePrefixedOpCode(0x6C, () => bitUnit.TestBit(H, 5, ref F), "BIT 5, H");
            CreatePrefixedOpCode(0x74, () => bitUnit.TestBit(H, 6, ref F), "BIT 6, H");
            CreatePrefixedOpCode(0x7C, () => bitUnit.TestBit(H, 7, ref F), "BIT 7, H");

            CreatePrefixedOpCode(0x45, () => bitUnit.TestBit(L, 0, ref F), "BIT 0, L");
            CreatePrefixedOpCode(0x4D, () => bitUnit.TestBit(L, 1, ref F), "BIT 1, L");
            CreatePrefixedOpCode(0x55, () => bitUnit.TestBit(L, 2, ref F), "BIT 2, L");
            CreatePrefixedOpCode(0x5D, () => bitUnit.TestBit(L, 3, ref F), "BIT 3, L");
            CreatePrefixedOpCode(0x65, () => bitUnit.TestBit(L, 4, ref F), "BIT 4, L");
            CreatePrefixedOpCode(0x6D, () => bitUnit.TestBit(L, 5, ref F), "BIT 5, L");
            CreatePrefixedOpCode(0x75, () => bitUnit.TestBit(L, 6, ref F), "BIT 6, L");
            CreatePrefixedOpCode(0x7D, () => bitUnit.TestBit(L, 7, ref F), "BIT 7, L");

            CreatePrefixedOpCode(0x46, () => { ReadFromMemory(H, L, out var data); bitUnit.TestBit(data, 0, ref F); return 16; }, "BIT 0, (HL)");
            CreatePrefixedOpCode(0x4E, () => { ReadFromMemory(H, L, out var data); bitUnit.TestBit(data, 1, ref F); return 16; }, "BIT 1, (HL)");
            CreatePrefixedOpCode(0x56, () => { ReadFromMemory(H, L, out var data); bitUnit.TestBit(data, 2, ref F); return 16; }, "BIT 2, (HL)");
            CreatePrefixedOpCode(0x5E, () => { ReadFromMemory(H, L, out var data); bitUnit.TestBit(data, 3, ref F); return 16; }, "BIT 3, (HL)");
            CreatePrefixedOpCode(0x66, () => { ReadFromMemory(H, L, out var data); bitUnit.TestBit(data, 4, ref F); return 16; }, "BIT 4, (HL)");
            CreatePrefixedOpCode(0x6E, () => { ReadFromMemory(H, L, out var data); bitUnit.TestBit(data, 5, ref F); return 16; }, "BIT 5, (HL)");
            CreatePrefixedOpCode(0x76, () => { ReadFromMemory(H, L, out var data); bitUnit.TestBit(data, 6, ref F); return 16; }, "BIT 6, (HL)");
            CreatePrefixedOpCode(0x7E, () => { ReadFromMemory(H, L, out var data); bitUnit.TestBit(data, 7, ref F); return 16; }, "BIT 7, (HL)");

            CreatePrefixedOpCode(0x00, () => bitUnit.RotateLeft(ref B, ref F, false), "RLC B");
            CreatePrefixedOpCode(0x01, () => bitUnit.RotateLeft(ref C, ref F, false), "RLC C");
            CreatePrefixedOpCode(0x02, () => bitUnit.RotateLeft(ref D, ref F, false), "RLC D");
            CreatePrefixedOpCode(0x03, () => bitUnit.RotateLeft(ref E, ref F, false), "RLC E");
            CreatePrefixedOpCode(0x04, () => bitUnit.RotateLeft(ref H, ref F, false), "RLC H");
            CreatePrefixedOpCode(0x05, () => bitUnit.RotateLeft(ref L, ref F, false), "RLC L");
            CreatePrefixedOpCode(0x06, () => bitUnit.RotateLeft(H, L, ref F), "RLC (HL)");
            CreatePrefixedOpCode(0x07, () => bitUnit.RotateLeft(ref A, ref F, false), "RLC A");

            CreatePrefixedOpCode(0x10, () => bitUnit.RotateLeftThroughCarry(ref B, ref F, false), "RL B");
            CreatePrefixedOpCode(0x11, () => bitUnit.RotateLeftThroughCarry(ref C, ref F, false), "RL C");
            CreatePrefixedOpCode(0x12, () => bitUnit.RotateLeftThroughCarry(ref D, ref F, false), "RL D");
            CreatePrefixedOpCode(0x13, () => bitUnit.RotateLeftThroughCarry(ref E, ref F, false), "RL E");
            CreatePrefixedOpCode(0x14, () => bitUnit.RotateLeftThroughCarry(ref H, ref F, false), "RL H");
            CreatePrefixedOpCode(0x15, () => bitUnit.RotateLeftThroughCarry(ref L, ref F, false), "RL L");
            CreatePrefixedOpCode(0x16, () => bitUnit.RotateLeftThroughCarry(H, L, ref F), "RL (HL)");
            CreatePrefixedOpCode(0x17, () => bitUnit.RotateLeftThroughCarry(ref A, ref F, false), "RL A");
        }

        private void CreateOpCode(byte command, Func<int> instruction, string name)
        {
            OpCodes.Add(command, new OpCode(instruction, name));
        }

        private void CreatePrefixedOpCode(byte command, Func<int> instruction, string name)
        {
            OpCodesPrefixed.Add(command, new OpCode(instruction, name));
        }
    }
}