using Xunit;

namespace GeemuBoy.GB.Tests
{
    public class InputRegisterTests
    {
        [Fact()]
        public void UpdateStateRequestsInterruptWhenButtonPressed()
        {
            var inputRegister = new InputRegister();
            var memory = new Memory();

            inputRegister.UpdateState(InputRegister.Keys.Down, memory);

            Assert.Equal(0x10, memory.ReadByte(0xFF0F));
            Assert.Equal(InputRegister.Keys.Down, inputRegister.CurrentState);
        }

        [Fact()]
        public void UpdateStateRequestsInterruptWhenButtonPressedAndReleased()
        {
            var inputRegister = new InputRegister();
            var memory = new Memory();
            inputRegister.CurrentState = InputRegister.Keys.Up;

            inputRegister.UpdateState(InputRegister.Keys.Left, memory);

            Assert.Equal(0x10, memory.ReadByte(0xFF0F));
            Assert.Equal(InputRegister.Keys.Left, inputRegister.CurrentState);
        }

        [Fact()]
        public void UpdateStateDoesNotRequestsInterruptWhenButtonStillPressed()
        {
            var inputRegister = new InputRegister();
            var memory = new Memory();
            inputRegister.CurrentState = InputRegister.Keys.A;

            inputRegister.UpdateState(InputRegister.Keys.A, memory);

            Assert.Equal(0x0, memory.ReadByte(0xFF0F));
            Assert.Equal(InputRegister.Keys.A, inputRegister.CurrentState);
        }

        [Fact()]
        public void UpdateStateDoesNotRequestsInterruptWhenButtonReleased()
        {
            var inputRegister = new InputRegister();
            var memory = new Memory();
            inputRegister.CurrentState = InputRegister.Keys.A | InputRegister.Keys.Start;

            inputRegister.UpdateState(InputRegister.Keys.None, memory);

            Assert.Equal(0x0, memory.ReadByte(0xFF0F));
            Assert.Equal(InputRegister.Keys.None, inputRegister.CurrentState);
        }
    }
}