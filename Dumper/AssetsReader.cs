using System.Collections.Generic;

namespace Dumper
{
    public class AssetsReader<T> where T : BaseAsset
    {
        private readonly Native _native;
        private readonly XAssetType _type;

        public AssetsReader(XAssetType type, Native native)
        {
            _type = type;
            _native = native;
        }

        public List<T> ReadAssets()
        {
            var assets = new List<T>();
            for (var i = 0; i < 73727; i++)
            {
                var xAsset = new XAsset<T>(_native, _native.AssetsPool + i*16);
                if (xAsset.Type == _type)
                {
                    assets.Add(xAsset.Asset);
                }
            }
            return assets;
        }
    }
}