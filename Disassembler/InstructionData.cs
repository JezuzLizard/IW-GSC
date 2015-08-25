using System.Collections.Generic;

namespace Disassembler
{
    public class InstructionData
    {
        public InstructionData()
        {
            Data = new List<object>();
        }

        public List<object> Data { get; }
        public string DataString { get; set; }

        public void AddData(object data)
        {
            Data.Add(data);
        }
    }
}