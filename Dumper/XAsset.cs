namespace Dumper
{
    public class XAsset<T> where T : BaseAsset, new()
    {
        private readonly int _pointer;

        public XAsset(int pointer)
        {
            _pointer = pointer;
            Asset = new T { Pointer = _pointer };
        }

        public XAssetType Type { get; }

        public T Asset { get; }
    }
}