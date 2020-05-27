namespace GeemuBoy.GB
{
    public class Timer
    {
        private const ushort DIV = 0xFF04;
        private const ushort TIMA = 0xFF05;
        private const ushort TMA = 0xFF06;
        private const ushort TAC = 0xFF07;

        private readonly Memory memory;

        public ushort Counter { get; private set; } = 0;
        private ushort previousCounter = 0;

        public Timer(Memory memory)
        {
            this.memory = memory;
        }

        public void Update(int cpuCycles)
        {
            for (int i = 0; i < cpuCycles; i++)
            {
                UpdateCounter(1);
                UpdateTIMA();
            }
        }

        private void UpdateTIMA()
        {
            byte tacValue = memory.ReadByte(TAC);
            int counterBit = (tacValue & 0x3) switch
            {
                0 => 9,
                1 => 3,
                2 => 5,
                3 => 7,
                _ => 7
            };

            if (previousCounter.IsBitSet(counterBit) && !(Counter.IsBitSet(counterBit) & tacValue.IsBitSet(2)))
            {
                byte value = memory.ReadByte(TIMA);
                if (value == 0xFF)
                {
                    byte timerModulo = memory.ReadByte(TMA);
                    memory.WriteByte(TIMA, timerModulo);
                    CPU.RequestInterrupt(memory, CPU.Interrupt.Timer);
                }
                else
                {
                    memory.WriteByte(TIMA, (byte)(value + 1));
                }
            }
            if (tacValue.IsBitSet(2))
            {
                previousCounter = Counter;
            }
        }

        public void ResetCounter()
        {
            Counter = 0;
        }

        private void UpdateCounter(int cpuCycles)
        {
            Counter = (ushort)(cpuCycles + Counter);

            memory.WriteByte(DIV, (byte)((Counter & 0xFF00) >> 8), false);
        }
    }
}
