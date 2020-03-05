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

        [Fact]
        public void CopyToRegisterTest()
        {
            var data = new byte[] {
                0x7F, // LD A, A
                0x78, // LD A, B
                0x79, // LD A, C
                0x7A, // LD A, D
                0x7B, // LD A, E
                0x7C, // LD A, H
                0x7D, // LD A, L
            };
            var memory = new Memory(data);
            CPU cpu = new CPU(memory);
            cpu.A = 0x01;
            cpu.B = 0x02;
            cpu.C = 0x03;
            cpu.D = 0x04;
            cpu.E = 0x05;
            cpu.H = 0x06;
            cpu.L = 0x07;

            cpu.RunCommand();
            Assert.Equal(0x01, cpu.A);
            Assert.Equal(4, cpu.Cycles);

            cpu.RunCommand();
            Assert.Equal(0x02, cpu.A);
            Assert.Equal(4, cpu.Cycles);

            cpu.RunCommand();
            Assert.Equal(0x03, cpu.A);
            Assert.Equal(4, cpu.Cycles);

            cpu.RunCommand();
            Assert.Equal(0x04, cpu.A);
            Assert.Equal(4, cpu.Cycles);

            cpu.RunCommand();
            Assert.Equal(0x05, cpu.A);
            Assert.Equal(4, cpu.Cycles);

            cpu.RunCommand();
            Assert.Equal(0x06, cpu.A);
            Assert.Equal(4, cpu.Cycles);

            cpu.RunCommand();
            Assert.Equal(0x07, cpu.A);
            Assert.Equal(4, cpu.Cycles);
        }

        [Fact]
        public void CopyFromAddressTest()
        {
            var data = new byte[] {
                0x7E, // LD A, (HL)
            };
            var memory = new Memory(data);
            CPU cpu = new CPU(memory);
            
            cpu.H = 0xAA;
            cpu.L = 0xBB;
            memory.WriteByte(0xAABB, 0xFF);

            cpu.RunCommand();

            Assert.Equal(0xFF, cpu.A);
            Assert.Equal(8, cpu.Cycles);
        }
    }
}