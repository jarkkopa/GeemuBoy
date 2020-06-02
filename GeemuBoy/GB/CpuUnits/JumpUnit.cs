namespace GeemuBoy.GB.CpuUnits
{
    public class JumpUnit
    {
        private readonly Memory memory;

        public event CPU.TickHandler? TickEvent;

        public JumpUnit(Memory memory)
        {
            this.memory = memory;
        }

        public void Call(ushort address, ref ushort sp, ref ushort pc)
        {
            sp = (ushort)(sp - 2);
            memory.WriteWord(sp, pc);
            TickEvent?.Invoke();
            TickEvent?.Invoke();
            pc = address;
            TickEvent?.Invoke();
        }

        public void CallConditional(ushort address, ref ushort sp, ref ushort pc, Flag flag, bool condition, byte flags)
        {
            if (FlagUtils.GetFlag(flag, flags) == condition)
            {
                Call(address, ref sp, ref pc);
            }
        }

        public void JumpRelative(byte value, ref ushort pc)
        {
            sbyte signed = unchecked((sbyte)value);
            pc = (ushort)(pc + signed);
            TickEvent?.Invoke();
        }

        public void JumpRelativeConditional(byte value, ref ushort pc, Flag flag, bool condition, byte flags)
        {
            if (FlagUtils.GetFlag(flag, flags) == condition)
            {
                JumpRelative(value, ref pc);
            }
        }

        public void JumpToAddress(ushort address, ref ushort pc)
        {
            pc = address;
            TickEvent?.Invoke();
        }

        public void JumpToAddress(byte addrHigh, byte addrLow, ref ushort pc)
        {
            ushort address = BitUtils.BytesToUshort(addrHigh, addrLow);
            //JumpToAddress(address, ref pc); //TODO Should not cause ticks?
            pc = address;
        }

        public void JumpToAddressConditional(ushort address, ref ushort pc, Flag flag, bool condition, byte flags)
        {
            if (FlagUtils.GetFlag(flag, flags) == condition)
            {
                JumpToAddress(address, ref pc);
            }
        }

        public void Return(ref ushort sp, ref ushort pc)
        {
            pc = memory.ReadWord(sp);
            TickEvent?.Invoke();
            TickEvent?.Invoke();
            sp = (ushort)(sp + 2);
            TickEvent?.Invoke();
        }

        public void ReturnAndEnableInterrupts(ref ushort sp, ref ushort pc, ref int enableAfter)
        {
            Return(ref sp, ref pc);
            enableAfter = 0;
        }

        public void ReturnConditional(ref ushort sp, ref ushort pc, Flag flag, bool condition, byte flags)
        {
            TickEvent?.Invoke();
            if (FlagUtils.GetFlag(flag, flags) == condition)
            {
                Return(ref sp, ref pc);
            }
        }
    }
}
