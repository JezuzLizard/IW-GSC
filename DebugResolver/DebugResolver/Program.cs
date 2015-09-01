using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Resolver;


namespace DebugResolver
{
    class Program
    {
        public class Target
        {
            public string fields;
        }

        //private readonly Dictionary<byte, Opcode> _opcodes;
        static void BaseResolver(bool console, int game)
        {
            /*
            _opcodes = new Dictionary<byte, Opcode>();
            var opcodesContent = Encoding.ASCII.GetString(Resources.debug_opcodes);
            var opcodesIds = JsonConvert.DeserializeObject<byte[]>(opcodesContent);
            for (var index = 0; index < Enum.GetValues(typeof(Opcode)).Length; index++)
            {
                var value = Enum.GetValues(typeof(Opcode)).GetValue(index);
                _opcodes[opcodesIds[index]] = (Opcode)value;
            }*/


            //string json = File.ReadAllText(@"C:\Users\Justin\Desktop\Other Files\GSC.json");
            //Target newTarget = JsonConvert.DeserializeObject<Target>(json);
            //Console.Write(newTarget);
            
        }

        static void Main(string[] args)
        {
            //BaseResolver(false, 0);
            //Console.ReadKey();
            var resolver = new ResolverImpl(false, Game.Ghosts);

        }
    }
}
