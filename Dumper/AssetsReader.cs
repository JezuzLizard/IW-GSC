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
            for (int i = 0; i < 4096; i++)
            {
                var xAsset = new XAsset<T>(i);
                if (xAsset.Type == _type)
                {
                    assets.Add(xAsset.Asset);
                }
            }
            return assets;
        }
    }
}