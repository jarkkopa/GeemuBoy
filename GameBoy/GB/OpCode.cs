using System;

namespace GameBoy.GB
{
    public class OpCode
    {
        public Action Instruction { get; }
        public int Cycles { get; }
        public String Name { get; }

        public OpCode(Action instruction, int cycles, String name)
        {
            Instruction = instruction;
            Cycles = cycles;
            Name = name;
        }
    }
}
