using Ionic.Zlib;

namespace Dumper
{
    public class ScriptFile : BaseAsset
    {
        public ScriptFile(Native native, long pointer) : base(native, pointer)
        {
        }

        public int CompressedLength => Native.ReadInt(Pointer + 8);
        public override int Length => Native.ReadInt(Pointer + 0xC);
        public int ByteCodeLength => Native.ReadInt(Pointer + 0x10);

        public byte[] Buffer
        {
            get
            {
                var pointer = Native.ReadLong(Pointer + 0x18);
                return ZlibStream.UncompressBuffer(Native.Read(pointer, CompressedLength));
            }
        }

        public byte[] ByteCode
        {
            get
            {
                var pointer = Native.ReadLong(Pointer + 0x20);
                return Native.Read(pointer, ByteCodeLength);
            }
        }
    }
}