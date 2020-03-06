using Xunit;
using GameBoy.GB;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameBoy.GB.Tests
{
    public class CPU8BitLoadTests
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
        public void CopyToRegisterATest()
        {
            var data = new byte[] {
                0x7F, // LD A, A
                0x78, // LD A, B
                0x79, // LD A, C
                0x7A, // LD A, D
                0x7B, // LD A, E
                0x7C, // LD A, H
                0x7D, // LD A, L
                0x3E, 0xFF // LD A, n
            };
            var memory = new Memory(data);
            CPU cpu = new CPU(memory)
            {
                A = 0x01,
                B = 0x02,
                C = 0x03,
                D = 0x04,
                E = 0x05,
                H = 0x06,
                L = 0x07
            };

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

            cpu.RunCommand();
            Assert.Equal(0xFF, cpu.A);
            Assert.Equal(8, cpu.Cycles);
        }

        [Fact]
        public void CopyToRegisterBTest()
        {
            var data = new byte[] {
                0x40, // LD B, B
                0x41, // LD B, C
                0x42, // LD B, D
                0x43, // LD B, E
                0x44, // LD B, H
                0x45, // LD B, L
            };
            var memory = new Memory(data);
            CPU cpu = new CPU(memory)
            {
                B = 0x02,
                C = 0x03,
                D = 0x04,
                E = 0x05,
                H = 0x06,
                L = 0x07
            };

            cpu.RunCommand();
            Assert.Equal(0x02, cpu.B);
            Assert.Equal(4, cpu.Cycles);

            cpu.RunCommand();
            Assert.Equal(0x03, cpu.B);
            Assert.Equal(4, cpu.Cycles);

            cpu.RunCommand();
            Assert.Equal(0x04, cpu.B);
            Assert.Equal(4, cpu.Cycles);

            cpu.RunCommand();
            Assert.Equal(0x05, cpu.B);
            Assert.Equal(4, cpu.Cycles);

            cpu.RunCommand();
            Assert.Equal(0x06, cpu.B);
            Assert.Equal(4, cpu.Cycles);

            cpu.RunCommand();
            Assert.Equal(0x07, cpu.B);
            Assert.Equal(4, cpu.Cycles);
        }

        [Fact]
        public void CopyToRegisterCTest()
        {
            var data = new byte[] {
                0x48, // LD C, B
                0x49, // LD C, C
                0x4A, // LD C, D
                0x4B, // LD C, E
                0x4C, // LD C, H
                0x4D, // LD C, L
            };
            var memory = new Memory(data);
            CPU cpu = new CPU(memory)
            {
                B = 0x02,
                C = 0x03,
                D = 0x04,
                E = 0x05,
                H = 0x06,
                L = 0x07
            };

            cpu.RunCommand();
            Assert.Equal(0x02, cpu.C);
            Assert.Equal(4, cpu.Cycles);

            cpu.C = 0x03;
            cpu.RunCommand();
            Assert.Equal(0x03, cpu.C);
            Assert.Equal(4, cpu.Cycles);

            cpu.RunCommand();
            Assert.Equal(0x04, cpu.C);
            Assert.Equal(4, cpu.Cycles);

            cpu.RunCommand();
            Assert.Equal(0x05, cpu.C);
            Assert.Equal(4, cpu.Cycles);

            cpu.RunCommand();
            Assert.Equal(0x06, cpu.C);
            Assert.Equal(4, cpu.Cycles);

            cpu.RunCommand();
            Assert.Equal(0x07, cpu.C);
            Assert.Equal(4, cpu.Cycles);
        }

        [Fact]
        public void CopyToRegisterDTest()
        {
            var data = new byte[] {
                0x50, // LD D, B
                0x51, // LD D, C
                0x52, // LD D, D
                0x53, // LD D, E
                0x54, // LD D, H
                0x55, // LD D, L
            };
            var memory = new Memory(data);
            CPU cpu = new CPU(memory)
            {
                B = 0x02,
                C = 0x03,
                D = 0x04,
                E = 0x05,
                H = 0x06,
                L = 0x07
            };

            cpu.RunCommand();
            Assert.Equal(0x02, cpu.D);
            Assert.Equal(4, cpu.Cycles);

            cpu.RunCommand();
            Assert.Equal(0x03, cpu.D);
            Assert.Equal(4, cpu.Cycles);

            cpu.D = 0x04;
            cpu.RunCommand();
            Assert.Equal(0x04, cpu.D);
            Assert.Equal(4, cpu.Cycles);

            cpu.RunCommand();
            Assert.Equal(0x05, cpu.D);
            Assert.Equal(4, cpu.Cycles);

            cpu.RunCommand();
            Assert.Equal(0x06, cpu.D);
            Assert.Equal(4, cpu.Cycles);

            cpu.RunCommand();
            Assert.Equal(0x07, cpu.D);
            Assert.Equal(4, cpu.Cycles);
        }

        [Fact]
        public void CopyToRegisterETest()
        {
            var data = new byte[] {
                0x58, // LD E, B
                0x59, // LD E, C
                0x5A, // LD E, D
                0x5B, // LD E, E
                0x5C, // LD E, H
                0x5D, // LD E, L
            };
            var memory = new Memory(data);
            CPU cpu = new CPU(memory)
            {
                B = 0x02,
                C = 0x03,
                D = 0x04,
                E = 0x05,
                H = 0x06,
                L = 0x07
            };

            cpu.RunCommand();
            Assert.Equal(0x02, cpu.E);
            Assert.Equal(4, cpu.Cycles);

            cpu.RunCommand();
            Assert.Equal(0x03, cpu.E);
            Assert.Equal(4, cpu.Cycles);

            cpu.RunCommand();
            Assert.Equal(0x04, cpu.E);
            Assert.Equal(4, cpu.Cycles);

            cpu.E = 0x05;
            cpu.RunCommand();
            Assert.Equal(0x05, cpu.E);
            Assert.Equal(4, cpu.Cycles);

            cpu.RunCommand();
            Assert.Equal(0x06, cpu.E);
            Assert.Equal(4, cpu.Cycles);

            cpu.RunCommand();
            Assert.Equal(0x07, cpu.E);
            Assert.Equal(4, cpu.Cycles);
        }

        [Fact]
        public void CopyToRegisterHTest()
        {
            var data = new byte[] {
                0x60, // LD H, B
                0x61, // LD H, C
                0x62, // LD H, D
                0x63, // LD H, E
                0x64, // LD H, H
                0x65, // LD H, L
            };
            var memory = new Memory(data);
            CPU cpu = new CPU(memory)
            {
                B = 0x02,
                C = 0x03,
                D = 0x04,
                E = 0x05,
                H = 0x06,
                L = 0x07
            };

            cpu.RunCommand();
            Assert.Equal(0x02, cpu.H);
            Assert.Equal(4, cpu.Cycles);

            cpu.RunCommand();
            Assert.Equal(0x03, cpu.H);
            Assert.Equal(4, cpu.Cycles);

            cpu.RunCommand();
            Assert.Equal(0x04, cpu.H);
            Assert.Equal(4, cpu.Cycles);

            cpu.RunCommand();
            Assert.Equal(0x05, cpu.H);
            Assert.Equal(4, cpu.Cycles);

            cpu.H = 0x06;
            cpu.RunCommand();
            Assert.Equal(0x06, cpu.H);
            Assert.Equal(4, cpu.Cycles);

            cpu.RunCommand();
            Assert.Equal(0x07, cpu.H);
            Assert.Equal(4, cpu.Cycles);
        }

        [Fact]
        public void CopyToRegisterLTest()
        {
            var data = new byte[] {
                0x68, // LD L, B
                0x69, // LD L, C
                0x6A, // LD L, D
                0x6B, // LD L, E
                0x6C, // LD L, H
                0x6D, // LD L, L
            };
            var memory = new Memory(data);
            CPU cpu = new CPU(memory)
            {
                B = 0x02,
                C = 0x03,
                D = 0x04,
                E = 0x05,
                H = 0x06,
                L = 0x07
            };

            cpu.RunCommand();
            Assert.Equal(0x02, cpu.L);
            Assert.Equal(4, cpu.Cycles);

            cpu.RunCommand();
            Assert.Equal(0x03, cpu.L);
            Assert.Equal(4, cpu.Cycles);

            cpu.RunCommand();
            Assert.Equal(0x04, cpu.L);
            Assert.Equal(4, cpu.Cycles);

            cpu.RunCommand();
            Assert.Equal(0x05, cpu.L);
            Assert.Equal(4, cpu.Cycles);

            cpu.RunCommand();
            Assert.Equal(0x06, cpu.L);
            Assert.Equal(4, cpu.Cycles);

            cpu.L = 0x07;
            cpu.RunCommand();
            Assert.Equal(0x07, cpu.L);
            Assert.Equal(4, cpu.Cycles);
        }

        [Fact]
        public void CopyFromAddressTest()
        {
            var data = new byte[] {
                0x7E, // LD A, (HL)
                0x46, // LD B, (HL)
                0x4E, // LD C, (HL)
                0x56, // LD D, (HL)
                0x5E, // LD E, (HL)
                0x66, // LD H, (HL)
                0x6E, // LD L, (HL)
            };
            var memory = new Memory(data);
            CPU cpu = new CPU(memory)
            {
                H = 0xCC,
                L = 0xDD
            };
            memory.WriteByte(0xCCDD, 0xFF);

            cpu.RunCommand();
            Assert.Equal(0xFF, cpu.A);
            Assert.Equal(8, cpu.Cycles);

            cpu.RunCommand();
            Assert.Equal(0xFF, cpu.B);
            Assert.Equal(8, cpu.Cycles);

            cpu.RunCommand();
            Assert.Equal(0xFF, cpu.C);
            Assert.Equal(8, cpu.Cycles);

            cpu.RunCommand();
            Assert.Equal(0xFF, cpu.D);
            Assert.Equal(8, cpu.Cycles);

            cpu.RunCommand();
            Assert.Equal(0xFF, cpu.E);
            Assert.Equal(8, cpu.Cycles);

            cpu.RunCommand();
            Assert.Equal(0xFF, cpu.H);
            Assert.Equal(8, cpu.Cycles);

            cpu.H = 0xCC;
            cpu.RunCommand();
            Assert.Equal(0xFF, cpu.L);
            Assert.Equal(8, cpu.Cycles);
        }

        [Fact]
        public void CopyFromAddressRegistersTest()
        {
            var data = new byte[] {
                0x0A, // LD A, (BC)
                0x1A, // LD A, (DE)
                0xFA, 0xAB, 0xBA // LD A, (nn)
            };
            var memory = new Memory(data);
            var cpu = new CPU(memory)
            {
                B = 0xC0,
                C = 0x11,
                D = 0xC1,
                E = 0x22
            };

            memory.WriteByte(0xC011, 0xAA);
            cpu.RunCommand();
            Assert.Equal(0xAA, cpu.A);
            Assert.Equal(8, cpu.Cycles);

            memory.WriteByte(0xC122, 0xBB);
            cpu.RunCommand();
            Assert.Equal(0xBB, cpu.A);
            Assert.Equal(8, cpu.Cycles);

            memory.WriteByte(0xABBA, 0x66);
            cpu.RunCommand();
            Assert.Equal(0x66, cpu.A);
            Assert.Equal(16, cpu.Cycles);
        }

        [Fact]
        public void CopyToAddressTest()
        {
            var data = new byte[] {
                0x70, // LD (HL), B
                0x71, // LD (HL), C
                0x72, // LD (HL), D
                0x73, // LD (HL), E
                0x74, // LD (HL), H
                0x75, // LD (HL), L
                0x36, 0xFF // LD (HL), n
            };
            var memory = new Memory(data);
            CPU cpu = new CPU(memory)
            {
                B = 0x02,
                C = 0x03,
                D = 0x04,
                E = 0x05,
                H = 0xCC,
                L = 0xDD
            };

            cpu.RunCommand();
            Assert.Equal(0x02, memory.ReadByte(0xCCDD));
            Assert.Equal(8, cpu.Cycles);

            cpu.RunCommand();
            Assert.Equal(0x03, memory.ReadByte(0xCCDD));
            Assert.Equal(8, cpu.Cycles);

            cpu.RunCommand();
            Assert.Equal(0x04, memory.ReadByte(0xCCDD));
            Assert.Equal(8, cpu.Cycles);

            cpu.RunCommand();
            Assert.Equal(0x05, memory.ReadByte(0xCCDD));
            Assert.Equal(8, cpu.Cycles);

            cpu.RunCommand();
            Assert.Equal(0xCC, memory.ReadByte(0xCCDD));
            Assert.Equal(8, cpu.Cycles);

            cpu.RunCommand();
            Assert.Equal(0xDD, memory.ReadByte(0xCCDD));
            Assert.Equal(8, cpu.Cycles);

            cpu.RunCommand();
            Assert.Equal(0xFF, memory.ReadByte(0xCCDD));
            Assert.Equal(12, cpu.Cycles);
        }
    }
}