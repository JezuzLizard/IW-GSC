using System;
using System.IO;
using Irony.Parsing;

namespace Compiler.Module
{
    public class ScriptCompiler
    {
        private readonly ParseTree _tree;

        public ScriptCompiler(string path)
        {
            string source = File.ReadAllText(path);
            var grammar = new ScriptGrammar();
            var parser = new Parser(grammar);
            _tree = parser.Parse(source);
        }

        public byte[] Compile()
        {
            return new byte[1];
        }
    }
}