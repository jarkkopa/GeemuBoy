﻿using GeemuBoyRomTests.GB.Mooneye;
using Xunit;

namespace GeemuBoyRomTests.GB.Mooneye
{
    public class TimerTests : TestBase
    {
        [Fact()]
        public void DivWriteTest()
        {
            RunTest("Roms/mooneye/acceptance/timer/div_write.gb", 0x486E);
        }

        [Fact(Skip = "Need to sync timer per machine cycle to pass this")]
        public void RapidToggleTest()
        {
            RunTest("Roms/mooneye/acceptance/timer/rapid_toggle.gb", 0x4B2E);
        }

        [Fact()]
        public void Tim00Test()
        {
            RunTest("Roms/mooneye/acceptance/timer/tim00.gb", 0x4B2E);
        }

        [Fact()]
        public void Tim00DivTriggerTest()
        {
            RunTest("Roms/mooneye/acceptance/timer/tim00_div_trigger.gb", 0x4B2E);
        }

        [Fact()]
        public void Tim01Test()
        {
            RunTest("Roms/mooneye/acceptance/timer/tim01.gb", 0x4B2E);
        }

        [Fact()]
        public void Tim01DivTriggerTest()
        {
            RunTest("Roms/mooneye/acceptance/timer/tim01_div_trigger.gb", 0x4B2E);
        }

        [Fact()]
        public void Tim10Test()
        {
            RunTest("Roms/mooneye/acceptance/timer/tim10.gb", 0x4B2E);
        }

        [Fact()]
        public void Tim10DivTriggerTest()
        {
            RunTest("Roms/mooneye/acceptance/timer/tim10_div_trigger.gb", 0x4B2E);
        }

        [Fact()]
        public void Tim11Test()
        {
            RunTest("Roms/mooneye/acceptance/timer/tim11.gb", 0x4B2E);
        }

        [Fact()]
        public void Tim11DivTriggerTest()
        {
            RunTest("Roms/mooneye/acceptance/timer/tim11_div_trigger.gb", 0x4B2E);
        }

        [Fact(Skip = "Not implemented")]
        public void TimaReloadTest()
        {
            RunTest("Roms/mooneye/acceptance/timer/tima_reload.gb", 0x4B2E);
        }

        [Fact(Skip = "Not implemented")]
        public void TimaWriteReloading()
        {
            RunTest("Roms/mooneye/acceptance/timer/tima_write_reloading.gb", 0x4B2E);
        }

        [Fact(Skip = "Not implemented")]
        public void TmaWriteReloading()
        {
            RunTest("Roms/mooneye/acceptance/timer/tma_write_reloading.gb", 0x4B2E);
        }
    }
}