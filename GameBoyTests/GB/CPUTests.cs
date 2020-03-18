using FakeItEasy;
using GameBoy.GB.CpuUnits;
using System;
using System.Linq.Expressions;
using Xunit;

namespace GameBoy.GB.Tests
{
    public class CPUTests
    {
        private const byte IMMEDIATE_BYTE = 0x01;
        private const ushort IMMEDIATE_WORD = 0x0123;

        private readonly Memory memory;
        private readonly CPU cpu;
        private readonly ILoadUnit loadUnit;
        private readonly IALU alu;

        public CPUTests()
        {
            memory = new Memory(new byte[]
            {
                IMMEDIATE_BYTE,
                IMMEDIATE_WORD & 0x00FF
            });
            loadUnit = A.Fake<ILoadUnit>();
            alu = A.Fake<IALU>();
            cpu = new CPU(memory, loadUnit, alu)
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
        }

        [Fact()]
        public void LoadByteInstructionMappingTest()
        {
            AssertSingleCall(0x06, () => loadUnit.Load(ref cpu.B, IMMEDIATE_BYTE));
            AssertSingleCall(0x0E, () => loadUnit.Load(ref cpu.C, IMMEDIATE_BYTE));
            AssertSingleCall(0x16, () => loadUnit.Load(ref cpu.D, IMMEDIATE_BYTE));
            AssertSingleCall(0x1E, () => loadUnit.Load(ref cpu.E, IMMEDIATE_BYTE));
            AssertSingleCall(0x26, () => loadUnit.Load(ref cpu.H, IMMEDIATE_BYTE));
            AssertSingleCall(0x2E, () => loadUnit.Load(ref cpu.L, IMMEDIATE_BYTE));

            AssertSingleCall(0x7F, () => loadUnit.Load(ref cpu.A, cpu.A));
            AssertSingleCall(0x78, () => loadUnit.Load(ref cpu.A, cpu.B));
            AssertSingleCall(0x79, () => loadUnit.Load(ref cpu.A, cpu.C));
            AssertSingleCall(0x7A, () => loadUnit.Load(ref cpu.A, cpu.D));
            AssertSingleCall(0x7B, () => loadUnit.Load(ref cpu.A, cpu.E));
            AssertSingleCall(0x7C, () => loadUnit.Load(ref cpu.A, cpu.H));
            AssertSingleCall(0x7D, () => loadUnit.Load(ref cpu.A, cpu.L));
            AssertSingleCall(0x7E, () => loadUnit.LoadFromAddress(ref cpu.A, cpu.H, cpu.L));
            AssertSingleCall(0x40, () => loadUnit.Load(ref cpu.B, cpu.B));
            AssertSingleCall(0x41, () => loadUnit.Load(ref cpu.B, cpu.C));
            AssertSingleCall(0x42, () => loadUnit.Load(ref cpu.B, cpu.D));
            AssertSingleCall(0x43, () => loadUnit.Load(ref cpu.B, cpu.E));
            AssertSingleCall(0x44, () => loadUnit.Load(ref cpu.B, cpu.H));
            AssertSingleCall(0x45, () => loadUnit.Load(ref cpu.B, cpu.L));
            AssertSingleCall(0x46, () => loadUnit.LoadFromAddress(ref cpu.B, cpu.H, cpu.L));
            AssertSingleCall(0x48, () => loadUnit.Load(ref cpu.C, cpu.B));
            AssertSingleCall(0x49, () => loadUnit.Load(ref cpu.C, cpu.C));
            AssertSingleCall(0x4A, () => loadUnit.Load(ref cpu.C, cpu.D));
            AssertSingleCall(0x4B, () => loadUnit.Load(ref cpu.C, cpu.E));
            AssertSingleCall(0x4C, () => loadUnit.Load(ref cpu.C, cpu.H));
            AssertSingleCall(0x4D, () => loadUnit.Load(ref cpu.C, cpu.L));
            AssertSingleCall(0x4E, () => loadUnit.LoadFromAddress(ref cpu.C, cpu.H, cpu.L));
            AssertSingleCall(0x50, () => loadUnit.Load(ref cpu.D, cpu.B));
            AssertSingleCall(0x51, () => loadUnit.Load(ref cpu.D, cpu.C));
            AssertSingleCall(0x52, () => loadUnit.Load(ref cpu.D, cpu.D));
            AssertSingleCall(0x53, () => loadUnit.Load(ref cpu.D, cpu.E));
            AssertSingleCall(0x54, () => loadUnit.Load(ref cpu.D, cpu.H));
            AssertSingleCall(0x55, () => loadUnit.Load(ref cpu.D, cpu.L));
            AssertSingleCall(0x56, () => loadUnit.LoadFromAddress(ref cpu.D, cpu.H, cpu.L));
            AssertSingleCall(0x58, () => loadUnit.Load(ref cpu.E, cpu.B));
            AssertSingleCall(0x59, () => loadUnit.Load(ref cpu.E, cpu.C));
            AssertSingleCall(0x5A, () => loadUnit.Load(ref cpu.E, cpu.D));
            AssertSingleCall(0x5B, () => loadUnit.Load(ref cpu.E, cpu.E));
            AssertSingleCall(0x5C, () => loadUnit.Load(ref cpu.E, cpu.H));
            AssertSingleCall(0x5D, () => loadUnit.Load(ref cpu.E, cpu.L));
            AssertSingleCall(0x5E, () => loadUnit.LoadFromAddress(ref cpu.E, cpu.H, cpu.L));
            AssertSingleCall(0x60, () => loadUnit.Load(ref cpu.H, cpu.B));
            AssertSingleCall(0x61, () => loadUnit.Load(ref cpu.H, cpu.C));
            AssertSingleCall(0x62, () => loadUnit.Load(ref cpu.H, cpu.D));
            AssertSingleCall(0x63, () => loadUnit.Load(ref cpu.H, cpu.E));
            AssertSingleCall(0x64, () => loadUnit.Load(ref cpu.H, cpu.H));
            AssertSingleCall(0x65, () => loadUnit.Load(ref cpu.H, cpu.L));
            AssertSingleCall(0x66, () => loadUnit.LoadFromAddress(ref cpu.H, cpu.H, cpu.L));
            AssertSingleCall(0x68, () => loadUnit.Load(ref cpu.L, cpu.B));
            AssertSingleCall(0x69, () => loadUnit.Load(ref cpu.L, cpu.C));
            AssertSingleCall(0x6A, () => loadUnit.Load(ref cpu.L, cpu.D));
            AssertSingleCall(0x6B, () => loadUnit.Load(ref cpu.L, cpu.E));
            AssertSingleCall(0x6C, () => loadUnit.Load(ref cpu.L, cpu.H));
            AssertSingleCall(0x6D, () => loadUnit.Load(ref cpu.L, cpu.L));
            AssertSingleCall(0x6E, () => loadUnit.LoadFromAddress(ref cpu.L, cpu.H, cpu.L));
            AssertSingleCall(0x70, () => loadUnit.WriteToAddress(cpu.H, cpu.L, cpu.B));
            AssertSingleCall(0x71, () => loadUnit.WriteToAddress(cpu.H, cpu.L, cpu.C));
            AssertSingleCall(0x72, () => loadUnit.WriteToAddress(cpu.H, cpu.L, cpu.D));
            AssertSingleCall(0x73, () => loadUnit.WriteToAddress(cpu.H, cpu.L, cpu.E));
            AssertSingleCall(0x74, () => loadUnit.WriteToAddress(cpu.H, cpu.L, cpu.H));
            AssertSingleCall(0x75, () => loadUnit.WriteToAddress(cpu.H, cpu.L, cpu.L));
            AssertSingleCall(0x36, () => loadUnit.WriteToAddress(cpu.H, cpu.L, IMMEDIATE_BYTE));

            AssertSingleCall(0x0A, () => loadUnit.LoadFromAddress(ref cpu.A, cpu.B, cpu.C));
            AssertSingleCall(0x1A, () => loadUnit.LoadFromAddress(ref cpu.A, cpu.D, cpu.E));
            AssertSingleCall(0xFA, () => loadUnit.LoadFromAddress(ref cpu.A, IMMEDIATE_WORD));
            AssertSingleCall(0x3E, () => loadUnit.Load(ref cpu.A, IMMEDIATE_BYTE));

            AssertSingleCall(0x47, () => loadUnit.Load(ref cpu.B, cpu.A));
            AssertSingleCall(0x4F, () => loadUnit.Load(ref cpu.C, cpu.A));
            AssertSingleCall(0x57, () => loadUnit.Load(ref cpu.D, cpu.A));
            AssertSingleCall(0x5F, () => loadUnit.Load(ref cpu.E, cpu.A));
            AssertSingleCall(0x67, () => loadUnit.Load(ref cpu.H, cpu.A));
            AssertSingleCall(0x6F, () => loadUnit.Load(ref cpu.L, cpu.A));
            AssertSingleCall(0x02, () => loadUnit.WriteToAddress(cpu.B, cpu.C, cpu.A));
            AssertSingleCall(0x12, () => loadUnit.WriteToAddress(cpu.D, cpu.E, cpu.A));
            AssertSingleCall(0x77, () => loadUnit.WriteToAddress(cpu.H, cpu.L, cpu.A));
            AssertSingleCall(0xEA, () => loadUnit.WriteToAddress(IMMEDIATE_WORD, cpu.A));

            AssertSingleCall(0xF2, () => loadUnit.LoadFromAddress(ref cpu.A, (ushort)(0xFF00 + cpu.C)));
            AssertSingleCall(0xE2, () => loadUnit.WriteToAddress((ushort)(0xFF00 + cpu.C), cpu.A));

            AssertSingleCall(0x3A, () => loadUnit.LoadFromAddressAndIncrement(ref cpu.A, ref cpu.H, ref cpu.L, -1));
            AssertSingleCall(0x32, () => loadUnit.WriteToAddressAndIncrement(ref cpu.H, ref cpu.L, cpu.A, -1));
            AssertSingleCall(0x2A, () => loadUnit.LoadFromAddressAndIncrement(ref cpu.A, ref cpu.H, ref cpu.L, 1));
            AssertSingleCall(0x22, () => loadUnit.WriteToAddressAndIncrement(ref cpu.H, ref cpu.L, cpu.A, 1));
            AssertSingleCall(0xE0, () => loadUnit.WriteToAddress(0xFF00 + IMMEDIATE_BYTE, cpu.A));
            AssertSingleCall(0xF0, () => loadUnit.LoadFromAddress(ref cpu.A, 0xFF00 + IMMEDIATE_BYTE));

            AssertSingleCall(0x01, () => loadUnit.Load(ref cpu.B, ref cpu.C, IMMEDIATE_WORD));
            AssertSingleCall(0x11, () => loadUnit.Load(ref cpu.D, ref cpu.E, IMMEDIATE_WORD));
            AssertSingleCall(0x21, () => loadUnit.Load(ref cpu.H, ref cpu.L, IMMEDIATE_WORD));
            AssertSingleCall(0x31, () => loadUnit.Load(ref cpu.SP, IMMEDIATE_WORD));

            AssertSingleCall(0xF9, () => loadUnit.Load(ref cpu.SP, cpu.H, cpu.L));
            AssertSingleCall(0xF8, () => loadUnit.LoadAdjusted(ref cpu.H, ref cpu.L, cpu.SP, IMMEDIATE_BYTE));
            AssertSingleCall(0x08, () => loadUnit.WriteToAddress(IMMEDIATE_WORD, cpu.SP));

            AssertSingleCall(0xF5, () => loadUnit.Push(ref cpu.SP, cpu.A, cpu.F));
            AssertSingleCall(0xC5, () => loadUnit.Push(ref cpu.SP, cpu.B, cpu.C));
            AssertSingleCall(0xD5, () => loadUnit.Push(ref cpu.SP, cpu.D, cpu.E));
            AssertSingleCall(0xE5, () => loadUnit.Push(ref cpu.SP, cpu.H, cpu.L));
            AssertSingleCall(0xF1, () => loadUnit.Pop(ref cpu.A, ref cpu.F, ref cpu.SP));
            AssertSingleCall(0xC1, () => loadUnit.Pop(ref cpu.B, ref cpu.C, ref cpu.SP));
            AssertSingleCall(0xD1, () => loadUnit.Pop(ref cpu.D, ref cpu.E, ref cpu.SP));
            AssertSingleCall(0xE1, () => loadUnit.Pop(ref cpu.H, ref cpu.L, ref cpu.SP));
        }

        [Fact()]
        public void ALUInstructionMappingTest()
        {
            AssertSingleCall(0x87, () => alu.Add(ref cpu.A, cpu.A, ref cpu.F, false));
            AssertSingleCall(0x80, () => alu.Add(ref cpu.A, cpu.B, ref cpu.F, false));
            AssertSingleCall(0x81, () => alu.Add(ref cpu.A, cpu.C, ref cpu.F, false));
            AssertSingleCall(0x82, () => alu.Add(ref cpu.A, cpu.D, ref cpu.F, false));
            AssertSingleCall(0x83, () => alu.Add(ref cpu.A, cpu.E, ref cpu.F, false));
            AssertSingleCall(0x84, () => alu.Add(ref cpu.A, cpu.H, ref cpu.F, false));
            AssertSingleCall(0x85, () => alu.Add(ref cpu.A, cpu.L, ref cpu.F, false));
            AssertSingleCall(0x86, () => alu.AddFromMemory(ref cpu.A, cpu.H, cpu.L, ref cpu.F, false));
            AssertSingleCall(0xC6, () => alu.Add(ref cpu.A, IMMEDIATE_BYTE, ref cpu.F, false));

            AssertSingleCall(0x8F, () => alu.Add(ref cpu.A, cpu.A, ref cpu.F, true));
            AssertSingleCall(0x88, () => alu.Add(ref cpu.A, cpu.B, ref cpu.F, true));
            AssertSingleCall(0x89, () => alu.Add(ref cpu.A, cpu.C, ref cpu.F, true));
            AssertSingleCall(0x8A, () => alu.Add(ref cpu.A, cpu.D, ref cpu.F, true));
            AssertSingleCall(0x8B, () => alu.Add(ref cpu.A, cpu.E, ref cpu.F, true));
            AssertSingleCall(0x8C, () => alu.Add(ref cpu.A, cpu.H, ref cpu.F, true));
            AssertSingleCall(0x8D, () => alu.Add(ref cpu.A, cpu.L, ref cpu.F, true));
            AssertSingleCall(0x8E, () => alu.AddFromMemory(ref cpu.A, cpu.H, cpu.L, ref cpu.F, true));
            AssertSingleCall(0xCE, () => alu.Add(ref cpu.A, IMMEDIATE_BYTE, ref cpu.F, true));

            AssertSingleCall(0x97, () => alu.Subtract(ref cpu.A, cpu.A, ref cpu.F, false));
            AssertSingleCall(0x90, () => alu.Subtract(ref cpu.A, cpu.B, ref cpu.F, false));
            AssertSingleCall(0x91, () => alu.Subtract(ref cpu.A, cpu.C, ref cpu.F, false));
            AssertSingleCall(0x92, () => alu.Subtract(ref cpu.A, cpu.D, ref cpu.F, false));
            AssertSingleCall(0x93, () => alu.Subtract(ref cpu.A, cpu.E, ref cpu.F, false));
            AssertSingleCall(0x94, () => alu.Subtract(ref cpu.A, cpu.H, ref cpu.F, false));
            AssertSingleCall(0x95, () => alu.Subtract(ref cpu.A, cpu.L, ref cpu.F, false));
            AssertSingleCall(0x96, () => alu.SubtractFromMemory(ref cpu.A, cpu.H, cpu.L, ref cpu.F, false));
            AssertSingleCall(0xD6, () => alu.Subtract(ref cpu.A, IMMEDIATE_BYTE, ref cpu.F, false));

            AssertSingleCall(0x9F, () => alu.Subtract(ref cpu.A, cpu.A, ref cpu.F, true));
            AssertSingleCall(0x98, () => alu.Subtract(ref cpu.A, cpu.B, ref cpu.F, true));
            AssertSingleCall(0x99, () => alu.Subtract(ref cpu.A, cpu.C, ref cpu.F, true));
            AssertSingleCall(0x9A, () => alu.Subtract(ref cpu.A, cpu.D, ref cpu.F, true));
            AssertSingleCall(0x9B, () => alu.Subtract(ref cpu.A, cpu.E, ref cpu.F, true));
            AssertSingleCall(0x9C, () => alu.Subtract(ref cpu.A, cpu.H, ref cpu.F, true));
            AssertSingleCall(0x9D, () => alu.Subtract(ref cpu.A, cpu.L, ref cpu.F, true));
            AssertSingleCall(0x9E, () => alu.SubtractFromMemory(ref cpu.A, cpu.H, cpu.L, ref cpu.F, true));
            AssertSingleCall(0xDE, () => alu.Subtract(ref cpu.A, IMMEDIATE_BYTE, ref cpu.F, true));
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

        private void AssertSingleCall(byte opcode, Expression<Func<int>> expectedCall)
        {
            cpu.OpCodes[opcode]();
            A.CallTo(expectedCall).MustHaveHappenedOnceExactly();
            Fake.ClearRecordedCalls(loadUnit);

            // Reset PC between calls because some instructions rely on immediate values at the beginning of the rom.
            cpu.PC = 0;
        }
    }
}