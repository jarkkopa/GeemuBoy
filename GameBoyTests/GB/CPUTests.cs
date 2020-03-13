﻿using FakeItEasy;
using GameBoy.GB.CpuUnits;
using System;
using System.Linq.Expressions;
using Xunit;

namespace GameBoy.GB.Tests
{
    public class CPUTests
    {
        private readonly Memory memory;
        private readonly ILoadUnit loadUnit;
        private readonly CPU cpu;

        public CPUTests()
        {
            memory = new Memory(new byte[] { });
            loadUnit = A.Fake<ILoadUnit>();
            cpu = new CPU(memory, loadUnit)
            {
                A = 0x0A,
                B = 0x0B,
                C = 0x0C,
                D = 0x0D,
                E = 0x0E,
                H = 0xAA,
                L = 0xBB,
                PC = 0x00
            };
        }

        [Fact()]
        public void LoadByteInstructionMappingTest()
        {
            AssertSingleCall(0x06, () => loadUnit.LoadImmediateByte(ref cpu.B, ref cpu.PC));
            AssertSingleCall(0x0E, () => loadUnit.LoadImmediateByte(ref cpu.C, ref cpu.PC));
            AssertSingleCall(0x16, () => loadUnit.LoadImmediateByte(ref cpu.D, ref cpu.PC));
            AssertSingleCall(0x1E, () => loadUnit.LoadImmediateByte(ref cpu.E, ref cpu.PC));
            AssertSingleCall(0x26, () => loadUnit.LoadImmediateByte(ref cpu.H, ref cpu.PC));
            AssertSingleCall(0x2E, () => loadUnit.LoadImmediateByte(ref cpu.L, ref cpu.PC));

            AssertSingleCall(0x7F, () => loadUnit.Copy(ref cpu.A, cpu.A));
            AssertSingleCall(0x78, () => loadUnit.Copy(ref cpu.A, cpu.B));
            AssertSingleCall(0x79, () => loadUnit.Copy(ref cpu.A, cpu.C));
            AssertSingleCall(0x7A, () => loadUnit.Copy(ref cpu.A, cpu.D));
            AssertSingleCall(0x7B, () => loadUnit.Copy(ref cpu.A, cpu.E));
            AssertSingleCall(0x7C, () => loadUnit.Copy(ref cpu.A, cpu.H));
            AssertSingleCall(0x7D, () => loadUnit.Copy(ref cpu.A, cpu.L));
            AssertSingleCall(0x7E, () => loadUnit.LoadFromAddress(ref cpu.A, cpu.H, cpu.L));
            AssertSingleCall(0x40, () => loadUnit.Copy(ref cpu.B, cpu.B));
            AssertSingleCall(0x41, () => loadUnit.Copy(ref cpu.B, cpu.C));
            AssertSingleCall(0x42, () => loadUnit.Copy(ref cpu.B, cpu.D));
            AssertSingleCall(0x43, () => loadUnit.Copy(ref cpu.B, cpu.E));
            AssertSingleCall(0x44, () => loadUnit.Copy(ref cpu.B, cpu.H));
            AssertSingleCall(0x45, () => loadUnit.Copy(ref cpu.B, cpu.L));
            AssertSingleCall(0x46, () => loadUnit.LoadFromAddress(ref cpu.B, cpu.H, cpu.L));
            AssertSingleCall(0x48, () => loadUnit.Copy(ref cpu.C, cpu.B));
            AssertSingleCall(0x49, () => loadUnit.Copy(ref cpu.C, cpu.C));
            AssertSingleCall(0x4A, () => loadUnit.Copy(ref cpu.C, cpu.D));
            AssertSingleCall(0x4B, () => loadUnit.Copy(ref cpu.C, cpu.E));
            AssertSingleCall(0x4C, () => loadUnit.Copy(ref cpu.C, cpu.H));
            AssertSingleCall(0x4D, () => loadUnit.Copy(ref cpu.C, cpu.L));
            AssertSingleCall(0x4E, () => loadUnit.LoadFromAddress(ref cpu.C, cpu.H, cpu.L));
            AssertSingleCall(0x50, () => loadUnit.Copy(ref cpu.D, cpu.B));
            AssertSingleCall(0x51, () => loadUnit.Copy(ref cpu.D, cpu.C));
            AssertSingleCall(0x52, () => loadUnit.Copy(ref cpu.D, cpu.D));
            AssertSingleCall(0x53, () => loadUnit.Copy(ref cpu.D, cpu.E));
            AssertSingleCall(0x54, () => loadUnit.Copy(ref cpu.D, cpu.H));
            AssertSingleCall(0x55, () => loadUnit.Copy(ref cpu.D, cpu.L));
            AssertSingleCall(0x56, () => loadUnit.LoadFromAddress(ref cpu.D, cpu.H, cpu.L));
            AssertSingleCall(0x58, () => loadUnit.Copy(ref cpu.E, cpu.B));
            AssertSingleCall(0x59, () => loadUnit.Copy(ref cpu.E, cpu.C));
            AssertSingleCall(0x5A, () => loadUnit.Copy(ref cpu.E, cpu.D));
            AssertSingleCall(0x5B, () => loadUnit.Copy(ref cpu.E, cpu.E));
            AssertSingleCall(0x5C, () => loadUnit.Copy(ref cpu.E, cpu.H));
            AssertSingleCall(0x5D, () => loadUnit.Copy(ref cpu.E, cpu.L));
            AssertSingleCall(0x5E, () => loadUnit.LoadFromAddress(ref cpu.E, cpu.H, cpu.L));
            AssertSingleCall(0x60, () => loadUnit.Copy(ref cpu.H, cpu.B));
            AssertSingleCall(0x61, () => loadUnit.Copy(ref cpu.H, cpu.C));
            AssertSingleCall(0x62, () => loadUnit.Copy(ref cpu.H, cpu.D));
            AssertSingleCall(0x63, () => loadUnit.Copy(ref cpu.H, cpu.E));
            AssertSingleCall(0x64, () => loadUnit.Copy(ref cpu.H, cpu.H));
            AssertSingleCall(0x65, () => loadUnit.Copy(ref cpu.H, cpu.L));
            AssertSingleCall(0x66, () => loadUnit.LoadFromAddress(ref cpu.H, cpu.H, cpu.L));
            AssertSingleCall(0x68, () => loadUnit.Copy(ref cpu.L, cpu.B));
            AssertSingleCall(0x69, () => loadUnit.Copy(ref cpu.L, cpu.C));
            AssertSingleCall(0x6A, () => loadUnit.Copy(ref cpu.L, cpu.D));
            AssertSingleCall(0x6B, () => loadUnit.Copy(ref cpu.L, cpu.E));
            AssertSingleCall(0x6C, () => loadUnit.Copy(ref cpu.L, cpu.H));
            AssertSingleCall(0x6D, () => loadUnit.Copy(ref cpu.L, cpu.L));
            AssertSingleCall(0x6E, () => loadUnit.LoadFromAddress(ref cpu.L, cpu.H, cpu.L));
            AssertSingleCall(0x70, () => loadUnit.WriteToAddress(cpu.H, cpu.L, cpu.B));
            AssertSingleCall(0x71, () => loadUnit.WriteToAddress(cpu.H, cpu.L, cpu.C));
            AssertSingleCall(0x72, () => loadUnit.WriteToAddress(cpu.H, cpu.L, cpu.D));
            AssertSingleCall(0x73, () => loadUnit.WriteToAddress(cpu.H, cpu.L, cpu.E));
            AssertSingleCall(0x74, () => loadUnit.WriteToAddress(cpu.H, cpu.L, cpu.H));
            AssertSingleCall(0x75, () => loadUnit.WriteToAddress(cpu.H, cpu.L, cpu.L));
            AssertSingleCall(0x36, () => loadUnit.LoadImmediateByteToAddress(cpu.H, cpu.L, ref cpu.PC));

            AssertSingleCall(0x0A, () => loadUnit.LoadFromAddress(ref cpu.A, cpu.B, cpu.C));
            AssertSingleCall(0x1A, () => loadUnit.LoadFromAddress(ref cpu.A, cpu.D, cpu.E));
            AssertSingleCall(0xFA, () => loadUnit.LoadFromImmediateAddress(ref cpu.A, ref cpu.PC));
            AssertSingleCall(0x3E, () => loadUnit.LoadImmediateByte(ref cpu.A, ref cpu.PC));

            AssertSingleCall(0x47, () => loadUnit.Copy(ref cpu.B, cpu.A));
            AssertSingleCall(0x4F, () => loadUnit.Copy(ref cpu.C, cpu.A));
            AssertSingleCall(0x57, () => loadUnit.Copy(ref cpu.D, cpu.A));
            AssertSingleCall(0x5F, () => loadUnit.Copy(ref cpu.E, cpu.A));
            AssertSingleCall(0x67, () => loadUnit.Copy(ref cpu.H, cpu.A));
            AssertSingleCall(0x6F, () => loadUnit.Copy(ref cpu.L, cpu.A));
            AssertSingleCall(0x02, () => loadUnit.WriteToAddress(cpu.B, cpu.C, cpu.A));
            AssertSingleCall(0x12, () => loadUnit.WriteToAddress(cpu.D, cpu.E, cpu.A));
            AssertSingleCall(0x77, () => loadUnit.WriteToAddress(cpu.H, cpu.L, cpu.A));
            AssertSingleCall(0xEA, () => loadUnit.WriteToImmediateAddress(cpu.A, ref cpu.PC));

            AssertSingleCall(0xF2, () => loadUnit.LoadFromAddress(ref cpu.A, (ushort)(0xFF00 + cpu.C)));
            AssertSingleCall(0xE2, () => loadUnit.WriteToAddress((ushort)(0xFF00 + cpu.C), cpu.A));

            AssertSingleCall(0x3A, () => loadUnit.LoadFromAddressAndIncrement(ref cpu.A, ref cpu.H, ref cpu.L, -1));
            AssertSingleCall(0x32, () => loadUnit.WriteToAddressAndIncrement(ref cpu.H, ref cpu.L, cpu.A, -1));
            AssertSingleCall(0x2A, () => loadUnit.LoadFromAddressAndIncrement(ref cpu.A, ref cpu.H, ref cpu.L, 1));
            AssertSingleCall(0x22, () => loadUnit.WriteToAddressAndIncrement(ref cpu.H, ref cpu.L, cpu.A, 1));
        }

        private void AssertSingleCall(byte opcode, Expression<Func<int>> expectedCall)
        {
            cpu.OpCodes[opcode]();
            A.CallTo(expectedCall).MustHaveHappenedOnceExactly();
            Fake.ClearRecordedCalls(loadUnit);
        }
    }
}