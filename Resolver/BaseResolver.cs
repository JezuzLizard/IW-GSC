using System.Text;
using System.Linq;
using Newtonsoft.Json;
using Resolver.Models;
using Resolver.Properties;
using System;

namespace Resolver
{
    /// <summary>
    ///     Resolver should inherit this class
    /// </summary>


    public class BaseResolver
    {
        protected readonly bool Console;
        protected readonly Game Game;

        private ResolverModel mainModel;
        private ResolverGamesModel systemModel;
        private ResolverMainStructureModel gameModel;

        public BaseResolver(bool console, Game game)
        {
            Console = console;
            Game = game;

            var text = Encoding.ASCII.GetString(Resources.j_main);
            mainModel = JsonConvert.DeserializeObject<ResolverModel>(text);

            systemModel = mainModel.PC;
            if (console)
                systemModel = mainModel.Console;

            switch(game)
            {
                case Game.Ghosts:
                    gameModel = systemModel.Ghosts;
                    break;
                case Game.MW3:
                    break;
                case Game.AW:
                    break;
            }
        }

        public byte ResolveIdOfOpcode(Opcode opcode)
        {
            return (byte)gameModel.OPCodes.FirstOrDefault(e => e.Key == Enum.GetName(typeof(Opcode), opcode)).Value;
        }

        public byte ResolveIdOfOpcodeString(string opcode)
        {
            return (byte)gameModel.OPCodes.FirstOrDefault(e => e.Key == opcode).Value;
        }

        public ushort ResolveIdOfMethod(string method)
        {
            return gameModel.Methods.FirstOrDefault(e => e.Key == method).Value;
        }

        public ushort ResolveIdOfFunction(string function)
        {
            return gameModel.Functions.FirstOrDefault(e => e.Key == function).Value;
        }

        public ushort ResolveIdOfField(string field)
        {
            return gameModel.Fields.FirstOrDefault(e => e.Key == field).Value;
        }

        public ushort ResolveIdOfString(string s)
        {
            return 0;
        }

        public Opcode ResolveOpcodeById(byte opcode)
        {
            Opcode result = 0;
            Enum.TryParse<Opcode>(ResolveOpcodeNameById(opcode), out result);
            return result;
        }

        public Opcode ResolveOpcodes()
        {
            Opcode result = 0;
            foreach (var opcode in gameModel.OPCodes)
            {
                Enum.TryParse<Opcode>(opcode.Key, out result);
            }
            return result;
        }

        public string ResolveOpcodeNameById(ushort value)
        {
            return gameModel.OPCodes.FirstOrDefault(e => e.Value == value).Key;
        }

        public string ResolveMethodNameById(ushort value)
        {
            return gameModel.Methods.FirstOrDefault(e => e.Value == value).Key;
        }
        public string ResolveFunctionNameById(ushort value)
        {
            return gameModel.Functions.FirstOrDefault(e => e.Value == value).Key;
        }
    
        public string ResolveFieldNameById(ushort value)
        {
            return gameModel.Fields.FirstOrDefault(e => e.Value == value).Key;
        }

        public string ResolveStringNamegById(ushort value)
        {
            return "Not Implemented";
        }
    }
}