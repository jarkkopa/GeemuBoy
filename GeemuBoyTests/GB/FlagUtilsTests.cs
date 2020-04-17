using Xunit;

namespace GeemuBoy.GB.Tests
{
    public class FlagUtilsTests
    {
        [Fact()]
        public void GetFlagTest()
        {
            byte flags = 0b01010000;
            Assert.False(FlagUtils.GetFlag(Flag.Z, flags));
            Assert.True(FlagUtils.GetFlag(Flag.N, flags));
            Assert.False(FlagUtils.GetFlag(Flag.H, flags));
            Assert.True(FlagUtils.GetFlag(Flag.C, flags));
        }

        [Fact()]
        public void SetFlagTest()
        {
            byte flags = 0b01110000;
            FlagUtils.SetFlags(ref flags,
                true,
                true,
                true,
                true);

            Assert.Equal(0b11110000, flags);
        }

        [Fact()]
        public void ResetFlagsTest()
        {
            byte flags = 0b01010000;
            FlagUtils.SetFlags(ref flags,
                false,
                false,
                false,
                false);

            Assert.Equal(0b00000000, flags);
        }
    }
}