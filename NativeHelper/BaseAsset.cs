using System.IO;

namespace NativeHelper
{
    public abstract class BaseAsset
    {
        protected readonly Native Native;
        protected readonly long Pointer;

        protected BaseAsset(Native native, long pointer)
        {
            Native = native;
            Pointer = pointer;
        }

        public string Name
        {
            get
            {
                string path = Native.ReadString(Pointer);
                return Path.GetFileName(path);
            }
        }
        public abstract int Length { get; set; }

        public override string ToString()
        {
            return $"Name : {Name}, Length: {Length:X}";
        }
    }
}