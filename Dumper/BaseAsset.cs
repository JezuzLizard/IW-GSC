using System.IO;

namespace Dumper
{
    public abstract class BaseAsset
    {
        public long Pointer { get; set; }
        public string Name => Path.GetFileName(Memory.ReadString(Pointer));
        public abstract int Length { get; }

        public override string ToString()
        {
            return $"Name : {Name}, Length: {Length:X}";
        }
    }
}