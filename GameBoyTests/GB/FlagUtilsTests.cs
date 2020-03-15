using Xunit;

namespace GameBoy.GB.Tests
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
        public void SetZeroFlagTest()
        {
            byte flags = 0b01110000;

            FlagUtils.SetFlag(Flag.Z, true, ref flags);
            FlagUtils.SetFlag(Flag.N, true, ref flags);
            FlagUtils.SetFlag(Flag.H, true, ref flags);
            FlagUtils.SetFlag(Flag.C, true, ref flags);

            Assert.Equal(0b11110000, flags);
        }

        [Fact()]
        public void ClearZeroFlagTest()
        {
            byte flags = 0b01010000;

            FlagUtils.SetFlag(Flag.Z, false, ref flags);
            FlagUtils.SetFlag(Flag.N, false, ref flags);
            FlagUtils.SetFlag(Flag.H, false, ref flags);
            FlagUtils.SetFlag(Flag.C, false, ref flags);

            Assert.Equal(0b00000000, flags);
        }
    }
}