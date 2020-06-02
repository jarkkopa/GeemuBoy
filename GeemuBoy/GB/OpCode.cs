using System;

namespace GeemuBoy.GB
{
    public class OpCode
    {
        public Action Instruction { get; }
        public string Name { get; }

        public OpCode(Action instruction, string name)
        {
            Instruction = instruction;
            Name = name;
        }
    }
}
