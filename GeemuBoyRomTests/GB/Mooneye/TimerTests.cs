using GeemuBoyRomTests.GB;
using Xunit;

namespace GeemuBoy.GB.Mooneye
{
    public class TimerTests
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

        private void RunTest(string path, ushort stopAddress)
        {
            var emulator = new TestEmulator(path, stopAddress);
            emulator.Run(AssertResult);
        }

        private void AssertResult(Memory memory)
        {
            Assert.Equal(6, memory.Serial.Count);

            // Mooneye tests write a sequence of Fibonacci numbers to serial when the test pass.
            Assert.Equal(0x03, memory.Serial[0]);
            Assert.Equal(0x05, memory.Serial[1]);
            Assert.Equal(0x08, memory.Serial[2]);
            Assert.Equal(0x0D, memory.Serial[3]);
            Assert.Equal(0x15, memory.Serial[4]);
            Assert.Equal(0x22, memory.Serial[5]);
        }
    }
}