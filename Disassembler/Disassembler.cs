using System;
using System.IO;
using Resolver;

namespace Disassembler
{
    public class Disassembler
    {
        private readonly byte[] _buffer;
        private readonly byte[] _bytecode;
        private readonly BaseResolver _resolver;

        /// <summary>
        ///     Creates instance of disassembler
        /// </summary>
        /// <param name="resolver">Resolver</param>
        /// <param name="path">path to target file</param>
        public Disassembler(BaseResolver resolver, string fileName)
        {
            _resolver = resolver;
            var path = Path.Combine(Path.GetDirectoryName(fileName), Path.GetFileNameWithoutExtension(fileName));
            var bytecodePath = path + ".bytecode";
            var bufferPath = path + ".buffer";
            if (!File.Exists(bytecodePath))
            {
                throw new ArgumentOutOfRangeException("File with script bytecode is not exists!");
            }
            if (!File.Exists(bufferPath))
            {
                throw new ArgumentOutOfRangeException("File with script buffer is not exists!");
            }
            _bytecode = File.ReadAllBytes(bytecodePath);
            _buffer = File.ReadAllBytes(bufferPath);
        }
    }
}