using System;
using Resolver;

namespace Compiler.Console
{
    public class FakeResolver : BaseResolver
    {
        public FakeResolver(bool console, Game game) : base(console, game)
        {
        }

        public override byte ResolveValueForOpcode(Opcode opcode)
        {
            throw new NotImplementedException();
        }

        public override ushort ResolveValueForMethod(string method)
        {
            throw new NotImplementedException();
        }

        public override ushort ResolveValueForFunction(string function)
        {
            throw new NotImplementedException();
        }

        public override ushort ResolveValueForField(string field)
        {
            throw new NotImplementedException();
        }

        public override Opcode ResolveOpcodeForValue(byte value)
        {
            throw new NotImplementedException();
        }

        public override string ResolveMethodNameForValue(ushort value)
        {
            throw new NotImplementedException();
        }

        public override string ResolveFunctionNameForValue(ushort value)
        {
            throw new NotImplementedException();
        }

        public override string ResolveFieldNameForValue(ushort value)
        {
            throw new NotImplementedException();
        }
    }
}