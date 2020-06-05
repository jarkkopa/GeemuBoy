using Xunit;

namespace GeemuBoyRomTests.GB.Blargg
{
    public class InstructionTests : TestBase
    {
        [Fact()]
        public void Individual01()
        {
            RunTest("Roms/blargg/gb-test-roms/cpu_instrs/individual/01-special.gb", 0xC7D2);
        }

        [Fact()]
        public void Individual02()
        {
            RunTest("Roms/blargg/gb-test-roms/cpu_instrs/individual/02-interrupts.gb", 0xC7F4);
        }

        [Fact()]
        public void Individual03()
        {
            RunTest("Roms/blargg/gb-test-roms/cpu_instrs/individual/03-op sp,hl.gb", 0xCB44);
        }

        [Fact()]
        public void Individual04()
        {
            RunTest("Roms/blargg/gb-test-roms/cpu_instrs/individual/04-op r,imm.gb", 0xCB35);
        }

        [Fact()]
        public void Individual05()
        {
            RunTest("Roms/blargg/gb-test-roms/cpu_instrs/individual/05-op rp.gb", 0xCB31);
        }

        [Fact()]
        public void Individual06()
        {
            RunTest("Roms/blargg/gb-test-roms/cpu_instrs/individual/06-ld r,r.gb", 0xCC5F);
        }

        [Fact()]
        public void Individual07()
        {
            RunTest("Roms/blargg/gb-test-roms/cpu_instrs/individual/07-jr,jp,call,ret,rst.gb", 0xCBB0);
        }

        [Fact()]
        public void Individual08()
        {
            RunTest("Roms/blargg/gb-test-roms/cpu_instrs/individual/08-misc instrs.gb", 0xCB91);
        }

        [Fact()]
        public void Individual09()
        {
            RunTest("Roms/blargg/gb-test-roms/cpu_instrs/individual/09-op r,r.gb", 0xCE67);
        }

        [Fact()]
        public void Individual10()
        {
            RunTest("Roms/blargg/gb-test-roms/cpu_instrs/individual/10-bit ops.gb", 0xCF58);
        }

        [Fact()]
        public void Individual11()
        {
            RunTest("Roms/blargg/gb-test-roms/cpu_instrs/individual/11-op a,(hl).gb", 0xCC62);
        }
    }
}
