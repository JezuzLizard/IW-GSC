namespace Dumper
{
    public abstract class BaseAsset
    {
        public long Pointer { get; set; }

        public string Name
        {
            get { return Memory.ReadString(Pointer); }
        }

        public abstract int Length { get; }
    }
}