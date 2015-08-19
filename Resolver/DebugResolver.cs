using System;

namespace Resolver
{
    public class DebugResolver : BaseResolver
    {
        public DebugResolver(bool console, Game game) : base(console, game)
        {
        }

        public override byte ResolveIdOfOpcode(Opcode opcode)
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
                case Opcode.OpGetInteger:
                    return 0x80;
                case Opcode.OpGetFloat:
                    return 0x2B;
                case Opcode.OpGetString:
                    return 0x53;
                case Opcode.OpWait:
                    return 0x79;
                case Opcode.OpCallBuiltin:
                    return 0x20;
                case Opcode.OpSafeCreateVariableFieldCached:
                    return 0x2C;
                case Opcode.OpCallBuiltin0:
                    return 0x1A;
                case Opcode.OpCallBuiltin1:
                    return 0x1B;
                case Opcode.OpCallBuiltin2:
                    return 0x1C;
                case Opcode.OpCallBuiltin3:
                    return 0x1D;
                case Opcode.OpGetSelf:
                    return 0x41;
                case Opcode.OpGetAnim:
                    return 0x89;
                case Opcode.OpGetLevel:
                    return 0x67;
                case Opcode.OpGetGame:
                    return 0x84;
                case Opcode.OpCallBuiltinMethod:
                    return 0x99;
                default:
                    throw new ArgumentOutOfRangeException(nameof(opcode), opcode, null);
            }
        }

        public override ushort ResolveIdOfMethod(string method)
        {
            switch (method)
            {
                case "iprintln":
                    return 0x8263;

                case "iprintlnbold":
                    return 0x8264;

                default:
                    return 0;
            }
        }

        public override ushort ResolveIdOfFunction(string function)
        {
            switch (function)
            {
                case "iprintln":
                    return 0x185;

                case "iprintlnbold":
                    return 0x186;

                case "setdvar":
                    return 0x32;

                case "max":
                    return 0xD5;

                default:
                    return 0;
            }
        }

        public override ushort ResolveIdOfField(string field)
        {
            throw new NotImplementedException();
        }

        public override ushort ResolveIdOfString(string s)
        {
            switch (s)
            {
                case "main":
                    return 0x4FDD;
                case "create_vision_set_fog":
                    return 0x20C0;
                default:
                    return 0;
            }
        }

        public override Opcode ResolveOpcodeById(byte value)
        {
            throw new NotImplementedException();
        }

        public override string ResolveMethodNameById(ushort value)
        {
            throw new NotImplementedException();
        }

        public override string ResolveFunctionNameById(ushort value)
        {
            throw new NotImplementedException();
        }

        public override string ResolveFieldNameById(ushort value)
        {
            throw new NotImplementedException();
        }
    }
}