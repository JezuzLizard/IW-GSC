﻿using System;
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
                default:
                    throw new ArgumentOutOfRangeException(nameof(opcode), opcode, null);
            }
        }

        public override ushort ResolveValueForMethod(string method)
        {
            switch (method)
            {
                case "iprintln":
                    return 0x8263;

                default:
                    throw new ArgumentOutOfRangeException(nameof(method), method, null);
            }
        }

        public override ushort ResolveValueForFunction(string function)
        {
            switch (function)
            {
                case "iprintln":
                    return 0x186;

                default:
                    throw new ArgumentOutOfRangeException(nameof(function), function, null);
            }
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