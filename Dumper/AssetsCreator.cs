using System;

namespace Dumper
{
    public class AssetsCreator
    {
        public static T CreateAsset<T>(Native native, long pointer) where T : BaseAsset
        {
            if (typeof(T) == typeof(ScriptFile))
            {
                return (T)(object)new ScriptFile(native, pointer);
            }
            throw new InvalidOperationException($"Type {typeof(T)} is not supported");
        }
    }
}