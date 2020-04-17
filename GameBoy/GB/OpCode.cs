using System;

namespace GeemuBoy.GB
{
    public class OpCode
    {
        public Func<int> Instruction { get; }
        public String Name { get; }

        public OpCode(Func<int> instruction, String name)
        {
            Instruction = instruction;
            Name = name;
        }
    }
}
