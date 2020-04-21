using FakeItEasy;
using GeemuBoy.GB.CpuUnits;
using System;
using System.Linq.Expressions;
using Xunit;

namespace GeemuBoy.GB.Tests
{
    public class InstructionMappingTests
    {
        private const byte IMMEDIATE_BYTE = 0x01;
        private const ushort IMMEDIATE_WORD = 0x2301;
        private const byte MEM_HL_BYTE = 0x45;
        private int ENABLE_IME_AFTER = -1;

        private readonly Memory memory;
        private readonly PPU ppu;
        private readonly CPU cpu;
        private readonly ILoadUnit loadUnit;
        private readonly IALU alu;
        private readonly IMiscUnit miscUnit;
        private readonly IJumpUnit jumpUnit;
        private readonly IBitUnit bitUnit;

        public InstructionMappingTests()
        {
            memory = new Memory(new byte[]
            {
                IMMEDIATE_BYTE,
                (IMMEDIATE_WORD >> 8) & 0xFF
            });
            ppu = new PPU(memory, new BlankDisplay());
            loadUnit = A.Fake<ILoadUnit>();
            alu = A.Fake<IALU>();
            miscUnit = A.Fake<IMiscUnit>();
            jumpUnit = A.Fake<IJumpUnit>();
            bitUnit = A.Fake<IBitUnit>();
            cpu = new CPU(memory, ppu, loadUnit, alu, miscUnit, jumpUnit, bitUnit)
            {
                A = 0x0A,
                B = 0x0B,
                C = 0x0C,
                D = 0x0D,
                E = 0x0E,
                H = 0xAA,
                L = 0xBB,
                PC = 0x00,
                SP = 0xFF
            };
            memory.WriteByte(0xAABB, MEM_HL_BYTE);
        }

        [Fact()]
        public void LoadByteInstructionMappingTest()
        {
            AssertSingleCall(0x06, () => loadUnit.Load(ref cpu.B, IMMEDIATE_BYTE), 8);
            AssertSingleCall(0x0E, () => loadUnit.Load(ref cpu.C, IMMEDIATE_BYTE), 8);
            AssertSingleCall(0x16, () => loadUnit.Load(ref cpu.D, IMMEDIATE_BYTE), 8);
            AssertSingleCall(0x1E, () => loadUnit.Load(ref cpu.E, IMMEDIATE_BYTE), 8);
            AssertSingleCall(0x26, () => loadUnit.Load(ref cpu.H, IMMEDIATE_BYTE), 8);
            AssertSingleCall(0x2E, () => loadUnit.Load(ref cpu.L, IMMEDIATE_BYTE), 8);

            AssertSingleCall(0x7F, () => loadUnit.Load(ref cpu.A, cpu.A), 4);
            AssertSingleCall(0x78, () => loadUnit.Load(ref cpu.A, cpu.B), 4);
            AssertSingleCall(0x79, () => loadUnit.Load(ref cpu.A, cpu.C), 4);
            AssertSingleCall(0x7A, () => loadUnit.Load(ref cpu.A, cpu.D), 4);
            AssertSingleCall(0x7B, () => loadUnit.Load(ref cpu.A, cpu.E), 4);
            AssertSingleCall(0x7C, () => loadUnit.Load(ref cpu.A, cpu.H), 4);
            AssertSingleCall(0x7D, () => loadUnit.Load(ref cpu.A, cpu.L), 4);
            AssertSingleCall(0x7E, () => loadUnit.LoadFromAddress(ref cpu.A, cpu.H, cpu.L), 8);
            AssertSingleCall(0x40, () => loadUnit.Load(ref cpu.B, cpu.B), 4);
            AssertSingleCall(0x41, () => loadUnit.Load(ref cpu.B, cpu.C), 4);
            AssertSingleCall(0x42, () => loadUnit.Load(ref cpu.B, cpu.D), 4);
            AssertSingleCall(0x43, () => loadUnit.Load(ref cpu.B, cpu.E), 4);
            AssertSingleCall(0x44, () => loadUnit.Load(ref cpu.B, cpu.H), 4);
            AssertSingleCall(0x45, () => loadUnit.Load(ref cpu.B, cpu.L), 4);
            AssertSingleCall(0x46, () => loadUnit.LoadFromAddress(ref cpu.B, cpu.H, cpu.L), 8);
            AssertSingleCall(0x48, () => loadUnit.Load(ref cpu.C, cpu.B), 4);
            AssertSingleCall(0x49, () => loadUnit.Load(ref cpu.C, cpu.C), 4);
            AssertSingleCall(0x4A, () => loadUnit.Load(ref cpu.C, cpu.D), 4);
            AssertSingleCall(0x4B, () => loadUnit.Load(ref cpu.C, cpu.E), 4);
            AssertSingleCall(0x4C, () => loadUnit.Load(ref cpu.C, cpu.H), 4);
            AssertSingleCall(0x4D, () => loadUnit.Load(ref cpu.C, cpu.L), 4);
            AssertSingleCall(0x4E, () => loadUnit.LoadFromAddress(ref cpu.C, cpu.H, cpu.L), 8);
            AssertSingleCall(0x50, () => loadUnit.Load(ref cpu.D, cpu.B), 4);
            AssertSingleCall(0x51, () => loadUnit.Load(ref cpu.D, cpu.C), 4);
            AssertSingleCall(0x52, () => loadUnit.Load(ref cpu.D, cpu.D), 4);
            AssertSingleCall(0x53, () => loadUnit.Load(ref cpu.D, cpu.E), 4);
            AssertSingleCall(0x54, () => loadUnit.Load(ref cpu.D, cpu.H), 4);
            AssertSingleCall(0x55, () => loadUnit.Load(ref cpu.D, cpu.L), 4);
            AssertSingleCall(0x56, () => loadUnit.LoadFromAddress(ref cpu.D, cpu.H, cpu.L), 8);
            AssertSingleCall(0x58, () => loadUnit.Load(ref cpu.E, cpu.B), 4);
            AssertSingleCall(0x59, () => loadUnit.Load(ref cpu.E, cpu.C), 4);
            AssertSingleCall(0x5A, () => loadUnit.Load(ref cpu.E, cpu.D), 4);
            AssertSingleCall(0x5B, () => loadUnit.Load(ref cpu.E, cpu.E), 4);
            AssertSingleCall(0x5C, () => loadUnit.Load(ref cpu.E, cpu.H), 4);
            AssertSingleCall(0x5D, () => loadUnit.Load(ref cpu.E, cpu.L), 4);
            AssertSingleCall(0x5E, () => loadUnit.LoadFromAddress(ref cpu.E, cpu.H, cpu.L), 8);
            AssertSingleCall(0x60, () => loadUnit.Load(ref cpu.H, cpu.B), 4);
            AssertSingleCall(0x61, () => loadUnit.Load(ref cpu.H, cpu.C), 4);
            AssertSingleCall(0x62, () => loadUnit.Load(ref cpu.H, cpu.D), 4);
            AssertSingleCall(0x63, () => loadUnit.Load(ref cpu.H, cpu.E), 4);
            AssertSingleCall(0x64, () => loadUnit.Load(ref cpu.H, cpu.H), 4);
            AssertSingleCall(0x65, () => loadUnit.Load(ref cpu.H, cpu.L), 4);
            AssertSingleCall(0x66, () => loadUnit.LoadFromAddress(ref cpu.H, cpu.H, cpu.L), 8);
            AssertSingleCall(0x68, () => loadUnit.Load(ref cpu.L, cpu.B), 4);
            AssertSingleCall(0x69, () => loadUnit.Load(ref cpu.L, cpu.C), 4);
            AssertSingleCall(0x6A, () => loadUnit.Load(ref cpu.L, cpu.D), 4);
            AssertSingleCall(0x6B, () => loadUnit.Load(ref cpu.L, cpu.E), 4);
            AssertSingleCall(0x6C, () => loadUnit.Load(ref cpu.L, cpu.H), 4);
            AssertSingleCall(0x6D, () => loadUnit.Load(ref cpu.L, cpu.L), 4);
            AssertSingleCall(0x6E, () => loadUnit.LoadFromAddress(ref cpu.L, cpu.H, cpu.L), 8);
            AssertSingleCall(0x70, () => loadUnit.WriteToAddress(cpu.H, cpu.L, cpu.B), 8);
            AssertSingleCall(0x71, () => loadUnit.WriteToAddress(cpu.H, cpu.L, cpu.C), 8);
            AssertSingleCall(0x72, () => loadUnit.WriteToAddress(cpu.H, cpu.L, cpu.D), 8);
            AssertSingleCall(0x73, () => loadUnit.WriteToAddress(cpu.H, cpu.L, cpu.E), 8);
            AssertSingleCall(0x74, () => loadUnit.WriteToAddress(cpu.H, cpu.L, cpu.H), 8);
            AssertSingleCall(0x75, () => loadUnit.WriteToAddress(cpu.H, cpu.L, cpu.L), 8);
            AssertSingleCall(0x36, () => loadUnit.WriteToAddress(cpu.H, cpu.L, IMMEDIATE_BYTE), 12);

            AssertSingleCall(0x0A, () => loadUnit.LoadFromAddress(ref cpu.A, cpu.B, cpu.C), 8);
            AssertSingleCall(0x1A, () => loadUnit.LoadFromAddress(ref cpu.A, cpu.D, cpu.E), 8);
            AssertSingleCall(0xFA, () => loadUnit.LoadFromAddress(ref cpu.A, IMMEDIATE_WORD), 16);
            AssertSingleCall(0x3E, () => loadUnit.Load(ref cpu.A, IMMEDIATE_BYTE), 8);

            AssertSingleCall(0x47, () => loadUnit.Load(ref cpu.B, cpu.A), 4);
            AssertSingleCall(0x57, () => loadUnit.Load(ref cpu.D, cpu.A), 4);
            AssertSingleCall(0x5F, () => loadUnit.Load(ref cpu.E, cpu.A), 4);
            AssertSingleCall(0x67, () => loadUnit.Load(ref cpu.H, cpu.A), 4);
            AssertSingleCall(0x6F, () => loadUnit.Load(ref cpu.L, cpu.A), 4);
            AssertSingleCall(0x4F, () => loadUnit.Load(ref cpu.C, cpu.A), 4);
            AssertSingleCall(0x02, () => loadUnit.WriteToAddress(cpu.B, cpu.C, cpu.A), 8);
            AssertSingleCall(0x12, () => loadUnit.WriteToAddress(cpu.D, cpu.E, cpu.A), 8);
            AssertSingleCall(0x77, () => loadUnit.WriteToAddress(cpu.H, cpu.L, cpu.A), 8);
            AssertSingleCall(0xEA, () => loadUnit.WriteToAddress(IMMEDIATE_WORD, cpu.A), 16);

            AssertSingleCall(0xF2, () => loadUnit.LoadFromAddress(ref cpu.A, (ushort)(0xFF00 + cpu.C)), 8);
            AssertSingleCall(0xE2, () => loadUnit.WriteToAddress((ushort)(0xFF00 + cpu.C), cpu.A), 8);

            AssertSingleCall(0x3A, () => loadUnit.LoadFromAddressAndIncrement(ref cpu.A, ref cpu.H, ref cpu.L, -1), 8);
            AssertSingleCall(0x32, () => loadUnit.WriteToAddressAndIncrement(ref cpu.H, ref cpu.L, cpu.A, -1), 8);
            AssertSingleCall(0x2A, () => loadUnit.LoadFromAddressAndIncrement(ref cpu.A, ref cpu.H, ref cpu.L, 1), 8);
            AssertSingleCall(0x22, () => loadUnit.WriteToAddressAndIncrement(ref cpu.H, ref cpu.L, cpu.A, 1), 8);
            AssertSingleCall(0xE0, () => loadUnit.WriteToAddress(0xFF00 + IMMEDIATE_BYTE, cpu.A), 12);
            AssertSingleCall(0xF0, () => loadUnit.LoadFromAddress(ref cpu.A, 0xFF00 + IMMEDIATE_BYTE), 12);

            AssertSingleCall(0x01, () => loadUnit.Load(ref cpu.B, ref cpu.C, IMMEDIATE_WORD), 12);
            AssertSingleCall(0x11, () => loadUnit.Load(ref cpu.D, ref cpu.E, IMMEDIATE_WORD), 12);
            AssertSingleCall(0x21, () => loadUnit.Load(ref cpu.H, ref cpu.L, IMMEDIATE_WORD), 12);
            AssertSingleCall(0x31, () => loadUnit.Load(ref cpu.SP, IMMEDIATE_WORD), 12);

            AssertSingleCall(0xF9, () => loadUnit.Load(ref cpu.SP, cpu.H, cpu.L), 8);
            AssertSingleCall(0xF8, () => loadUnit.LoadAdjusted(ref cpu.H, ref cpu.L, cpu.SP, IMMEDIATE_BYTE, ref cpu.F), 12);
            AssertSingleCall(0x08, () => loadUnit.WriteToAddress(IMMEDIATE_WORD, cpu.SP), 20);

            AssertSingleCall(0xF5, () => loadUnit.Push(ref cpu.SP, cpu.A, cpu.F), 16);
            AssertSingleCall(0xC5, () => loadUnit.Push(ref cpu.SP, cpu.B, cpu.C), 16);
            AssertSingleCall(0xD5, () => loadUnit.Push(ref cpu.SP, cpu.D, cpu.E), 16);
            AssertSingleCall(0xE5, () => loadUnit.Push(ref cpu.SP, cpu.H, cpu.L), 16);
            AssertSingleCall(0xF1, () => loadUnit.Pop(ref cpu.A, ref cpu.F, ref cpu.SP), 12);
            AssertSingleCall(0xC1, () => loadUnit.Pop(ref cpu.B, ref cpu.C, ref cpu.SP), 12);
            AssertSingleCall(0xD1, () => loadUnit.Pop(ref cpu.D, ref cpu.E, ref cpu.SP), 12);
            AssertSingleCall(0xE1, () => loadUnit.Pop(ref cpu.H, ref cpu.L, ref cpu.SP), 12);
        }

        [Fact()]
        public void ALUInstructionMappingTest()
        {
            AssertSingleCall(0x87, () => alu.Add(ref cpu.A, cpu.A, ref cpu.F, false), 4);
            AssertSingleCall(0x80, () => alu.Add(ref cpu.A, cpu.B, ref cpu.F, false), 4);
            AssertSingleCall(0x81, () => alu.Add(ref cpu.A, cpu.C, ref cpu.F, false), 4);
            AssertSingleCall(0x82, () => alu.Add(ref cpu.A, cpu.D, ref cpu.F, false), 4);
            AssertSingleCall(0x83, () => alu.Add(ref cpu.A, cpu.E, ref cpu.F, false), 4);
            AssertSingleCall(0x84, () => alu.Add(ref cpu.A, cpu.H, ref cpu.F, false), 4);
            AssertSingleCall(0x85, () => alu.Add(ref cpu.A, cpu.L, ref cpu.F, false), 4);
            AssertSingleCall(0x86, () => alu.Add(ref cpu.A, MEM_HL_BYTE, ref cpu.F, false), 8);
            AssertSingleCall(0xC6, () => alu.Add(ref cpu.A, IMMEDIATE_BYTE, ref cpu.F, false), 8);

            AssertSingleCall(0x8F, () => alu.Add(ref cpu.A, cpu.A, ref cpu.F, true), 4);
            AssertSingleCall(0x88, () => alu.Add(ref cpu.A, cpu.B, ref cpu.F, true), 4);
            AssertSingleCall(0x89, () => alu.Add(ref cpu.A, cpu.C, ref cpu.F, true), 4);
            AssertSingleCall(0x8A, () => alu.Add(ref cpu.A, cpu.D, ref cpu.F, true), 4);
            AssertSingleCall(0x8B, () => alu.Add(ref cpu.A, cpu.E, ref cpu.F, true), 4);
            AssertSingleCall(0x8C, () => alu.Add(ref cpu.A, cpu.H, ref cpu.F, true), 4);
            AssertSingleCall(0x8D, () => alu.Add(ref cpu.A, cpu.L, ref cpu.F, true), 4);
            AssertSingleCall(0x8E, () => alu.Add(ref cpu.A, MEM_HL_BYTE, ref cpu.F, true), 8);
            AssertSingleCall(0xCE, () => alu.Add(ref cpu.A, IMMEDIATE_BYTE, ref cpu.F, true), 8);

            AssertSingleCall(0x97, () => alu.Subtract(ref cpu.A, cpu.A, ref cpu.F, false), 4);
            AssertSingleCall(0x90, () => alu.Subtract(ref cpu.A, cpu.B, ref cpu.F, false), 4);
            AssertSingleCall(0x91, () => alu.Subtract(ref cpu.A, cpu.C, ref cpu.F, false), 4);
            AssertSingleCall(0x92, () => alu.Subtract(ref cpu.A, cpu.D, ref cpu.F, false), 4);
            AssertSingleCall(0x93, () => alu.Subtract(ref cpu.A, cpu.E, ref cpu.F, false), 4);
            AssertSingleCall(0x94, () => alu.Subtract(ref cpu.A, cpu.H, ref cpu.F, false), 4);
            AssertSingleCall(0x95, () => alu.Subtract(ref cpu.A, cpu.L, ref cpu.F, false), 4);
            AssertSingleCall(0x96, () => alu.Subtract(ref cpu.A, MEM_HL_BYTE, ref cpu.F, false), 8);
            AssertSingleCall(0xD6, () => alu.Subtract(ref cpu.A, IMMEDIATE_BYTE, ref cpu.F, false), 8);

            AssertSingleCall(0x9F, () => alu.Subtract(ref cpu.A, cpu.A, ref cpu.F, true), 4);
            AssertSingleCall(0x98, () => alu.Subtract(ref cpu.A, cpu.B, ref cpu.F, true), 4);
            AssertSingleCall(0x99, () => alu.Subtract(ref cpu.A, cpu.C, ref cpu.F, true), 4);
            AssertSingleCall(0x9A, () => alu.Subtract(ref cpu.A, cpu.D, ref cpu.F, true), 4);
            AssertSingleCall(0x9B, () => alu.Subtract(ref cpu.A, cpu.E, ref cpu.F, true), 4);
            AssertSingleCall(0x9C, () => alu.Subtract(ref cpu.A, cpu.H, ref cpu.F, true), 4);
            AssertSingleCall(0x9D, () => alu.Subtract(ref cpu.A, cpu.L, ref cpu.F, true), 4);
            AssertSingleCall(0x9E, () => alu.Subtract(ref cpu.A, MEM_HL_BYTE, ref cpu.F, true), 8);
            AssertSingleCall(0xDE, () => alu.Subtract(ref cpu.A, IMMEDIATE_BYTE, ref cpu.F, true), 8);

            AssertSingleCall(0xA7, () => alu.And(ref cpu.A, cpu.A, ref cpu.F), 4);
            AssertSingleCall(0xA0, () => alu.And(ref cpu.A, cpu.B, ref cpu.F), 4);
            AssertSingleCall(0xA1, () => alu.And(ref cpu.A, cpu.C, ref cpu.F), 4);
            AssertSingleCall(0xA2, () => alu.And(ref cpu.A, cpu.D, ref cpu.F), 4);
            AssertSingleCall(0xA3, () => alu.And(ref cpu.A, cpu.E, ref cpu.F), 4);
            AssertSingleCall(0xA4, () => alu.And(ref cpu.A, cpu.H, ref cpu.F), 4);
            AssertSingleCall(0xA5, () => alu.And(ref cpu.A, cpu.L, ref cpu.F), 4);
            AssertSingleCall(0xA6, () => alu.And(ref cpu.A, MEM_HL_BYTE, ref cpu.F), 8);
            AssertSingleCall(0xE6, () => alu.And(ref cpu.A, IMMEDIATE_BYTE, ref cpu.F), 8);

            AssertSingleCall(0xB7, () => alu.Or(ref cpu.A, cpu.A, ref cpu.F), 4);
            AssertSingleCall(0xB0, () => alu.Or(ref cpu.A, cpu.B, ref cpu.F), 4);
            AssertSingleCall(0xB1, () => alu.Or(ref cpu.A, cpu.C, ref cpu.F), 4);
            AssertSingleCall(0xB2, () => alu.Or(ref cpu.A, cpu.D, ref cpu.F), 4);
            AssertSingleCall(0xB3, () => alu.Or(ref cpu.A, cpu.E, ref cpu.F), 4);
            AssertSingleCall(0xB4, () => alu.Or(ref cpu.A, cpu.H, ref cpu.F), 4);
            AssertSingleCall(0xB5, () => alu.Or(ref cpu.A, cpu.L, ref cpu.F), 4);
            AssertSingleCall(0xB6, () => alu.Or(ref cpu.A, MEM_HL_BYTE, ref cpu.F), 8);
            AssertSingleCall(0xF6, () => alu.Or(ref cpu.A, IMMEDIATE_BYTE, ref cpu.F), 8);

            AssertSingleCall(0xAF, () => alu.Xor(ref cpu.A, cpu.A, ref cpu.F), 4);
            AssertSingleCall(0xA8, () => alu.Xor(ref cpu.A, cpu.B, ref cpu.F), 4);
            AssertSingleCall(0xA9, () => alu.Xor(ref cpu.A, cpu.C, ref cpu.F), 4);
            AssertSingleCall(0xAA, () => alu.Xor(ref cpu.A, cpu.D, ref cpu.F), 4);
            AssertSingleCall(0xAB, () => alu.Xor(ref cpu.A, cpu.E, ref cpu.F), 4);
            AssertSingleCall(0xAC, () => alu.Xor(ref cpu.A, cpu.H, ref cpu.F), 4);
            AssertSingleCall(0xAD, () => alu.Xor(ref cpu.A, cpu.L, ref cpu.F), 4);
            AssertSingleCall(0xAE, () => alu.Xor(ref cpu.A, MEM_HL_BYTE, ref cpu.F), 8);
            AssertSingleCall(0xEE, () => alu.Xor(ref cpu.A, IMMEDIATE_BYTE, ref cpu.F), 8);

            AssertSingleCall(0xBF, () => alu.Compare(cpu.A, cpu.A, ref cpu.F), 4);
            AssertSingleCall(0xB8, () => alu.Compare(cpu.A, cpu.B, ref cpu.F), 4);
            AssertSingleCall(0xB9, () => alu.Compare(cpu.A, cpu.C, ref cpu.F), 4);
            AssertSingleCall(0xBA, () => alu.Compare(cpu.A, cpu.D, ref cpu.F), 4);
            AssertSingleCall(0xBB, () => alu.Compare(cpu.A, cpu.E, ref cpu.F), 4);
            AssertSingleCall(0xBC, () => alu.Compare(cpu.A, cpu.H, ref cpu.F), 4);
            AssertSingleCall(0xBD, () => alu.Compare(cpu.A, cpu.L, ref cpu.F), 4);
            AssertSingleCall(0xBE, () => alu.Compare(cpu.A, MEM_HL_BYTE, ref cpu.F), 8);
            AssertSingleCall(0xFE, () => alu.Compare(cpu.A, IMMEDIATE_BYTE, ref cpu.F), 8);

            AssertSingleCall(0x3C, () => alu.Increment(ref cpu.A, ref cpu.F), 4);
            AssertSingleCall(0x04, () => alu.Increment(ref cpu.B, ref cpu.F), 4);
            AssertSingleCall(0x0C, () => alu.Increment(ref cpu.C, ref cpu.F), 4);
            AssertSingleCall(0x14, () => alu.Increment(ref cpu.D, ref cpu.F), 4);
            AssertSingleCall(0x1C, () => alu.Increment(ref cpu.E, ref cpu.F), 4);
            AssertSingleCall(0x24, () => alu.Increment(ref cpu.H, ref cpu.F), 4);
            AssertSingleCall(0x2C, () => alu.Increment(ref cpu.L, ref cpu.F), 4);
            AssertSingleCall(0x34, () => alu.IncrementInMemory(cpu.H, cpu.L, ref cpu.F), 12);

            AssertSingleCall(0x3D, () => alu.Decrement(ref cpu.A, ref cpu.F), 4);
            AssertSingleCall(0x05, () => alu.Decrement(ref cpu.B, ref cpu.F), 4);
            AssertSingleCall(0x0D, () => alu.Decrement(ref cpu.C, ref cpu.F), 4);
            AssertSingleCall(0x15, () => alu.Decrement(ref cpu.D, ref cpu.F), 4);
            AssertSingleCall(0x1D, () => alu.Decrement(ref cpu.E, ref cpu.F), 4);
            AssertSingleCall(0x25, () => alu.Decrement(ref cpu.H, ref cpu.F), 4);
            AssertSingleCall(0x2D, () => alu.Decrement(ref cpu.L, ref cpu.F), 4);
            AssertSingleCall(0x35, () => alu.DecrementInMemory(cpu.H, cpu.L, ref cpu.F), 12);

            AssertSingleCall(0x09, () => alu.Add(ref cpu.H, ref cpu.L, cpu.B, cpu.C, ref cpu.F), 8);
            AssertSingleCall(0x19, () => alu.Add(ref cpu.H, ref cpu.L, cpu.D, cpu.E, ref cpu.F), 8);
            AssertSingleCall(0x29, () => alu.Add(ref cpu.H, ref cpu.L, cpu.H, cpu.L, ref cpu.F), 8);
            AssertSingleCall(0x39, () => alu.Add(ref cpu.H, ref cpu.L, BitUtils.MostSignificantByte(cpu.SP), BitUtils.LeastSignificantByte(cpu.SP), ref cpu.F), 8);

            AssertSingleCall(0xE8, () => alu.AddSigned(ref cpu.SP, IMMEDIATE_BYTE, ref cpu.F), 16);

            AssertSingleCall(0x03, () => alu.IncrementWord(ref cpu.B, ref cpu.C), 8);
            AssertSingleCall(0x12, () => alu.IncrementWord(ref cpu.B, ref cpu.C), 8);
            AssertSingleCall(0x23, () => alu.IncrementWord(ref cpu.B, ref cpu.C), 8);
            AssertSingleCall(0x33, () => alu.IncrementWord(ref cpu.SP), 8);

            AssertSingleCall(0x0B, () => alu.DecrementWord(ref cpu.B, ref cpu.C), 8);
            AssertSingleCall(0x1B, () => alu.DecrementWord(ref cpu.D, ref cpu.E), 8);
            AssertSingleCall(0x2B, () => alu.DecrementWord(ref cpu.H, ref cpu.L), 8);
            AssertSingleCall(0x3B, () => alu.DecrementWord(ref cpu.SP), 8);
        }

        [Fact()]
        public void MiscInstructionMappingTest()
        {
            AssertSingleCall(0x00, () => miscUnit.Nop(), 4);
            AssertSingleCall(0xF3, () => miscUnit.DisableInterruptMasterFlag(ref cpu.InterruptMasterEnableFlag), 4);
            AssertSingleCall(0xFB, () => miscUnit.EnableInterruptMasterFlag(ref ENABLE_IME_AFTER), 4);
            AssertSingleCall(0x37, () => miscUnit.SetCarry(ref cpu.F), 4);
        }

        [Fact()]
        public void JumpUnitInstructionMappingTest()
        {
            AssertSingleCall(0xCD, () => jumpUnit.Call(IMMEDIATE_WORD, ref cpu.SP, ref cpu.PC), 24);
            AssertSingleCall(0xC4, () => jumpUnit.CallConditional(IMMEDIATE_WORD, ref cpu.SP, ref cpu.PC, Flag.Z, false, cpu.F), 24);
            AssertSingleCall(0xCC, () => jumpUnit.CallConditional(IMMEDIATE_WORD, ref cpu.SP, ref cpu.PC, Flag.Z, true, cpu.F), 24);
            AssertSingleCall(0xD4, () => jumpUnit.CallConditional(IMMEDIATE_WORD, ref cpu.SP, ref cpu.PC, Flag.C, false, cpu.F), 24);
            AssertSingleCall(0xDC, () => jumpUnit.CallConditional(IMMEDIATE_WORD, ref cpu.SP, ref cpu.PC, Flag.C, true, cpu.F), 24);

            AssertSingleCall(0xC3, () => jumpUnit.JumpToAddress(IMMEDIATE_WORD, ref cpu.PC), 16);
            AssertSingleCall(0x18, () => jumpUnit.JumpRelative(IMMEDIATE_BYTE, ref cpu.PC), 12);
            AssertSingleCall(0xE9, () => jumpUnit.JumpToAddress(cpu.H, cpu.L, ref cpu.PC), 4);

            AssertSingleCall(0xC2, () => jumpUnit.JumpToAddressConditional(IMMEDIATE_WORD, ref cpu.PC, Flag.Z, false, cpu.F), 16);
            AssertSingleCall(0xCA, () => jumpUnit.JumpToAddressConditional(IMMEDIATE_WORD, ref cpu.PC, Flag.Z, true, cpu.F), 16);
            AssertSingleCall(0xD2, () => jumpUnit.JumpToAddressConditional(IMMEDIATE_WORD, ref cpu.PC, Flag.C, false, cpu.F), 16);
            AssertSingleCall(0xDA, () => jumpUnit.JumpToAddressConditional(IMMEDIATE_WORD, ref cpu.PC, Flag.C, true, cpu.F), 16);

            AssertSingleCall(0x20, () => jumpUnit.JumpRelativeConditional(IMMEDIATE_BYTE, ref cpu.PC, Flag.Z, false, cpu.F), 12);
            AssertSingleCall(0x28, () => jumpUnit.JumpRelativeConditional(IMMEDIATE_BYTE, ref cpu.PC, Flag.Z, true, cpu.F), 12);
            AssertSingleCall(0x30, () => jumpUnit.JumpRelativeConditional(IMMEDIATE_BYTE, ref cpu.PC, Flag.C, false, cpu.F), 12);
            AssertSingleCall(0x38, () => jumpUnit.JumpRelativeConditional(IMMEDIATE_BYTE, ref cpu.PC, Flag.C, true, cpu.F), 12);

            AssertSingleCall(0xC9, () => jumpUnit.Return(ref cpu.SP, ref cpu.PC), 16);
            AssertSingleCall(0xC0, () => jumpUnit.ReturnConditional(ref cpu.SP, ref cpu.PC, Flag.Z, false, cpu.F), 20);
            AssertSingleCall(0xC8, () => jumpUnit.ReturnConditional(ref cpu.SP, ref cpu.PC, Flag.Z, true, cpu.F), 20);
            AssertSingleCall(0xD0, () => jumpUnit.ReturnConditional(ref cpu.SP, ref cpu.PC, Flag.C, false, cpu.F), 20);
            AssertSingleCall(0xD8, () => jumpUnit.ReturnConditional(ref cpu.SP, ref cpu.PC, Flag.C, true, cpu.F), 20);
            int enableAFter = -1;
            AssertSingleCall(0xD9, () => jumpUnit.ReturnAndEnableInterrupts(ref cpu.SP, ref cpu.PC, ref enableAFter), 16);

            AssertSingleCall(0xC7, () => jumpUnit.Call(0x0000, ref cpu.SP, ref cpu.PC), 16);
            AssertSingleCall(0xD7, () => jumpUnit.Call(0x0010, ref cpu.SP, ref cpu.PC), 16);
            AssertSingleCall(0xE7, () => jumpUnit.Call(0x0020, ref cpu.SP, ref cpu.PC), 16);
            AssertSingleCall(0xF7, () => jumpUnit.Call(0x0030, ref cpu.SP, ref cpu.PC), 16);
            AssertSingleCall(0xCF, () => jumpUnit.Call(0x0008, ref cpu.SP, ref cpu.PC), 16);
            AssertSingleCall(0xDF, () => jumpUnit.Call(0x0018, ref cpu.SP, ref cpu.PC), 16);
            AssertSingleCall(0xEF, () => jumpUnit.Call(0x0028, ref cpu.SP, ref cpu.PC), 16);
            AssertSingleCall(0xFF, () => jumpUnit.Call(0x0038, ref cpu.SP, ref cpu.PC), 16);
        }

        [Fact()]
        public void BitUnitInstructionMappingTest()
        {
            AssertSingleCall(0x07, () => bitUnit.RotateLeft(ref cpu.A, ref cpu.F, true), 4);
            AssertSingleCall(0x17, () => bitUnit.RotateLeftThroughCarry(ref cpu.A, ref cpu.F, true), 4);
            AssertSingleCall(0x0F, () => bitUnit.RotateRight(ref cpu.A, ref cpu.F, true), 4);
            AssertSingleCall(0x1F, () => bitUnit.RotateRightThroughCarry(ref cpu.A, ref cpu.F, true), 4);

            AssertSingleCall(0x2F, () => bitUnit.Complement(ref cpu.A, ref cpu.F), 4);

            AssertSinglePrefixedCall(0x47, () => bitUnit.TestBit(cpu.A, 0, ref cpu.F), 12);
            AssertSinglePrefixedCall(0x4F, () => bitUnit.TestBit(cpu.A, 1, ref cpu.F), 12);
            AssertSinglePrefixedCall(0x57, () => bitUnit.TestBit(cpu.A, 2, ref cpu.F), 12);
            AssertSinglePrefixedCall(0x5F, () => bitUnit.TestBit(cpu.A, 3, ref cpu.F), 12);
            AssertSinglePrefixedCall(0x67, () => bitUnit.TestBit(cpu.A, 4, ref cpu.F), 12);
            AssertSinglePrefixedCall(0x6F, () => bitUnit.TestBit(cpu.A, 5, ref cpu.F), 12);
            AssertSinglePrefixedCall(0x77, () => bitUnit.TestBit(cpu.A, 6, ref cpu.F), 12);
            AssertSinglePrefixedCall(0x7F, () => bitUnit.TestBit(cpu.A, 7, ref cpu.F), 12);

            AssertSinglePrefixedCall(0x40, () => bitUnit.TestBit(cpu.B, 0, ref cpu.F), 12);
            AssertSinglePrefixedCall(0x48, () => bitUnit.TestBit(cpu.B, 1, ref cpu.F), 12);
            AssertSinglePrefixedCall(0x50, () => bitUnit.TestBit(cpu.B, 2, ref cpu.F), 12);
            AssertSinglePrefixedCall(0x58, () => bitUnit.TestBit(cpu.B, 3, ref cpu.F), 12);
            AssertSinglePrefixedCall(0x60, () => bitUnit.TestBit(cpu.B, 4, ref cpu.F), 12);
            AssertSinglePrefixedCall(0x68, () => bitUnit.TestBit(cpu.B, 5, ref cpu.F), 12);
            AssertSinglePrefixedCall(0x70, () => bitUnit.TestBit(cpu.B, 6, ref cpu.F), 12);
            AssertSinglePrefixedCall(0x78, () => bitUnit.TestBit(cpu.B, 7, ref cpu.F), 12);

            AssertSinglePrefixedCall(0x41, () => bitUnit.TestBit(cpu.C, 0, ref cpu.F), 12);
            AssertSinglePrefixedCall(0x49, () => bitUnit.TestBit(cpu.C, 1, ref cpu.F), 12);
            AssertSinglePrefixedCall(0x51, () => bitUnit.TestBit(cpu.C, 2, ref cpu.F), 12);
            AssertSinglePrefixedCall(0x59, () => bitUnit.TestBit(cpu.C, 3, ref cpu.F), 12);
            AssertSinglePrefixedCall(0x61, () => bitUnit.TestBit(cpu.C, 4, ref cpu.F), 12);
            AssertSinglePrefixedCall(0x69, () => bitUnit.TestBit(cpu.C, 5, ref cpu.F), 12);
            AssertSinglePrefixedCall(0x71, () => bitUnit.TestBit(cpu.C, 6, ref cpu.F), 12);
            AssertSinglePrefixedCall(0x79, () => bitUnit.TestBit(cpu.C, 7, ref cpu.F), 12);

            AssertSinglePrefixedCall(0x42, () => bitUnit.TestBit(cpu.D, 0, ref cpu.F), 12);
            AssertSinglePrefixedCall(0x4A, () => bitUnit.TestBit(cpu.D, 1, ref cpu.F), 12);
            AssertSinglePrefixedCall(0x52, () => bitUnit.TestBit(cpu.D, 2, ref cpu.F), 12);
            AssertSinglePrefixedCall(0x5A, () => bitUnit.TestBit(cpu.D, 3, ref cpu.F), 12);
            AssertSinglePrefixedCall(0x62, () => bitUnit.TestBit(cpu.D, 4, ref cpu.F), 12);
            AssertSinglePrefixedCall(0x6A, () => bitUnit.TestBit(cpu.D, 5, ref cpu.F), 12);
            AssertSinglePrefixedCall(0x72, () => bitUnit.TestBit(cpu.D, 6, ref cpu.F), 12);
            AssertSinglePrefixedCall(0x7A, () => bitUnit.TestBit(cpu.D, 7, ref cpu.F), 12);

            AssertSinglePrefixedCall(0x43, () => bitUnit.TestBit(cpu.E, 0, ref cpu.F), 12);
            AssertSinglePrefixedCall(0x4B, () => bitUnit.TestBit(cpu.E, 1, ref cpu.F), 12);
            AssertSinglePrefixedCall(0x53, () => bitUnit.TestBit(cpu.E, 2, ref cpu.F), 12);
            AssertSinglePrefixedCall(0x5B, () => bitUnit.TestBit(cpu.E, 3, ref cpu.F), 12);
            AssertSinglePrefixedCall(0x63, () => bitUnit.TestBit(cpu.E, 4, ref cpu.F), 12);
            AssertSinglePrefixedCall(0x6B, () => bitUnit.TestBit(cpu.E, 5, ref cpu.F), 12);
            AssertSinglePrefixedCall(0x73, () => bitUnit.TestBit(cpu.E, 6, ref cpu.F), 12);
            AssertSinglePrefixedCall(0x7B, () => bitUnit.TestBit(cpu.E, 7, ref cpu.F), 12);

            AssertSinglePrefixedCall(0x44, () => bitUnit.TestBit(cpu.H, 0, ref cpu.F), 12);
            AssertSinglePrefixedCall(0x4C, () => bitUnit.TestBit(cpu.H, 1, ref cpu.F), 12);
            AssertSinglePrefixedCall(0x54, () => bitUnit.TestBit(cpu.H, 2, ref cpu.F), 12);
            AssertSinglePrefixedCall(0x5C, () => bitUnit.TestBit(cpu.H, 3, ref cpu.F), 12);
            AssertSinglePrefixedCall(0x64, () => bitUnit.TestBit(cpu.H, 4, ref cpu.F), 12);
            AssertSinglePrefixedCall(0x6C, () => bitUnit.TestBit(cpu.H, 5, ref cpu.F), 12);
            AssertSinglePrefixedCall(0x74, () => bitUnit.TestBit(cpu.H, 6, ref cpu.F), 12);
            AssertSinglePrefixedCall(0x7C, () => bitUnit.TestBit(cpu.H, 7, ref cpu.F), 12);

            AssertSinglePrefixedCall(0x45, () => bitUnit.TestBit(cpu.L, 0, ref cpu.F), 12);
            AssertSinglePrefixedCall(0x4D, () => bitUnit.TestBit(cpu.L, 1, ref cpu.F), 12);
            AssertSinglePrefixedCall(0x55, () => bitUnit.TestBit(cpu.L, 2, ref cpu.F), 12);
            AssertSinglePrefixedCall(0x5D, () => bitUnit.TestBit(cpu.L, 3, ref cpu.F), 12);
            AssertSinglePrefixedCall(0x65, () => bitUnit.TestBit(cpu.L, 4, ref cpu.F), 12);
            AssertSinglePrefixedCall(0x6D, () => bitUnit.TestBit(cpu.L, 5, ref cpu.F), 12);
            AssertSinglePrefixedCall(0x75, () => bitUnit.TestBit(cpu.L, 6, ref cpu.F), 12);
            AssertSinglePrefixedCall(0x7D, () => bitUnit.TestBit(cpu.L, 7, ref cpu.F), 12);

            AssertSinglePrefixedCall(0x46, () => bitUnit.TestBit(MEM_HL_BYTE, 0, ref cpu.F), 16);
            AssertSinglePrefixedCall(0x4E, () => bitUnit.TestBit(MEM_HL_BYTE, 1, ref cpu.F), 16);
            AssertSinglePrefixedCall(0x56, () => bitUnit.TestBit(MEM_HL_BYTE, 2, ref cpu.F), 16);
            AssertSinglePrefixedCall(0x5E, () => bitUnit.TestBit(MEM_HL_BYTE, 3, ref cpu.F), 16);
            AssertSinglePrefixedCall(0x66, () => bitUnit.TestBit(MEM_HL_BYTE, 4, ref cpu.F), 16);
            AssertSinglePrefixedCall(0x6E, () => bitUnit.TestBit(MEM_HL_BYTE, 5, ref cpu.F), 16);
            AssertSinglePrefixedCall(0x76, () => bitUnit.TestBit(MEM_HL_BYTE, 6, ref cpu.F), 16);
            AssertSinglePrefixedCall(0x7E, () => bitUnit.TestBit(MEM_HL_BYTE, 7, ref cpu.F), 16);

            AssertSinglePrefixedCall(0x00, () => bitUnit.RotateLeft(ref cpu.B, ref cpu.F, false), 12);
            AssertSinglePrefixedCall(0x01, () => bitUnit.RotateLeft(ref cpu.C, ref cpu.F, false), 12);
            AssertSinglePrefixedCall(0x02, () => bitUnit.RotateLeft(ref cpu.D, ref cpu.F, false), 12);
            AssertSinglePrefixedCall(0x03, () => bitUnit.RotateLeft(ref cpu.E, ref cpu.F, false), 12);
            AssertSinglePrefixedCall(0x04, () => bitUnit.RotateLeft(ref cpu.H, ref cpu.F, false), 12);
            AssertSinglePrefixedCall(0x05, () => bitUnit.RotateLeft(ref cpu.L, ref cpu.F, false), 12);
            AssertSinglePrefixedCall(0x06, () => bitUnit.RotateLeft(cpu.H, cpu.L, ref cpu.F), 20);
            AssertSinglePrefixedCall(0x07, () => bitUnit.RotateLeft(ref cpu.A, ref cpu.F, false), 12);

            AssertSinglePrefixedCall(0x08, () => bitUnit.RotateRight(ref cpu.B, ref cpu.F, false), 12);
            AssertSinglePrefixedCall(0x09, () => bitUnit.RotateRight(ref cpu.C, ref cpu.F, false), 12);
            AssertSinglePrefixedCall(0x0A, () => bitUnit.RotateRight(ref cpu.D, ref cpu.F, false), 12);
            AssertSinglePrefixedCall(0x0B, () => bitUnit.RotateRight(ref cpu.E, ref cpu.F, false), 12);
            AssertSinglePrefixedCall(0x0C, () => bitUnit.RotateRight(ref cpu.H, ref cpu.F, false), 12);
            AssertSinglePrefixedCall(0x0D, () => bitUnit.RotateRight(ref cpu.L, ref cpu.F, false), 12);
            AssertSinglePrefixedCall(0x0E, () => bitUnit.RotateRight(cpu.H, cpu.L, ref cpu.F), 20);
            AssertSinglePrefixedCall(0x0F, () => bitUnit.RotateRight(ref cpu.A, ref cpu.F, false), 12);

            AssertSinglePrefixedCall(0x10, () => bitUnit.RotateLeftThroughCarry(ref cpu.B, ref cpu.F, false), 12);
            AssertSinglePrefixedCall(0x11, () => bitUnit.RotateLeftThroughCarry(ref cpu.C, ref cpu.F, false), 12);
            AssertSinglePrefixedCall(0x12, () => bitUnit.RotateLeftThroughCarry(ref cpu.D, ref cpu.F, false), 12);
            AssertSinglePrefixedCall(0x13, () => bitUnit.RotateLeftThroughCarry(ref cpu.E, ref cpu.F, false), 12);
            AssertSinglePrefixedCall(0x14, () => bitUnit.RotateLeftThroughCarry(ref cpu.H, ref cpu.F, false), 12);
            AssertSinglePrefixedCall(0x15, () => bitUnit.RotateLeftThroughCarry(ref cpu.L, ref cpu.F, false), 12);
            AssertSinglePrefixedCall(0x16, () => bitUnit.RotateLeftThroughCarry(cpu.H, cpu.L, ref cpu.F), 20);
            AssertSinglePrefixedCall(0x17, () => bitUnit.RotateLeftThroughCarry(ref cpu.A, ref cpu.F, false), 12);

            AssertSinglePrefixedCall(0x18, () => bitUnit.RotateRightThroughCarry(ref cpu.B, ref cpu.F, false), 12);
            AssertSinglePrefixedCall(0x19, () => bitUnit.RotateRightThroughCarry(ref cpu.C, ref cpu.F, false), 12);
            AssertSinglePrefixedCall(0x1A, () => bitUnit.RotateRightThroughCarry(ref cpu.D, ref cpu.F, false), 12);
            AssertSinglePrefixedCall(0x1B, () => bitUnit.RotateRightThroughCarry(ref cpu.E, ref cpu.F, false), 12);
            AssertSinglePrefixedCall(0x1C, () => bitUnit.RotateRightThroughCarry(ref cpu.H, ref cpu.F, false), 12);
            AssertSinglePrefixedCall(0x1D, () => bitUnit.RotateRightThroughCarry(ref cpu.L, ref cpu.F, false), 12);
            AssertSinglePrefixedCall(0x1E, () => bitUnit.RotateRightThroughCarry(cpu.H, cpu.L, ref cpu.F), 20);
            AssertSinglePrefixedCall(0x1F, () => bitUnit.RotateRightThroughCarry(ref cpu.A, ref cpu.F, false), 12);

            AssertSinglePrefixedCall(0x20, () => bitUnit.ShiftLeftArithmetic(ref cpu.B, ref cpu.F), 12);
            AssertSinglePrefixedCall(0x21, () => bitUnit.ShiftLeftArithmetic(ref cpu.C, ref cpu.F), 12);
            AssertSinglePrefixedCall(0x22, () => bitUnit.ShiftLeftArithmetic(ref cpu.D, ref cpu.F), 12);
            AssertSinglePrefixedCall(0x23, () => bitUnit.ShiftLeftArithmetic(ref cpu.E, ref cpu.F), 12);
            AssertSinglePrefixedCall(0x24, () => bitUnit.ShiftLeftArithmetic(ref cpu.H, ref cpu.F), 12);
            AssertSinglePrefixedCall(0x25, () => bitUnit.ShiftLeftArithmetic(ref cpu.L, ref cpu.F), 12);
            AssertSinglePrefixedCall(0x26, () => bitUnit.ShiftLeftArithmetic(cpu.H, cpu.L, ref cpu.F), 20);
            AssertSinglePrefixedCall(0x27, () => bitUnit.ShiftLeftArithmetic(ref cpu.A, ref cpu.F), 12);

            AssertSinglePrefixedCall(0x28, () => bitUnit.ShiftRightArithmetic(ref cpu.B, ref cpu.F), 12);
            AssertSinglePrefixedCall(0x29, () => bitUnit.ShiftRightArithmetic(ref cpu.C, ref cpu.F), 12);
            AssertSinglePrefixedCall(0x2A, () => bitUnit.ShiftRightArithmetic(ref cpu.D, ref cpu.F), 12);
            AssertSinglePrefixedCall(0x2B, () => bitUnit.ShiftRightArithmetic(ref cpu.E, ref cpu.F), 12);
            AssertSinglePrefixedCall(0x2C, () => bitUnit.ShiftRightArithmetic(ref cpu.H, ref cpu.F), 12);
            AssertSinglePrefixedCall(0x2D, () => bitUnit.ShiftRightArithmetic(ref cpu.L, ref cpu.F), 12);
            AssertSinglePrefixedCall(0x2E, () => bitUnit.ShiftRightArithmetic(cpu.H, cpu.L, ref cpu.F), 20);
            AssertSinglePrefixedCall(0x2F, () => bitUnit.ShiftRightArithmetic(ref cpu.A, ref cpu.F), 12);

            AssertSinglePrefixedCall(0x30, () => bitUnit.Swap(ref cpu.B, ref cpu.F), 12);
            AssertSinglePrefixedCall(0x31, () => bitUnit.Swap(ref cpu.C, ref cpu.F), 12);
            AssertSinglePrefixedCall(0x32, () => bitUnit.Swap(ref cpu.D, ref cpu.F), 12);
            AssertSinglePrefixedCall(0x33, () => bitUnit.Swap(ref cpu.E, ref cpu.F), 12);
            AssertSinglePrefixedCall(0x34, () => bitUnit.Swap(ref cpu.H, ref cpu.F), 12);
            AssertSinglePrefixedCall(0x35, () => bitUnit.Swap(ref cpu.L, ref cpu.F), 12);
            AssertSinglePrefixedCall(0x36, () => bitUnit.Swap(cpu.H, cpu.L, ref cpu.F), 20);
            AssertSinglePrefixedCall(0x37, () => bitUnit.Swap(ref cpu.A, ref cpu.F), 12);

            AssertSinglePrefixedCall(0x38, () => bitUnit.ShiftRightLogic(ref cpu.B, ref cpu.F), 12);
            AssertSinglePrefixedCall(0x39, () => bitUnit.ShiftRightLogic(ref cpu.C, ref cpu.F), 12);
            AssertSinglePrefixedCall(0x3A, () => bitUnit.ShiftRightLogic(ref cpu.D, ref cpu.F), 12);
            AssertSinglePrefixedCall(0x3B, () => bitUnit.ShiftRightLogic(ref cpu.E, ref cpu.F), 12);
            AssertSinglePrefixedCall(0x3C, () => bitUnit.ShiftRightLogic(ref cpu.H, ref cpu.F), 12);
            AssertSinglePrefixedCall(0x3D, () => bitUnit.ShiftRightLogic(ref cpu.L, ref cpu.F), 12);
            AssertSinglePrefixedCall(0x3E, () => bitUnit.ShiftRightLogic(cpu.H, cpu.L, ref cpu.F), 20);
            AssertSinglePrefixedCall(0x3F, () => bitUnit.ShiftRightLogic(ref cpu.A, ref cpu.F), 12);

            AssertSinglePrefixedCall(0x80, () => bitUnit.SetBit(ref cpu.B, 0, false), 12);
            AssertSinglePrefixedCall(0x81, () => bitUnit.SetBit(ref cpu.C, 0, false), 12);
            AssertSinglePrefixedCall(0x82, () => bitUnit.SetBit(ref cpu.D, 0, false), 12);
            AssertSinglePrefixedCall(0x83, () => bitUnit.SetBit(ref cpu.E, 0, false), 12);
            AssertSinglePrefixedCall(0x84, () => bitUnit.SetBit(ref cpu.H, 0, false), 12);
            AssertSinglePrefixedCall(0x85, () => bitUnit.SetBit(ref cpu.L, 0, false), 12);
            AssertSinglePrefixedCall(0x86, () => bitUnit.SetBit(cpu.H, cpu.L, 0, false), 20);
            AssertSinglePrefixedCall(0x87, () => bitUnit.SetBit(ref cpu.A, 0, false), 12);
            AssertSinglePrefixedCall(0x88, () => bitUnit.SetBit(ref cpu.B, 1, false), 12);
            AssertSinglePrefixedCall(0x89, () => bitUnit.SetBit(ref cpu.C, 1, false), 12);
            AssertSinglePrefixedCall(0x8A, () => bitUnit.SetBit(ref cpu.D, 1, false), 12);
            AssertSinglePrefixedCall(0x8B, () => bitUnit.SetBit(ref cpu.E, 1, false), 12);
            AssertSinglePrefixedCall(0x8C, () => bitUnit.SetBit(ref cpu.H, 1, false), 12);
            AssertSinglePrefixedCall(0x8D, () => bitUnit.SetBit(ref cpu.L, 1, false), 12);
            AssertSinglePrefixedCall(0x8E, () => bitUnit.SetBit(cpu.H, cpu.L, 1, false), 20);
            AssertSinglePrefixedCall(0x8F, () => bitUnit.SetBit(ref cpu.A, 1, false), 12);

            AssertSinglePrefixedCall(0x90, () => bitUnit.SetBit(ref cpu.B, 2, false), 12);
            AssertSinglePrefixedCall(0x91, () => bitUnit.SetBit(ref cpu.C, 2, false), 12);
            AssertSinglePrefixedCall(0x92, () => bitUnit.SetBit(ref cpu.D, 2, false), 12);
            AssertSinglePrefixedCall(0x93, () => bitUnit.SetBit(ref cpu.E, 2, false), 12);
            AssertSinglePrefixedCall(0x94, () => bitUnit.SetBit(ref cpu.H, 2, false), 12);
            AssertSinglePrefixedCall(0x95, () => bitUnit.SetBit(ref cpu.L, 2, false), 12);
            AssertSinglePrefixedCall(0x96, () => bitUnit.SetBit(cpu.H, cpu.L, 2, false), 20);
            AssertSinglePrefixedCall(0x97, () => bitUnit.SetBit(ref cpu.A, 2, false), 12);
            AssertSinglePrefixedCall(0x98, () => bitUnit.SetBit(ref cpu.B, 3, false), 12);
            AssertSinglePrefixedCall(0x99, () => bitUnit.SetBit(ref cpu.C, 3, false), 12);
            AssertSinglePrefixedCall(0x9A, () => bitUnit.SetBit(ref cpu.D, 3, false), 12);
            AssertSinglePrefixedCall(0x9B, () => bitUnit.SetBit(ref cpu.E, 3, false), 12);
            AssertSinglePrefixedCall(0x9C, () => bitUnit.SetBit(ref cpu.H, 3, false), 12);
            AssertSinglePrefixedCall(0x9D, () => bitUnit.SetBit(ref cpu.L, 3, false), 12);
            AssertSinglePrefixedCall(0x9E, () => bitUnit.SetBit(cpu.H, cpu.L, 3, false), 20);
            AssertSinglePrefixedCall(0x9F, () => bitUnit.SetBit(ref cpu.A, 3, false), 12);

            AssertSinglePrefixedCall(0xA0, () => bitUnit.SetBit(ref cpu.B, 4, false), 12);
            AssertSinglePrefixedCall(0xA1, () => bitUnit.SetBit(ref cpu.C, 4, false), 12);
            AssertSinglePrefixedCall(0xA2, () => bitUnit.SetBit(ref cpu.D, 4, false), 12);
            AssertSinglePrefixedCall(0xA3, () => bitUnit.SetBit(ref cpu.E, 4, false), 12);
            AssertSinglePrefixedCall(0xA4, () => bitUnit.SetBit(ref cpu.H, 4, false), 12);
            AssertSinglePrefixedCall(0xA5, () => bitUnit.SetBit(ref cpu.L, 4, false), 12);
            AssertSinglePrefixedCall(0xA6, () => bitUnit.SetBit(cpu.H, cpu.L, 4, false), 20);
            AssertSinglePrefixedCall(0xA7, () => bitUnit.SetBit(ref cpu.A, 4, false), 12);
            AssertSinglePrefixedCall(0xA8, () => bitUnit.SetBit(ref cpu.B, 5, false), 12);
            AssertSinglePrefixedCall(0xA9, () => bitUnit.SetBit(ref cpu.C, 5, false), 12);
            AssertSinglePrefixedCall(0xAA, () => bitUnit.SetBit(ref cpu.D, 5, false), 12);
            AssertSinglePrefixedCall(0xAB, () => bitUnit.SetBit(ref cpu.E, 5, false), 12);
            AssertSinglePrefixedCall(0xAC, () => bitUnit.SetBit(ref cpu.H, 5, false), 12);
            AssertSinglePrefixedCall(0xAD, () => bitUnit.SetBit(ref cpu.L, 5, false), 12);
            AssertSinglePrefixedCall(0xAE, () => bitUnit.SetBit(cpu.H, cpu.L, 5, false), 20);
            AssertSinglePrefixedCall(0xAF, () => bitUnit.SetBit(ref cpu.A, 5, false), 12);

            AssertSinglePrefixedCall(0xB0, () => bitUnit.SetBit(ref cpu.B, 6, false), 12);
            AssertSinglePrefixedCall(0xB1, () => bitUnit.SetBit(ref cpu.C, 6, false), 12);
            AssertSinglePrefixedCall(0xB2, () => bitUnit.SetBit(ref cpu.D, 6, false), 12);
            AssertSinglePrefixedCall(0xB3, () => bitUnit.SetBit(ref cpu.E, 6, false), 12);
            AssertSinglePrefixedCall(0xB4, () => bitUnit.SetBit(ref cpu.H, 6, false), 12);
            AssertSinglePrefixedCall(0xB5, () => bitUnit.SetBit(ref cpu.L, 6, false), 12);
            AssertSinglePrefixedCall(0xB6, () => bitUnit.SetBit(cpu.H, cpu.L, 6, false), 20);
            AssertSinglePrefixedCall(0xB7, () => bitUnit.SetBit(ref cpu.A, 6, false), 12);
            AssertSinglePrefixedCall(0xB8, () => bitUnit.SetBit(ref cpu.B, 7, false), 12);
            AssertSinglePrefixedCall(0xB9, () => bitUnit.SetBit(ref cpu.C, 7, false), 12);
            AssertSinglePrefixedCall(0xBA, () => bitUnit.SetBit(ref cpu.D, 7, false), 12);
            AssertSinglePrefixedCall(0xBB, () => bitUnit.SetBit(ref cpu.E, 7, false), 12);
            AssertSinglePrefixedCall(0xBC, () => bitUnit.SetBit(ref cpu.H, 7, false), 12);
            AssertSinglePrefixedCall(0xBD, () => bitUnit.SetBit(ref cpu.L, 7, false), 12);
            AssertSinglePrefixedCall(0xBE, () => bitUnit.SetBit(cpu.H, cpu.L, 7, false), 20);
            AssertSinglePrefixedCall(0xBF, () => bitUnit.SetBit(ref cpu.A, 7, false), 12);

            AssertSinglePrefixedCall(0xC0, () => bitUnit.SetBit(ref cpu.B, 0, true), 12);
            AssertSinglePrefixedCall(0xC1, () => bitUnit.SetBit(ref cpu.C, 0, true), 12);
            AssertSinglePrefixedCall(0xC2, () => bitUnit.SetBit(ref cpu.D, 0, true), 12);
            AssertSinglePrefixedCall(0xC3, () => bitUnit.SetBit(ref cpu.E, 0, true), 12);
            AssertSinglePrefixedCall(0xC4, () => bitUnit.SetBit(ref cpu.H, 0, true), 12);
            AssertSinglePrefixedCall(0xC5, () => bitUnit.SetBit(ref cpu.L, 0, true), 12);
            AssertSinglePrefixedCall(0xC6, () => bitUnit.SetBit(cpu.H, cpu.L, 0, true), 20);
            AssertSinglePrefixedCall(0xC7, () => bitUnit.SetBit(ref cpu.A, 0, true), 12);
            AssertSinglePrefixedCall(0xC8, () => bitUnit.SetBit(ref cpu.B, 1, true), 12);
            AssertSinglePrefixedCall(0xC9, () => bitUnit.SetBit(ref cpu.C, 1, true), 12);
            AssertSinglePrefixedCall(0xCA, () => bitUnit.SetBit(ref cpu.D, 1, true), 12);
            AssertSinglePrefixedCall(0xCB, () => bitUnit.SetBit(ref cpu.E, 1, true), 12);
            AssertSinglePrefixedCall(0xCC, () => bitUnit.SetBit(ref cpu.H, 1, true), 12);
            AssertSinglePrefixedCall(0xCD, () => bitUnit.SetBit(ref cpu.L, 1, true), 12);
            AssertSinglePrefixedCall(0xCE, () => bitUnit.SetBit(cpu.H, cpu.L, 1, true), 20);
            AssertSinglePrefixedCall(0xCF, () => bitUnit.SetBit(ref cpu.A, 1, true), 12);

            AssertSinglePrefixedCall(0xD0, () => bitUnit.SetBit(ref cpu.B, 2, true), 12);
            AssertSinglePrefixedCall(0xD1, () => bitUnit.SetBit(ref cpu.C, 2, true), 12);
            AssertSinglePrefixedCall(0xD2, () => bitUnit.SetBit(ref cpu.D, 2, true), 12);
            AssertSinglePrefixedCall(0xD3, () => bitUnit.SetBit(ref cpu.E, 2, true), 12);
            AssertSinglePrefixedCall(0xD4, () => bitUnit.SetBit(ref cpu.H, 2, true), 12);
            AssertSinglePrefixedCall(0xD5, () => bitUnit.SetBit(ref cpu.L, 2, true), 12);
            AssertSinglePrefixedCall(0xD6, () => bitUnit.SetBit(cpu.H, cpu.L, 2, true), 20);
            AssertSinglePrefixedCall(0xD7, () => bitUnit.SetBit(ref cpu.A, 2, true), 12);
            AssertSinglePrefixedCall(0xD8, () => bitUnit.SetBit(ref cpu.B, 3, true), 12);
            AssertSinglePrefixedCall(0xD9, () => bitUnit.SetBit(ref cpu.C, 3, true), 12);
            AssertSinglePrefixedCall(0xDA, () => bitUnit.SetBit(ref cpu.D, 3, true), 12);
            AssertSinglePrefixedCall(0xDB, () => bitUnit.SetBit(ref cpu.E, 3, true), 12);
            AssertSinglePrefixedCall(0xDC, () => bitUnit.SetBit(ref cpu.H, 3, true), 12);
            AssertSinglePrefixedCall(0xDD, () => bitUnit.SetBit(ref cpu.L, 3, true), 12);
            AssertSinglePrefixedCall(0xDE, () => bitUnit.SetBit(cpu.H, cpu.L, 3, true), 20);
            AssertSinglePrefixedCall(0xDF, () => bitUnit.SetBit(ref cpu.A, 3, true), 12);

            AssertSinglePrefixedCall(0xE0, () => bitUnit.SetBit(ref cpu.B, 4, true), 12);
            AssertSinglePrefixedCall(0xE1, () => bitUnit.SetBit(ref cpu.C, 4, true), 12);
            AssertSinglePrefixedCall(0xE2, () => bitUnit.SetBit(ref cpu.D, 4, true), 12);
            AssertSinglePrefixedCall(0xE3, () => bitUnit.SetBit(ref cpu.E, 4, true), 12);
            AssertSinglePrefixedCall(0xE4, () => bitUnit.SetBit(ref cpu.H, 4, true), 12);
            AssertSinglePrefixedCall(0xE5, () => bitUnit.SetBit(ref cpu.L, 4, true), 12);
            AssertSinglePrefixedCall(0xE6, () => bitUnit.SetBit(cpu.H, cpu.L, 4, true), 20);
            AssertSinglePrefixedCall(0xE7, () => bitUnit.SetBit(ref cpu.A, 4, true), 12);
            AssertSinglePrefixedCall(0xE8, () => bitUnit.SetBit(ref cpu.B, 5, true), 12);
            AssertSinglePrefixedCall(0xE9, () => bitUnit.SetBit(ref cpu.C, 5, true), 12);
            AssertSinglePrefixedCall(0xEA, () => bitUnit.SetBit(ref cpu.D, 5, true), 12);
            AssertSinglePrefixedCall(0xEB, () => bitUnit.SetBit(ref cpu.E, 5, true), 12);
            AssertSinglePrefixedCall(0xEC, () => bitUnit.SetBit(ref cpu.H, 5, true), 12);
            AssertSinglePrefixedCall(0xED, () => bitUnit.SetBit(ref cpu.L, 5, true), 12);
            AssertSinglePrefixedCall(0xEE, () => bitUnit.SetBit(cpu.H, cpu.L, 5, true), 20);
            AssertSinglePrefixedCall(0xEF, () => bitUnit.SetBit(ref cpu.A, 5, true), 12);

            AssertSinglePrefixedCall(0xF0, () => bitUnit.SetBit(ref cpu.B, 6, true), 12);
            AssertSinglePrefixedCall(0xF1, () => bitUnit.SetBit(ref cpu.C, 6, true), 12);
            AssertSinglePrefixedCall(0xF2, () => bitUnit.SetBit(ref cpu.D, 6, true), 12);
            AssertSinglePrefixedCall(0xF3, () => bitUnit.SetBit(ref cpu.E, 6, true), 12);
            AssertSinglePrefixedCall(0xF4, () => bitUnit.SetBit(ref cpu.H, 6, true), 12);
            AssertSinglePrefixedCall(0xF5, () => bitUnit.SetBit(ref cpu.L, 6, true), 12);
            AssertSinglePrefixedCall(0xF6, () => bitUnit.SetBit(cpu.H, cpu.L, 6, true), 20);
            AssertSinglePrefixedCall(0xF7, () => bitUnit.SetBit(ref cpu.A, 6, true), 12);
            AssertSinglePrefixedCall(0xF8, () => bitUnit.SetBit(ref cpu.B, 7, true), 12);
            AssertSinglePrefixedCall(0xF9, () => bitUnit.SetBit(ref cpu.C, 7, true), 12);
            AssertSinglePrefixedCall(0xFA, () => bitUnit.SetBit(ref cpu.D, 7, true), 12);
            AssertSinglePrefixedCall(0xFB, () => bitUnit.SetBit(ref cpu.E, 7, true), 12);
            AssertSinglePrefixedCall(0xFC, () => bitUnit.SetBit(ref cpu.H, 7, true), 12);
            AssertSinglePrefixedCall(0xFD, () => bitUnit.SetBit(ref cpu.L, 7, true), 12);
            AssertSinglePrefixedCall(0xFE, () => bitUnit.SetBit(cpu.H, cpu.L, 7, true), 20);
            AssertSinglePrefixedCall(0xFF, () => bitUnit.SetBit(ref cpu.A, 7, true), 12);
        }

        [Fact()]
        public void ReadImmediateByteTest()
        {
            var cycles = cpu.ReadImmediateByte(out var immediate);

            Assert.Equal(IMMEDIATE_BYTE, immediate);
            Assert.Equal(1, cpu.PC);
            Assert.Equal(4, cycles);
        }

        [Fact()]
        public void ReadImmediateWordTest()
        {
            var cycles = cpu.ReadImmediateWord(out var immediate);

            Assert.Equal(IMMEDIATE_WORD, immediate);
            Assert.Equal(2, cpu.PC);
            Assert.Equal(8, cycles);
        }

        [Fact()]
        public void ReadFromMemoryTest()
        {
            memory.WriteByte(0xABCD, 0x99);

            int cycles = cpu.ReadFromMemory(0xAB, 0xCD, out var value);

            Assert.Equal(0x99, value);
            Assert.Equal(4, cycles);
        }

        private void AssertSingleCall(byte opcode, Expression<Action> expectedCall, int expectedCycles)
        {
            OpCode opCode = cpu.OpCodes[opcode];
            opCode.Instruction();
            A.CallTo(expectedCall).MustHaveHappenedOnceExactly();
            Fake.ClearRecordedCalls(loadUnit);

            // Reset PC between calls because some instructions rely on immediate values at the beginning of the rom.
            cpu.PC = 0;
        }

        private void AssertSinglePrefixedCall(byte opcode, Expression<Action> expectedCall, int expectedCycles)
        {
            OpCode opCode = cpu.OpCodesPrefixed[opcode];
            opCode.Instruction();
            A.CallTo(expectedCall).MustHaveHappenedOnceExactly();
            Fake.ClearRecordedCalls(loadUnit);

            // Reset PC between calls because some instructions rely on immediate values at the beginning of the rom.
            cpu.PC = 0;
        }
    }
}
