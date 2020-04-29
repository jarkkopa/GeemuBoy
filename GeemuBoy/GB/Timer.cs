namespace GeemuBoy.GB
{
    public class Timer
    {
        private const int CLOCKSPEED = 4194304;

        private const ushort DIV = 0xFF04;
        private const ushort TIMA = 0xFF05;
        private const ushort TMA = 0xFF06;
        private const ushort TAC = 0xFF07;

        private readonly Memory memory;

        private int cycles = 0;
        private int dividerCycles = 0;

        public Timer(Memory memory)
        {
            this.memory = memory;
        }

        public void Update(int cpuCycles)
        {
            UpdateDivider(cpuCycles);

            if (memory.ReadByte(TAC).IsBitSet(2))
            {
                cycles += cpuCycles;
                int clock = (memory.ReadByte(TAC) & 0x3) switch
                {
                    0 => 1024,
                    1 => 16,
                    2 => 64,
                    _ => 256
                };

                if (cycles >= clock)
                {
                    byte value = memory.ReadByte(TIMA);
                    if (value == 0xFF)
                    {
                        CPU.RequestInterrupt(memory, CPU.Interrupt.Timer);
                        byte timerModulo = memory.ReadByte(TMA);
                        memory.WriteByte(TIMA, timerModulo);
                    }
                    else
                    {
                        memory.WriteByte(TIMA, (byte)(value + 1));
                    }

                    cycles = 0;
                }
            }
        }

        private void UpdateDivider(int cpuCycles)
        {
            dividerCycles += cpuCycles;

            if (dividerCycles >= 255)
            {
                dividerCycles = 0;
                byte value = memory.ReadByte(DIV);
                value = (byte)(value == 0xFF ? 0 : value + 1);
                memory.WriteByte(DIV, value, false);
            }
        }
    }
}
