using FakeItEasy;
using GeemuBoy.GB.CpuUnits;
using Xunit;

namespace GeemuBoy.GB.Tests
{
    public class CPUTests
    {
        private const byte IMMEDIATE_BYTE = 0x01;
        private const ushort IMMEDIATE_WORD = 0x2301;
        private const byte MEM_HL_BYTE = 0x45;

        private readonly Memory memory;
        private readonly PPU ppu;
        private readonly IDisplay display;
        private readonly CPU cpu;
        private readonly ILoadUnit loadUnit;
        private readonly IALU alu;
        private readonly IMiscUnit miscUnit;
        private readonly IJumpUnit jumpUnit;
        private readonly IBitUnit bitUnit;

        public CPUTests()
        {
            memory = new Memory(new byte[]
            {
                IMMEDIATE_BYTE,
                (IMMEDIATE_WORD >> 8) & 0xFF
            });
            display = new BlankDisplay();
            ppu = new PPU(memory, display);
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

        [Fact()]
        public void EIEnablesInterruptMasterFlagWithDelay()
        {
            var memory = new Memory(new byte[]{
                0xFB, //EI
                0x00 //NOP
            });
            var cpu = new CPU(memory, display)
            {
                InterruptMasterEnableFlag = false
            };

            cpu.RunCommand();
            Assert.False(cpu.InterruptMasterEnableFlag);
            cpu.RunCommand();
            Assert.True(cpu.InterruptMasterEnableFlag);
        }

        [Fact()]
        public void DIDisablesInterruptMasterFlagImmediately()
        {
            var memory = new Memory(new byte[]{
                0xF3 //DI
            });
            var cpu = new CPU(memory, display)
            {
                InterruptMasterEnableFlag = true
            };

            cpu.RunCommand();
            Assert.False(cpu.InterruptMasterEnableFlag);
        }

        [Fact()]
        public void RETIEnablesInterruptMasterFlagImmediately()
        {
            var memory = new Memory(new byte[]{
                0xD9 // RETI
            });
            var cpu = new CPU(memory, display)
            {
                SP = 0xFFFA,
                InterruptMasterEnableFlag = false
            };
            memory.WriteByte(0xFFFA, 0x23);
            memory.WriteByte(0xFFFB, 0x01);
            
            cpu.RunCommand();

            Assert.True(cpu.InterruptMasterEnableFlag);
            Assert.Equal(0x0123, cpu.PC);
            Assert.Equal(0xFFFC, cpu.SP);
        }

        [Fact()]
        public void JumpToHighestPriorityInterruptVectorTest()
        {
            var memory = new Memory(new byte[]{
                0xFB, // DI, enable interrupts
                0x00, // NOP, interrupts enabled after this instruction
            });
            var cpu = new CPU(memory, display)
            {
                SP = 0xFFFE
            };

            CPU.RequestInterrupt(memory, CPU.Interrupt.LCDStat);
            CPU.RequestInterrupt(memory, CPU.Interrupt.VBlank);
            CPU.RequestInterrupt(memory, CPU.Interrupt.Serial);
            // Enable LCDStat and Serial
            memory.WriteByte(0xFFFF, 0b00000110);
            cpu.RunCommand();
            Assert.Equal(0x1, cpu.PC);
            Assert.False(cpu.InterruptMasterEnableFlag);
            cpu.RunCommand();

            // Highest enabled and requested interrupt is LCDStat with interrupt vector 0x48
            Assert.Equal(0x48, cpu.PC);
            Assert.Equal(0xFFFC, cpu.SP);
            Assert.Equal(0x0002, memory.ReadWord(0xFFFC));
        }
    }
}