﻿namespace GeemuBoy.GB.CpuUnits
{
    public class ALU
    {
        private readonly Memory memory;

        public event CPU.TickHandler? TickEvent;

        public ALU(Memory memory)
        {
            this.memory = memory;
        }

        public void Add(ref byte to, byte value, ref byte flags, bool addCarryFlag = false)
        {
            int additionalValue = addCarryFlag && FlagUtils.GetFlag(Flag.C, flags) ? 1 : 0;
            byte origValue = to;
            int newValue = to + value + additionalValue;
            to = (byte)(newValue & 0xFF);
            FlagUtils.SetFlags(ref flags,
                to == 0,
                false,
                (origValue & 0x0F) + (value & 0x0F) + additionalValue > 0x0F,
                newValue > 0xFF);
        }

        public void Add(ref byte toHigh, ref byte toLow, byte valueHigh, byte valueLow, ref byte flags)
        {
            ushort to = BitUtils.BytesToUshort(toHigh, toLow);
            ushort value = BitUtils.BytesToUshort(valueHigh, valueLow);
            ushort result = (ushort)((to + value) & 0xFFFF);
            toHigh = BitUtils.MostSignificantByte(result);
            toLow = BitUtils.LeastSignificantByte(result);
            // Zero flag is not affected
            FlagUtils.SetFlag(Flag.N, false, ref flags);
            FlagUtils.SetFlag(Flag.H, (to & 0xFFF) + (value & 0xFFF) > 0xFFF, ref flags);
            FlagUtils.SetFlag(Flag.C, to + value > 0xFFFF, ref flags);
            TickEvent?.Invoke();
        }

        public void AddSigned(ref ushort to, byte value, ref byte flags)
        {
            ushort originalValue = to;
            sbyte valueSigned = unchecked((sbyte)value);
            to = (ushort)(to + valueSigned);
            FlagUtils.SetFlag(Flag.Z, false, ref flags);
            FlagUtils.SetFlag(Flag.N, false, ref flags);
            FlagUtils.SetFlag(Flag.H, (originalValue & 0x0F) + (value & 0x0F) > 0x0F, ref flags);
            FlagUtils.SetFlag(Flag.C, (originalValue & 0xFF) + value > 0xFF, ref flags);
            TickEvent?.Invoke();
            TickEvent?.Invoke();
        }

        public void Subtract(ref byte from, byte value, ref byte flags, bool subtractCarryFlag = false)
        {
            int additionalValue = subtractCarryFlag && FlagUtils.GetFlag(Flag.C, flags) ? 1 : 0;
            byte origValue = from;
            int newValue = from - value - additionalValue;
            from = (byte)(newValue & 0xFF);
            FlagUtils.SetFlags(ref flags,
                from == 0,
                true,
                (origValue & 0x0F) - (value & 0xF) - (additionalValue & 0xF) < 0,
                origValue < (value + additionalValue));
        }

        public void And(ref byte to, byte value, ref byte flags)
        {
            to = (byte)(to & value);
            FlagUtils.SetFlags(ref flags,
                to == 0,
                false,
                true,
                false);
        }

        public void Or(ref byte to, byte value, ref byte flags)
        {
            to = (byte)(to | value);
            FlagUtils.SetFlags(
                ref flags,
                to == 0,
                false,
                false,
                false);
        }

        public void Xor(ref byte to, byte value, ref byte flags)
        {
            to = (byte)(to ^ value);
            FlagUtils.SetFlags(ref flags,
                to == 0,
                false,
                false,
                false);
        }

        public void Compare(byte to, byte value, ref byte flags)
        {
            byte result = (byte)((to - value) & 0xFF);
            FlagUtils.SetFlags(ref flags,
                result == 0,
                true,
                (to & 0x0F) < (value & 0x0F),
                to < value);
        }

        public void Increment(ref byte target, ref byte flags)
        {
            byte origValue = target;
            target++;
            FlagUtils.SetFlag(Flag.Z, target == 0, ref flags);
            FlagUtils.SetFlag(Flag.N, false, ref flags);
            FlagUtils.SetFlag(Flag.H, (origValue & 0x0F) + 1 > 0x0F, ref flags);
        }

        public void IncrementInMemory(byte addrHigh, byte addrLow, ref byte flags)
        {
            ushort address = BitUtils.BytesToUshort(addrHigh, addrLow);
            byte origValue = memory.ReadByte(address);
            TickEvent?.Invoke();
            memory.WriteByte(address, (byte)(origValue + 1));
            FlagUtils.SetFlag(Flag.Z, (byte)(origValue + 1) == 0, ref flags);
            FlagUtils.SetFlag(Flag.N, false, ref flags);
            FlagUtils.SetFlag(Flag.H, (origValue & 0x0F) + 1 > 0x0F, ref flags);
            TickEvent?.Invoke();
        }

        public void IncrementWord(ref byte targetHigh, ref byte targetLow)
        {
            ushort target = BitUtils.BytesToUshort(targetHigh, targetLow);
            target = (ushort)(target + 1);
            targetHigh = BitUtils.MostSignificantByte(target);
            targetLow = BitUtils.LeastSignificantByte(target);
            TickEvent?.Invoke();
        }

        public void IncrementWord(ref ushort target)
        {
            byte high = BitUtils.MostSignificantByte(target);
            byte low = BitUtils.LeastSignificantByte(target);
            IncrementWord(ref high, ref low);
            target = BitUtils.BytesToUshort(high, low);
        }

        public void Decrement(ref byte target, ref byte flags)
        {
            byte origValue = target;
            target--;
            FlagUtils.SetFlag(Flag.Z, target == 0, ref flags);
            FlagUtils.SetFlag(Flag.N, true, ref flags);
            FlagUtils.SetFlag(Flag.H, (origValue & 0x0F) < 1, ref flags);
        }

        public void DecrementInMemory(byte addrHigh, byte addrLow, ref byte flags)
        {
            ushort address = BitUtils.BytesToUshort(addrHigh, addrLow);
            byte origValue = memory.ReadByte(address);
            TickEvent?.Invoke();
            memory.WriteByte(address, (byte)(origValue - 1));
            FlagUtils.SetFlag(Flag.Z, (byte)(origValue - 1) == 0, ref flags);
            FlagUtils.SetFlag(Flag.N, true, ref flags);
            FlagUtils.SetFlag(Flag.H, (origValue & 0x0F) < 1, ref flags);
            TickEvent?.Invoke();
        }

        public void DecrementWord(ref byte targetHigh, ref byte targetLow)
        {
            ushort target = BitUtils.BytesToUshort(targetHigh, targetLow);
            target = (ushort)(target - 1);
            targetHigh = BitUtils.MostSignificantByte(target);
            targetLow = BitUtils.LeastSignificantByte(target);
            TickEvent?.Invoke();
        }

        public void DecrementWord(ref ushort target)
        {
            byte high = BitUtils.MostSignificantByte(target);
            byte low = BitUtils.LeastSignificantByte(target);
            DecrementWord(ref high, ref low);
            target = BitUtils.BytesToUshort(high, low);
        }
    }
}
