using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Resolver.Properties;

namespace Resolver
{
    public class DebugResolver : BaseResolver
    {
        private readonly Dictionary<byte, Opcode> _opcodes;

        public DebugResolver(bool console, Game game) : base(console, game)
        {
            _opcodes = new Dictionary<byte, Opcode>();
            var opcodesContent = Encoding.ASCII.GetString(Resources.debug_opcodes);
            var opcodesIds = JsonConvert.DeserializeObject<byte[]>(opcodesContent);
            for (var index = 0; index < Enum.GetValues(typeof (Opcode)).Length; index++)
            {
                var value = Enum.GetValues(typeof (Opcode)).GetValue(index);
                _opcodes[opcodesIds[index]] = (Opcode) value;
            }
        }

        public override byte ResolveIdOfOpcode(Opcode opcode)
        {
            return _opcodes.FirstOrDefault(e => e.Value == opcode).Key;
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
            return _opcodes[value];
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

        public override string ResolveStringById(ushort value)
        {
            return value.ToString();
        }
    }
}