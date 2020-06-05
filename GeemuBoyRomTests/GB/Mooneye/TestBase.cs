using GeemuBoy.GB;
using Xunit;

namespace GeemuBoyRomTests.GB.Mooneye
{
    public abstract class TestBase
    {
        protected void RunTest(string path, ushort stopAddress)
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
