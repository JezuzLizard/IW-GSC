using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Resolver.Models;
using Resolver.Properties;

namespace Resolver
{
    public class ResolverImpl : BaseResolver
    {
        private ResolverModel model;

        public ResolverImpl(bool console, Game game) : base(console, game)
        {
            var text = Encoding.ASCII.GetString(Resources.j_main);
            model = JsonConvert.DeserializeObject<ResolverModel>(text);
        }

        public override byte ResolveIdOfOpcode(Opcode opcode)
        {
            throw new System.NotImplementedException();
        }

        public override ushort ResolveIdOfMethod(string method)
        {
            throw new System.NotImplementedException();
        }

        public override ushort ResolveIdOfFunction(string function)
        {
            throw new System.NotImplementedException();
        }

        public override ushort ResolveIdOfField(string field)
        {
            throw new System.NotImplementedException();
        }

        public override ushort ResolveIdOfString(string s)
        {
            throw new System.NotImplementedException();
        }

        public override Opcode ResolveOpcodeById(byte value)
        {
            throw new System.NotImplementedException();
        }

        public override string ResolveMethodNameById(ushort value)
        {
            throw new System.NotImplementedException();
        }

        public override string ResolveFunctionNameById(ushort value)
        {
            throw new System.NotImplementedException();
        }

        public override string ResolveFieldNameById(ushort value)
        {
            throw new System.NotImplementedException();
        }

        public override string ResolveStringById(ushort value)
        {
            throw new System.NotImplementedException();
        }
    }
}