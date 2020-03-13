using FakeItEasy;
using GameBoy.GB.CpuUnits;
using System;
using System.Linq.Expressions;
using Xunit;

namespace GameBoy.GB.Tests
{
    public class CPUTests
    {
        private readonly Memory _memory;
        private readonly ILoadUnit _loadUnit;
        private readonly CPU _cpu;

        public CPUTests()
        {
            _memory = new Memory(new byte[] { });
            _loadUnit = A.Fake<ILoadUnit>();
            _cpu = new CPU(_memory, _loadUnit)
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
        public void RunCommandTest()
        {
            AssertSingleCall(0x06, () => _loadUnit.LoadImmediateByte(ref _cpu.B, ref _cpu.PC));
            AssertSingleCall(0x0E, () => _loadUnit.LoadImmediateByte(ref _cpu.C, ref _cpu.PC));
            AssertSingleCall(0x16, () => _loadUnit.LoadImmediateByte(ref _cpu.D, ref _cpu.PC));
            AssertSingleCall(0x1E, () => _loadUnit.LoadImmediateByte(ref _cpu.E, ref _cpu.PC));
            AssertSingleCall(0x26, () => _loadUnit.LoadImmediateByte(ref _cpu.H, ref _cpu.PC));
            AssertSingleCall(0x2E, () => _loadUnit.LoadImmediateByte(ref _cpu.L, ref _cpu.PC));

            AssertSingleCall(0x7F, () => _loadUnit.Copy(ref _cpu.A, ref _cpu.A));
            AssertSingleCall(0x78, () => _loadUnit.Copy(ref _cpu.A, ref _cpu.B));
            AssertSingleCall(0x79, () => _loadUnit.Copy(ref _cpu.A, ref _cpu.C));
            AssertSingleCall(0x7A, () => _loadUnit.Copy(ref _cpu.A, ref _cpu.D));
            AssertSingleCall(0x7B, () => _loadUnit.Copy(ref _cpu.A, ref _cpu.E));
            AssertSingleCall(0x7C, () => _loadUnit.Copy(ref _cpu.A, ref _cpu.H));
            AssertSingleCall(0x7D, () => _loadUnit.Copy(ref _cpu.A, ref _cpu.L));
            AssertSingleCall(0x7E, () => _loadUnit.LoadFromAddress(ref _cpu.A, _cpu.H, _cpu.L));
            AssertSingleCall(0x40, () => _loadUnit.Copy(ref _cpu.B, ref _cpu.B));
            AssertSingleCall(0x41, () => _loadUnit.Copy(ref _cpu.B, ref _cpu.C));
            AssertSingleCall(0x42, () => _loadUnit.Copy(ref _cpu.B, ref _cpu.D));
            AssertSingleCall(0x43, () => _loadUnit.Copy(ref _cpu.B, ref _cpu.E));
            AssertSingleCall(0x44, () => _loadUnit.Copy(ref _cpu.B, ref _cpu.H));
            AssertSingleCall(0x45, () => _loadUnit.Copy(ref _cpu.B, ref _cpu.L));
            AssertSingleCall(0x46, () => _loadUnit.LoadFromAddress(ref _cpu.B, _cpu.H, _cpu.L));
            AssertSingleCall(0x48, () => _loadUnit.Copy(ref _cpu.C, ref _cpu.B));
            AssertSingleCall(0x49, () => _loadUnit.Copy(ref _cpu.C, ref _cpu.C));
            AssertSingleCall(0x4A, () => _loadUnit.Copy(ref _cpu.C, ref _cpu.D));
            AssertSingleCall(0x4B, () => _loadUnit.Copy(ref _cpu.C, ref _cpu.E));
            AssertSingleCall(0x4C, () => _loadUnit.Copy(ref _cpu.C, ref _cpu.H));
            AssertSingleCall(0x4D, () => _loadUnit.Copy(ref _cpu.C, ref _cpu.L));
            AssertSingleCall(0x4E, () => _loadUnit.LoadFromAddress(ref _cpu.C, _cpu.H, _cpu.L));
            AssertSingleCall(0x50, () => _loadUnit.Copy(ref _cpu.D, ref _cpu.B));
            AssertSingleCall(0x51, () => _loadUnit.Copy(ref _cpu.D, ref _cpu.C));
            AssertSingleCall(0x52, () => _loadUnit.Copy(ref _cpu.D, ref _cpu.D));
            AssertSingleCall(0x53, () => _loadUnit.Copy(ref _cpu.D, ref _cpu.E));
            AssertSingleCall(0x54, () => _loadUnit.Copy(ref _cpu.D, ref _cpu.H));
            AssertSingleCall(0x55, () => _loadUnit.Copy(ref _cpu.D, ref _cpu.L));
            AssertSingleCall(0x56, () => _loadUnit.LoadFromAddress(ref _cpu.D, _cpu.H, _cpu.L));
            AssertSingleCall(0x58, () => _loadUnit.Copy(ref _cpu.E, ref _cpu.B));
            AssertSingleCall(0x59, () => _loadUnit.Copy(ref _cpu.E, ref _cpu.C));
            AssertSingleCall(0x5A, () => _loadUnit.Copy(ref _cpu.E, ref _cpu.D));
            AssertSingleCall(0x5B, () => _loadUnit.Copy(ref _cpu.E, ref _cpu.E));
            AssertSingleCall(0x5C, () => _loadUnit.Copy(ref _cpu.E, ref _cpu.H));
            AssertSingleCall(0x5D, () => _loadUnit.Copy(ref _cpu.E, ref _cpu.L));
            AssertSingleCall(0x5E, () => _loadUnit.LoadFromAddress(ref _cpu.E, _cpu.H, _cpu.L));
            AssertSingleCall(0x60, () => _loadUnit.Copy(ref _cpu.H, ref _cpu.B));
            AssertSingleCall(0x61, () => _loadUnit.Copy(ref _cpu.H, ref _cpu.C));
            AssertSingleCall(0x62, () => _loadUnit.Copy(ref _cpu.H, ref _cpu.D));
            AssertSingleCall(0x63, () => _loadUnit.Copy(ref _cpu.H, ref _cpu.E));
            AssertSingleCall(0x64, () => _loadUnit.Copy(ref _cpu.H, ref _cpu.H));
            AssertSingleCall(0x65, () => _loadUnit.Copy(ref _cpu.H, ref _cpu.L));
            AssertSingleCall(0x66, () => _loadUnit.LoadFromAddress(ref _cpu.H, _cpu.H, _cpu.L));
            AssertSingleCall(0x68, () => _loadUnit.Copy(ref _cpu.L, ref _cpu.B));
            AssertSingleCall(0x69, () => _loadUnit.Copy(ref _cpu.L, ref _cpu.C));
            AssertSingleCall(0x6A, () => _loadUnit.Copy(ref _cpu.L, ref _cpu.D));
            AssertSingleCall(0x6B, () => _loadUnit.Copy(ref _cpu.L, ref _cpu.E));
            AssertSingleCall(0x6C, () => _loadUnit.Copy(ref _cpu.L, ref _cpu.H));
            AssertSingleCall(0x6D, () => _loadUnit.Copy(ref _cpu.L, ref _cpu.L));
            AssertSingleCall(0x6E, () => _loadUnit.LoadFromAddress(ref _cpu.L, _cpu.H, _cpu.L));
            AssertSingleCall(0x70, () => _loadUnit.WriteToAddress(_cpu.H, _cpu.L, ref _cpu.B));
            AssertSingleCall(0x71, () => _loadUnit.WriteToAddress(_cpu.H, _cpu.L, ref _cpu.C));
            AssertSingleCall(0x72, () => _loadUnit.WriteToAddress(_cpu.H, _cpu.L, ref _cpu.D));
            AssertSingleCall(0x73, () => _loadUnit.WriteToAddress(_cpu.H, _cpu.L, ref _cpu.E));
            AssertSingleCall(0x74, () => _loadUnit.WriteToAddress(_cpu.H, _cpu.L, ref _cpu.H));
            AssertSingleCall(0x75, () => _loadUnit.WriteToAddress(_cpu.H, _cpu.L, ref _cpu.L));
            AssertSingleCall(0x36, () => _loadUnit.LoadImmediateByteToAddress(_cpu.H, _cpu.L, ref _cpu.PC));

            AssertSingleCall(0x0A, () => _loadUnit.LoadFromAddress(ref _cpu.A, _cpu.B, _cpu.C));
            AssertSingleCall(0x1A, () => _loadUnit.LoadFromAddress(ref _cpu.A, _cpu.D, _cpu.E));
            AssertSingleCall(0xFA, () => _loadUnit.LoadFromImmediateAddress(ref _cpu.A, ref _cpu.PC));
            AssertSingleCall(0x3E, () => _loadUnit.LoadImmediateByte(ref _cpu.A, ref _cpu.PC));

            AssertSingleCall(0x47, () => _loadUnit.Copy(ref _cpu.B, ref _cpu.A));
            AssertSingleCall(0x4F, () => _loadUnit.Copy(ref _cpu.C, ref _cpu.A));
            AssertSingleCall(0x57, () => _loadUnit.Copy(ref _cpu.D, ref _cpu.A));
            AssertSingleCall(0x5F, () => _loadUnit.Copy(ref _cpu.E, ref _cpu.A));
            AssertSingleCall(0x67, () => _loadUnit.Copy(ref _cpu.H, ref _cpu.A));
            AssertSingleCall(0x6F, () => _loadUnit.Copy(ref _cpu.L, ref _cpu.A));
            AssertSingleCall(0x02, () => _loadUnit.WriteToAddress(_cpu.B, _cpu.C, ref _cpu.A));
            AssertSingleCall(0x12, () => _loadUnit.WriteToAddress(_cpu.D, _cpu.E, ref _cpu.A));
            AssertSingleCall(0x77, () => _loadUnit.WriteToAddress(_cpu.H, _cpu.L, ref _cpu.A));
            AssertSingleCall(0xEA, () => _loadUnit.WriteToImmediateAddress(_cpu.A, ref _cpu.PC));
        }

        private void AssertSingleCall(byte opcode, Expression<Func<int>> expectedCall)
        {
            _cpu.OpCodes[opcode]();
            A.CallTo(expectedCall).MustHaveHappenedOnceExactly();
            Fake.ClearRecordedCalls(_loadUnit);
        }
    }
}