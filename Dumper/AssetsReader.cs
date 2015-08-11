using System.Collections.Generic;

namespace Dumper
{
    public class AssetsReader<T> where T : BaseAsset, new()
    {
        private readonly XAssetType _type;

        public AssetsReader(XAssetType type)
        {
            _type = type;
        }

        public List<T> ReadAssets()
        {
            var assets = new List<T>();
            for (var i = 0; i < 73727; i++)
            {
                var xAsset = new XAsset<T>(Offsets.AssetsPool + i*16);
                if (xAsset.Type == _type)
                {
                    assets.Add(xAsset.Asset);
                }
            }
            return assets;
        }
    }
}