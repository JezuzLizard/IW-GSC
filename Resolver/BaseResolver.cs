using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Resolver.Properties;
using System.IO;

namespace Resolver
{
    /// <summary>
    ///     Resolver should inherit this class
    /// </summary>
   

    public abstract class BaseResolver
    {
        protected readonly bool Console;
        protected readonly Game Game;

        private readonly Dictionary<byte, Opcode> _opcodes;
        protected BaseResolver(bool console, Game game)
        {
            Console = console;
            Game = game;

            /*
            _opcodes = new Dictionary<byte, Opcode>();
            var opcodesContent = Encoding.ASCII.GetString(Resources.debug_opcodes);
            var opcodesIds = JsonConvert.DeserializeObject<byte[]>(opcodesContent);
            for (var index = 0; index < Enum.GetValues(typeof(Opcode)).Length; index++)
            {
                var value = Enum.GetValues(typeof(Opcode)).GetValue(index);
                _opcodes[opcodesIds[index]] = (Opcode)value;
            }

            string deserializedObject = JsonConvert.DeserializeObject<string>(File.ReadAllText(@"C:\Users\Justin\Desktop\Other Files\GSC.json"));
            string serializedObject = "";
            */
            using (StreamReader file = File.OpenText(@"C:\Users\Justin\Desktop\Other Files\GSC.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializedObject = (string)serializer.Deserialize(file, typeof(string));
            }
            System.Console.WriteLine(deserializedObject);
            System.Console.WriteLine(serializedObject);
        }

        public abstract byte ResolveIdOfOpcode(Opcode opcode);
        public abstract ushort ResolveIdOfMethod(string method);
        public abstract ushort ResolveIdOfFunction(string function);
        public abstract ushort ResolveIdOfField(string field);
        public abstract ushort ResolveIdOfString(string s);
        public abstract Opcode ResolveOpcodeById(byte value);
        public abstract string ResolveMethodNameById(ushort value);
        public abstract string ResolveFunctionNameById(ushort value);
        public abstract string ResolveFieldNameById(ushort value);
        public abstract string ResolveStringById(ushort value);
    }
}