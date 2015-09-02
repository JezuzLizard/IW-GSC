using System.Collections.Generic;

namespace Resolver.Models
{
    public class ResolverMainStructureModel
    {
        public Dictionary<string, ushort> Functions;
        public Dictionary<string, ushort> Methods;
        public Dictionary<string, ushort> Fields;
        public Dictionary<string, ushort> Strings;
        public Dictionary<string, ushort> OPCodes;
    }
}