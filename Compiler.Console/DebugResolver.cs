using System;
using Resolver;

namespace Compiler.Console
{
    public class DebugResolver : BaseResolver
    {
        public DebugResolver(bool console, Game game) : base(console, game)
        {
        }

        public override byte ResolveValueForOpcode(Opcode opcode)
        {
            switch (opcode)
            {
                case Opcode.OpEnd:
                    return 0x34;
                case Opcode.OpCheckclearparams:
                    return 0x32;
                case Opcode.OpPreScriptCall:
                    return 0x88;
                case Opcode.OpDecTop:
                    return 0x69;
                default:
                    throw new ArgumentOutOfRangeException(nameof(opcode), opcode, null);
            }
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