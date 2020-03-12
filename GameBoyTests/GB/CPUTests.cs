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
            AssertSingleCall(0x06, () => _loadUnit.LoadImmediate8(ref _cpu.B, ref _cpu.PC));
            AssertSingleCall(0x0E, () => _loadUnit.LoadImmediate8(ref _cpu.C, ref _cpu.PC));
            AssertSingleCall(0x16, () => _loadUnit.LoadImmediate8(ref _cpu.D, ref _cpu.PC));
            AssertSingleCall(0x1E, () => _loadUnit.LoadImmediate8(ref _cpu.E, ref _cpu.PC));
            AssertSingleCall(0x26, () => _loadUnit.LoadImmediate8(ref _cpu.H, ref _cpu.PC));
            AssertSingleCall(0x2E, () => _loadUnit.LoadImmediate8(ref _cpu.L, ref _cpu.PC));

            AssertSingleCall(0x7F, () => _loadUnit.Copy(ref _cpu.A, ref _cpu.A));
            AssertSingleCall(0x78, () => _loadUnit.Copy(ref _cpu.A, ref _cpu.B));
            AssertSingleCall(0x79, () => _loadUnit.Copy(ref _cpu.A, ref _cpu.C));
            AssertSingleCall(0x7A, () => _loadUnit.Copy(ref _cpu.A, ref _cpu.D));
            AssertSingleCall(0x7B, () => _loadUnit.Copy(ref _cpu.A, ref _cpu.E));
            AssertSingleCall(0x7C, () => _loadUnit.Copy(ref _cpu.A, ref _cpu.H));
            AssertSingleCall(0x7D, () => _loadUnit.Copy(ref _cpu.A, ref _cpu.L));
        }

        private void AssertSingleCall(byte opcode, Expression<Func<int>> expectedCall)
        {
            _cpu.OpCodes[opcode]();
            A.CallTo(expectedCall).MustHaveHappenedOnceExactly();
            Fake.ClearRecordedCalls(_loadUnit);
        }
    }
}