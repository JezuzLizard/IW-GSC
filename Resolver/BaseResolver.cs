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

        public abstract byte ResolveValueForOpcode(Opcode opcode);
        public abstract ushort ResolveValueForMethod(string method);
        public abstract ushort ResolveValueForFunction(string function);
        public abstract ushort ResolveValueForField(string field);
        public abstract ushort ResolveValueForString(string s);
        public abstract Opcode ResolveOpcodeForValue(byte value);
        public abstract string ResolveMethodNameForValue(ushort value);
        public abstract string ResolveFunctionNameForValue(ushort value);
        public abstract string ResolveFieldNameForValue(ushort value);
    }
}