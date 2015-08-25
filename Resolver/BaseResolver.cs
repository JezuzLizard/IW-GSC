namespace Resolver
{
    /// <summary>
    ///     Resolver should inherit this class
    /// </summary>
    public abstract class BaseResolver
    {
        protected readonly bool Console;
        protected readonly Game Game;

        protected BaseResolver(bool console, Game game)
        {
            Console = console;
            Game = game;
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