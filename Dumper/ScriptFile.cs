namespace Dumper
{
    public class ScriptFile : BaseAsset
    {
        public int CompressedLength => Memory.ReadInt(Pointer + 8);

        public override int Length => Memory.ReadInt(Pointer + 0xC);

        public int ByteCodeLength => Memory.ReadInt(Pointer + 0x10);

        public byte[] Buffer
        {
            get
            {
                long pointer = Memory.ReadLong(Pointer + 0x18);
                return Memory.Read(pointer, CompressedLength);
            }
        }

        public byte[] ByteCode
        {
            get
            {
                long pointer = Memory.ReadLong(Pointer + 0x20);
                return Memory.Read(pointer, ByteCodeLength);
            }
        }
    }
}