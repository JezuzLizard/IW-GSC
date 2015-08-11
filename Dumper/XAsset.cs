namespace Dumper
{
    public class XAsset<T> where T : BaseAsset, new()
    {
        private readonly long _pointer;

        public XAsset(long pointer)
        {
            _pointer = pointer;
            Asset = new T { Pointer = Memory.ReadLong(_pointer + 8) };
        }

        public XAssetType Type
        {
            get
            {
                long result = Memory.ReadInt(_pointer);
                return (XAssetType) result;
            }
        }

        public T Asset { get; }
    }
}