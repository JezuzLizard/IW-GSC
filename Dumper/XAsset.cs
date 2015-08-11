namespace Dumper
{
    public class XAsset<T> where T : BaseAsset, new()
    {
        private readonly long _pointer;

        public XAsset(long pointer)
        {
            _pointer = pointer;
            Asset = new T {Pointer = Memory.ReadLong(_pointer + 8)};
        }

        public XAssetType Type => (XAssetType) Memory.ReadInt(_pointer);

        public T Asset { get; }
    }
}