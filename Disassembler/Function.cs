using System.Collections.Generic;
using System.Text;

namespace Disassembler
{
    public class Function
    {
        public string Name { get; set; }
        public List<Instruction> Instructions { get; set; }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendLine($"Function: {Name}");
            foreach (var instruction in Instructions)
            {
                builder.AppendLine(instruction.ToString());
            }
            builder.AppendLine();
            return builder.ToString();
        }
    }
}