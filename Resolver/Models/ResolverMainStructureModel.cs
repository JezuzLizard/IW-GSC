using System.Collections.Generic;

namespace Resolver.Models
{
    public class ResolverMainStructureModel
    {
        public Dictionary<string, ushort> Functions;
        public Dictionary<string, ushort> Methods;
        public Dictionary<string, ushort> Fields;
    }
}