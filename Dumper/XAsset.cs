namespace Dumper
{
    public class XAsset<T> where T : BaseAsset
    {
        private readonly long _pointer;
        private readonly Native _native;

        public XAsset(Native native, long pointer)
        {
            _native = native;
            _pointer = pointer;
            Asset = AssetsCreator.CreateAsset<T>(_native, _native.ReadLong(_pointer + 8));
        }

        public XAssetType Type => (XAssetType)_native.ReadInt(_pointer);

        public T Asset { get; }
    }
}