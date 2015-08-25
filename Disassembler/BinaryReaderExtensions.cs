using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Disassembler
{
    internal static class BinaryReaderExtensions
    {
        public static string ReadTerminatedString(this BinaryReader reader)
        {
            var bytes = new List<byte>();
            byte b = 0;
            do
            {
                b = reader.ReadByte();
                bytes.Add(b);
            } while (b != 0);
            return Encoding.ASCII.GetString(bytes.Where(e => e != 0).ToArray());
        }
    }
}