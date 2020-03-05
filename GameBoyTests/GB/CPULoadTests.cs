using Xunit;
using GameBoy.GB;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameBoy.GB.Tests
{
    public class CPULoadTests
    {
        [Fact()]
        public void LoadImmediate8Test()
        {
            var data = new byte[] {
                0x06, 0x01, // LD B, 0x06
                0x0E, 0x02, // LD C, 0x0E
                0x16, 0x03, // LD D, 0x16
                0x1E, 0x04, // LD E, 0x1E
                0x26, 0x05, // LD H, 0x26
                0x2E, 0x06, // LD L, 0x2E
            };
            var memory = new Memory(data);
            CPU cpu = new CPU(memory);

            cpu.RunCommand();
            Assert.Equal((byte)0x01, cpu.B);
            Assert.Equal(8, cpu.Cycles);

            cpu.RunCommand();
            Assert.Equal((byte)0x02, cpu.C);
            Assert.Equal(8, cpu.Cycles);

            cpu.RunCommand();
            Assert.Equal((byte)0x03, cpu.D);
            Assert.Equal(8, cpu.Cycles);

            cpu.RunCommand();
            Assert.Equal((byte)0x04, cpu.E);
            Assert.Equal(8, cpu.Cycles);

            cpu.RunCommand();
            Assert.Equal((byte)0x05, cpu.H);
            Assert.Equal(8, cpu.Cycles);

            cpu.RunCommand();
            Assert.Equal((byte)0x06, cpu.L);
            Assert.Equal(8, cpu.Cycles);
        }
    }
}