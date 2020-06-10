using Xunit;

namespace GeemuBoy.GB.Tests
{
    public class TimerTests
    {
        [Fact()]
        public void UpdateDividerRegister()
        {
            var memory = new Memory();
            var timer = new Timer(memory);

            // Divider should function despite timer being disabled
            memory.WriteByte(0xFF07, 0x00);

            // Reset divider by writing something to register
            memory.WriteByte(0xFF04, 0xFF);
            Assert.Equal(0x00, memory.ReadByte(0xFF04));

            timer.Update(252);
            Assert.Equal(0x0, memory.ReadByte(0xFF04));

            timer.Update(4);
            Assert.Equal(0x1, memory.ReadByte(0xFF04));

            for (int i = 1; i < 255; i++)
            {
                timer.Update(256);
                Assert.Equal(i + 1, memory.ReadByte(0xFF04));
            }

            timer.Update(256);
            Assert.Equal(0x00, memory.ReadByte(0xFF04));
        }

        [Fact()]
        public void UpdateTimerAt4096Hz()
        {
            int clock = 1024;

            var memory = new Memory();
            var timer = new Timer(memory);

            // Reset interrupts
            memory.WriteByte(0xFF0F, 0x00);

            // Set timer clock to 4096 Hz and disable timer
            memory.WriteByte(0xFF07, 0x00);

            Tick(clock, timer);
            // Timer is not updated when disabled
            Assert.Equal(0x00, memory.ReadByte(0xFF05));

            // Enable timer
            memory.WriteByte(0xFF07, 0x04);
            Tick(clock - 4, timer);
            Assert.Equal(0x0, memory.ReadByte(0xFF05));

            timer.Update(4);
            Assert.Equal(0x1, memory.ReadByte(0xFF05));
            for (int i = 1; i < 255; i++)
            {
                Tick(clock, timer);
                Assert.Equal(i + 1, memory.ReadByte(0xFF05));
            }

            // Overflow sets timer to modulo register value
            memory.WriteByte(0xFF06, 0xF0);
            Tick(clock, timer);
            Assert.Equal(0xF0, memory.ReadByte(0xFF05));

            // Interrupt is requested after the overflow
            Assert.True(memory.ReadByte(0xFF0F).IsBitSet(2));
        }

        [Fact()]
        public void UpdateTimerAt262144Hz()
        {
            int clock = 16;

            var memory = new Memory();
            var timer = new Timer(memory);

            memory.WriteByte(0xFF0F, 0x00);

            memory.WriteByte(0xFF07, 0x01);

            Tick(clock, timer);
            Assert.Equal(0x00, memory.ReadByte(0xFF05));

            memory.WriteByte(0xFF07, 0x05);
            Tick(clock - 4, timer);
            Assert.Equal(0x0, memory.ReadByte(0xFF05));

            timer.Update(4);
            Assert.Equal(0x1, memory.ReadByte(0xFF05));
            for (int i = 1; i < 255; i++)
            {
                Tick(clock, timer);
                Assert.Equal(i + 1, memory.ReadByte(0xFF05));
            }

            memory.WriteByte(0xFF06, 0x0);
            Tick(clock, timer);
            Assert.Equal(0x0, memory.ReadByte(0xFF05));

            Assert.True(memory.ReadByte(0xFF0F).IsBitSet(2));
        }

        [Fact()]
        public void UpdateTimerAt65536Hz()
        {
            int clock = 64;

            var memory = new Memory();
            var timer = new Timer(memory);

            memory.WriteByte(0xFF0F, 0x00);

            memory.WriteByte(0xFF07, 0x02);

            Tick(clock, timer);
            Assert.Equal(0x00, memory.ReadByte(0xFF05));

            memory.WriteByte(0xFF07, 0x06);
            Tick(clock - 4, timer);
            Assert.Equal(0x0, memory.ReadByte(0xFF05));

            timer.Update(4);
            Assert.Equal(0x1, memory.ReadByte(0xFF05));
            for (int i = 1; i < 255; i++)
            {
                Tick(clock, timer);
                Assert.Equal(i + 1, memory.ReadByte(0xFF05));
            }

            memory.WriteByte(0xFF06, 0x10);
            Tick(clock, timer);
            Assert.Equal(0x10, memory.ReadByte(0xFF05));

            Assert.True(memory.ReadByte(0xFF0F).IsBitSet(2));
        }

        [Fact()]
        public void UpdateTimerAt16384Hz()
        {
            int clock = 256;

            var memory = new Memory();
            var timer = new Timer(memory);

            memory.WriteByte(0xFF0F, 0x00);

            memory.WriteByte(0xFF07, 0x03);

            Tick(clock, timer);
            Assert.Equal(0x00, memory.ReadByte(0xFF05));

            memory.WriteByte(0xFF07, 0x07);
            Tick(clock - 4, timer);
            Assert.Equal(0x0, memory.ReadByte(0xFF05));

            timer.Update(4);
            Assert.Equal(0x1, memory.ReadByte(0xFF05));
            for (int i = 1; i < 255; i++)
            {
                Tick(clock, timer);
                Assert.Equal(i + 1, memory.ReadByte(0xFF05));
            }

            memory.WriteByte(0xFF06, 0xF0);
            Tick(clock, timer);
            Assert.Equal(0xF0, memory.ReadByte(0xFF05));

            Assert.True(memory.ReadByte(0xFF0F).IsBitSet(2));
        }

        private static void Tick(int cycles, Timer timer)
        {
            for (int i = 0; i < cycles / 4; i++)
            {
                timer.Update(4);
            }
        }
    }
}