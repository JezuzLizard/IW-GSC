using System.Collections.Generic;
using System.IO;
using System.Linq;
using Irony.Parsing;
using static Compiler.Module.ScriptGrammarConst;

namespace Compiler.Module
{
    public class ScriptCompiler
    {
        private readonly List<ScriptFunction> _functions;
        private readonly ParseTree _tree;

        public ScriptCompiler(string path)
        {
            var source = File.ReadAllText(path);
            var grammar = new ScriptGrammar();
            var parser = new Parser(grammar);
            _tree = parser.Parse(source);
            _functions = new List<ScriptFunction>();
        }

        public byte[] Compile()
        {
            CompileInternal(_tree.Root);
            return new byte[1];
        }

        private void CompileInternal(ParseTreeNode node)
        {
            if (node == null)
                return;
            foreach (var childNode in node.ChildNodes)
            {
                switch (childNode.Term.Name)
                {
                    case FunctionId:
                        CreateFunction(childNode);
                        break;

                    case LinesId:
                        foreach (var line in childNode.ChildNodes)
                        {
                            CompileInternal(line);
                        }
                        break;

                    case SimpleCallId:
                        EmitCall(node.ChildNodes.FirstOrDefault(), true);
                        break;

                    case CallId:
                        EmitCall(node, false);
                        break;
                }
            }
        }

        private void EmitCall(ParseTreeNode node, bool decTop)
        {
            if (node == null)
                return;
            switch (node.Term.Name)
            {
                case FunctionCallId:
                    break;

                case FunctionThreadCallId:
                    break;

                case MethodCallId:
                    break;

                case MethodThreadCallId:
                    break;
            }
        }

        private void CreateFunction(ParseTreeNode node)
        {
            var name = node.FindTokenAndGetString();
            var scriptFunction = new ScriptFunction {FunctionName = name};
            _functions.Add(scriptFunction);
            var parameters =
                node.ChildNodes.FindAll(e => e.Term.Name == IdentifierId).Select(e => e.Token.ValueString.ToLower());
            foreach (var parameter in parameters)
            {
                //TODO support for parameters
            }
            CompileInternal(node.ChildNodes.Find(e => e.Term.Name == LinesId));
            AddByteCode(Opcodes.OpEnd);
        }

        private void AddByteCode(params byte[] bytecode)
        {
            _functions.LastOrDefault()?.AddByteCode(bytecode);
        }

        private void AddDataMember(object member)
        {
            _functions.LastOrDefault()?.AddDataMember(member);
        }
    }
}