namespace Compiler.Module
{
    public class CompiledScriptFile
    {
        public int UncompressedLength { get; set; }
        public int CompressedLength { get; set; }
        public int ByteCodeLength { get; set; }
        public byte[] ByteCode { get; set; }
        public byte[] Buffer { get; set; }
    }
}