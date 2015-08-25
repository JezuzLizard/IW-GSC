using System.Collections.Generic;
using System.Text;
using Resolver;

namespace Disassembler
{
    public class Instruction
    {
        public Instruction()
        {
            Data = new List<object>();
        }

        public int Index { get; set; }
        public Opcode Opcode { get; set; }
        public List<object> Data { get; }

        public void AddData(object data)
        {
            Data.Add(data);
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append($"Index: 0x{Index:X}, Opcode: {Opcode}");
            switch (Opcode)
            {
            }
            return builder.ToString();
        }
    }
}