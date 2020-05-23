using System;

namespace GeemuBoy.GB
{
    public class InputRegister
    {
        [Flags]
        public enum Keys
        {
            Down = 0x80,
            Up = 0x40,
            Left = 0x20,
            Right = 0x10,
            Start = 0x08,
            Select = 0x04,
            A = 0x02,
            B = 0x01,
            None = 0x0
        }

        public Keys CurrentState { get; set; } = Keys.None;

        public void UpdateState(Keys pressed, Memory memory)
        {
            Keys newPressed = (pressed ^ CurrentState) & pressed;
            CurrentState = pressed;
            if (newPressed != Keys.None)
            {
                CPU.RequestInterrupt(memory, CPU.Interrupt.Joypad);
            }
        }

        public byte ReadValue(byte oldRegister)
        {
            byte value = (byte)(oldRegister | 0xF);
            if (oldRegister.IsBitSet(5) == false)
            {
                BitUtils.SetBit(ref value, 3, !CurrentState.HasFlag(Keys.Start));
                BitUtils.SetBit(ref value, 2, !CurrentState.HasFlag(Keys.Select));
                BitUtils.SetBit(ref value, 1, !CurrentState.HasFlag(Keys.B));
                BitUtils.SetBit(ref value, 0, !CurrentState.HasFlag(Keys.A));
            }
            else if (oldRegister.IsBitSet(4) == false)
            {
                BitUtils.SetBit(ref value, 3, !CurrentState.HasFlag(Keys.Down));
                BitUtils.SetBit(ref value, 2, !CurrentState.HasFlag(Keys.Up));
                BitUtils.SetBit(ref value, 1, !CurrentState.HasFlag(Keys.Left));
                BitUtils.SetBit(ref value, 0, !CurrentState.HasFlag(Keys.Right));
            }

            return value;
        }
    }
}
