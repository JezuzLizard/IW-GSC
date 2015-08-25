using System.Collections.Generic;
using System.Text;
using Resolver;

namespace Disassembler
{
    public class Instruction
    {

        public int Index { get; set; }
        public Opcode Opcode { get; set; }
        public InstructionData Data { get; set; }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append($"Index: 0x{Index:X}, Opcode: {Opcode}");
            if (!string.IsNullOrEmpty(Data?.DataString))
            {
                builder.Append($", {Data.DataString}");
            }
            return builder.ToString();
        }
    }
}