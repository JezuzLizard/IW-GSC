using System.Collections.Generic;
using System.IO;
using System.Linq;
using Irony.Parsing;
using Resolver;
using static Compiler.Module.ScriptGrammarConst;

namespace Compiler.Module
{
    public class ScriptCompiler
    {
        private readonly List<ScriptFunction> _functions;
        private readonly BaseResolver _resolver;
        private readonly ParseTree _tree;

        public ScriptCompiler(string path, BaseResolver resolver)
        {
            var source = File.ReadAllText(path);
            var grammar = new ScriptGrammar();
            var parser = new Parser(grammar);
            _tree = parser.Parse(source);
            _functions = new List<ScriptFunction>();
            _resolver = resolver;
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
            if (decTop)
            {
                AddOpcode(Opcode.OpDecTop);
            }
        }

        private void CreateFunction(ParseTreeNode node)
        {
            var name = node.FindTokenAndGetString();
            var scriptFunction = new ScriptFunction {FunctionName = name};
            _functions.Add(scriptFunction);
            var parameters =
                node.ChildNodes.FindAll(e => e.Term.Name == IdentifierId)
                    .Select(e => e.Token.ValueString.ToLower())
                    .ToList();

            AddOpcode(Opcode.OpEnd);
            foreach (var parameter in parameters)
            {
                //TODO support for parameters
            }
            if (!parameters.Any())
            {
                AddOpcode(Opcode.OpCheckclearparams);
            }
            CompileInternal(node.ChildNodes.Find(e => e.Term.Name == LinesId));
            AddOpcode(Opcode.OpEnd);
        }

        private void AddOpcode(Opcode opcode)
        {
            AddByteCode(_resolver.ResolveValueForOpcode(opcode));
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