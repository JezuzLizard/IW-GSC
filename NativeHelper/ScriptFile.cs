using Ionic.Zlib;

namespace NativeHelper
{
    public class ScriptFile : BaseAsset
    {
        public ScriptFile(Native native, long pointer) : base(native, pointer)
        {
        }

        public int CompressedLength
        {
            get { return Native.ReadInt(Pointer + 8); }
            set { Native.WriteInt(Pointer + 8, value);}
        }


        public override int Length
        {
            get { return Native.ReadInt(Pointer + 0xC); }
            set { Native.WriteInt(Pointer + 0xC, value); }
        }

        public int ByteCodeLength
        {
            get { return Native.ReadInt(Pointer + 0x10); }
            set { Native.WriteInt(Pointer + 0x10, value); }
        }

        public byte[] Buffer
        {
            get
            {
                var pointer = Native.ReadLong(Pointer + 0x18);
                return ZlibStream.UncompressBuffer(Native.Read(pointer, CompressedLength));
            }
            set
            {
                //var pointer = Native.Malloc(value.Length);
                var pointer = Native.ReadLong(Pointer + 0x18);
                Native.Write(pointer, value);
                //Native.WriteLong(Pointer + 0x18, pointer);
            }
        }

        public byte[] ByteCode
        {
            get
            {
                var pointer = Native.ReadLong(Pointer + 0x20);
                return Native.Read(pointer, ByteCodeLength);
            }
            set
            {
                //var pointer = Native.Malloc(value.Length);
                var pointer = Native.ReadLong(Pointer + 0x20);
                Native.Write(pointer, value);
                //Native.WriteLong(Pointer + 0x20, pointer);
            }
        }
    }
}