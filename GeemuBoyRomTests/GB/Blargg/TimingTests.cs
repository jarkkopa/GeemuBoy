using GeemuBoy.GB;
using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace GeemuBoyRomTests.GB.Blargg
{
    public class TimingTests : TestBase
    {
        [Fact()]
        public void InstructionTimings()
        {
            RunTest("Roms/blargg/gb-test-roms/instr_timing/instr_timing.gb", 0xC8B0);
        }
    }
}
