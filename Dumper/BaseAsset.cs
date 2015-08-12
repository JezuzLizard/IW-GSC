using System.IO;

namespace Dumper
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
        
        public string Name => Path.GetFileName(Native.ReadString(Pointer));
        public abstract int Length { get; }

        public override string ToString()
        {
            return $"Name : {Name}, Length: {Length:X}";
        }
    }
}