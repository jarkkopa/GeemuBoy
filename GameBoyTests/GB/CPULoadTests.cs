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
            CPU cpu = new CPU(new byte[] {
                0x06, 0x01, // LD B, 0x06
                0x0E, 0x02, // LD C, 0x0E
                0x16, 0x03, // LD D, 0x16
                0x1E, 0x04, // LD E, 0x1E
                0x26, 0x05, // LD H, 0x26
                0x2E, 0x06, // LD L, 0x2E
            });

            cpu.RunCommand();
            Assert.Equal((byte)0x01, cpu.b);
            Assert.Equal(8, cpu.Cycles);

            cpu.RunCommand();
            Assert.Equal((byte)0x02, cpu.c);
            Assert.Equal(16, cpu.Cycles);

            cpu.RunCommand();
            Assert.Equal((byte)0x03, cpu.d);
            Assert.Equal(24, cpu.Cycles);

            cpu.RunCommand();
            Assert.Equal((byte)0x04, cpu.e);
            Assert.Equal(32, cpu.Cycles);

            cpu.RunCommand();
            Assert.Equal((byte)0x05, cpu.h);
            Assert.Equal(40, cpu.Cycles);

            cpu.RunCommand();
            Assert.Equal((byte)0x06, cpu.l);
            Assert.Equal(48, cpu.Cycles);
        }
    }
}