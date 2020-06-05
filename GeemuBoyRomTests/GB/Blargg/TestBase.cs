using GeemuBoy.GB;
using System;
using System.Linq;
using Xunit;

namespace GeemuBoyRomTests.GB.Blargg
{
    public abstract class TestBase
    {
        public void RunTest(string path, ushort stopAddress, Action<Memory>? assertFunction = null)
        {
            var emulator = new TestEmulator(path, stopAddress);
            emulator.Run(assertFunction ?? AssertResult);
        }

        private static void AssertResult(Memory memory)
        {
            var result = string.Concat(memory.Serial.Select(s => (char)s));

            Assert.DoesNotContain("Failed", result);
            Assert.Contains("Passed", result);
        }
    }
}
