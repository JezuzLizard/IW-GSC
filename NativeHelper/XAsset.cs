using System;

namespace NativeHelper
{
    public class XAsset<T> where T : BaseAsset
    {
        private readonly Native _native;
        private readonly long _pointer;

        public XAsset(Native native, long pointer)
        {
            _native = native;
            _pointer = pointer;
            Asset = CreateAsset();
        }

        public XAssetType Type => (XAssetType) _native.ReadInt(_pointer);
        public T Asset { get; }

        private T CreateAsset()
        {
            if (typeof (T) == typeof (ScriptFile))
            {
                return (T) (object) new ScriptFile(_native, _native.ReadLong(_pointer + 8));
            }
            throw new InvalidOperationException($"Type {typeof (T)} is not supported");
        }
    }
}