using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Compiler.Module.Properties;
using Ionic.Zlib;
using Irony.Parsing;
using Newtonsoft.Json;
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
            PrepareParseTree(_tree.Root);
            _functions = new List<ScriptFunction>();
            _resolver = resolver;
        }

        private void PrepareParseTree(ParseTreeNode node)
        {
            foreach (var childNode in node.ChildNodes)
            {
                if (childNode.Term.Name == IdentifierId)
                {
                    childNode.Token.Value = childNode.Token.ValueString.ToLower();
                }
                else
                {
                    PrepareParseTree(childNode);
                }
            }
        }

        private void Compile()
        {
            foreach (var childNode in _tree.Root.ChildNodes)
            {
                CompileInternal(childNode);
            }
        }

        public CompiledScriptFile CompileToAsset()
        {
            Compile();
            var buffer = new List<byte>();
            var byteCode = new List<byte>();
            foreach (var scriptFunction in _functions)
            {
                buffer.AddRange(scriptFunction.Buffer);
                byteCode.AddRange(scriptFunction.ByteCode);
            }
            var compiledFile = new CompiledScriptFile();
            var compressed = ZlibStream.CompressBuffer(buffer.ToArray());
            compiledFile.Buffer = compressed;
            compiledFile.ByteCode = byteCode.ToArray();
            compiledFile.ByteCodeLength = byteCode.Count;
            compiledFile.CompressedLength = compressed.Length;
            compiledFile.UncompressedLength = buffer.Count;
            return compiledFile;
        }

        public byte[] CompileToByteArray()
        {
            Compile();
            var buffer = new List<byte>();
            var byteCode = new List<byte>();
            foreach (var scriptFunction in _functions)
            {
                buffer.AddRange(scriptFunction.Buffer);
                byteCode.AddRange(scriptFunction.ByteCode);
            }
            var result = new List<byte>();
#if !DEBUG
            var compressed = ZlibStream.CompressBuffer(buffer.ToArray());
#else
            var compressed = buffer.ToArray();
#endif
            result.AddRange(BitConverter.GetBytes(compressed.Length));
            result.AddRange(BitConverter.GetBytes(buffer.Count));
            result.AddRange(BitConverter.GetBytes(byteCode.Count));
            result.AddRange(compressed);
            result.AddRange(byteCode);
            return result.ToArray();
        }

        private void CompileInternal(ParseTreeNode node)
        {
            if (node == null)
                return;
            switch (node.Term.Name)
            {
                case FunctionId:
                    CreateFunction(node);
                    break;

                case LinesId:
                    foreach (var line in node.ChildNodes)
                    {
                        CompileInternal(line);
                    }
                    break;

                case SimpleCallId:
                    EmitCall(node.ChildNodes.FirstOrDefault()?.ChildNodes.FirstOrDefault(), true);
                    break;

                case CallId:
                    EmitCall(node.ChildNodes.FirstOrDefault(), false);
                    break;

                case ParametersId:
                    node.ChildNodes.Reverse();
                    foreach (var childNode in node.ChildNodes)
                    {
                        CompileInternal(childNode);
                    }
                    break;

                case NumberId:
                    var value = node.Token.Value;
                    if (value is int)
                    {
                        EmitGetInt((int) value);
                    }
                    else if (value is double)
                    {
                        EmitGetFloat((float) (double) value);
                    }
                    break;

                case StringId:
                    EmitGetString(node.Token.ValueString);
                    break;
            }
        }

        private void EmitGetString(string s)
        {
            AddDataMember(s);
            AddOpcode(Opcode.OpGetString);
            AddByteCode(new byte[4]);
        }

        private void EmitGetFloat(float value)
        {
            AddOpcode(Opcode.OpGetFloat);
            AddByteCode(BitConverter.GetBytes(value));
        }

        private void EmitGetInt(int value)
        {
            AddOpcode(Opcode.OpGetInteger);
            AddByteCode(BitConverter.GetBytes(value));
        }

        private ParseTreeNode FindParametersNode(ParseTreeNode node)
        {
            if (node.Term.Name == MethodCallId)
            {
                node = node.ChildNodes[1];
            }
            return node.ChildNodes.Find(e => e.Term.Name == ParametersId);
        }

        private string FindFunctionName(ParseTreeNode node)
        {
            if (node.Term.Name == MethodCallId)
            {
                node = node.ChildNodes[1];
            }
            return node.ChildNodes.Find(e => e.Term.Name == IdentifierId)?.Token.ValueString;
        }

        private void EmitBuiltInFunctionCall(ParseTreeNode node, bool decTop)
        {
            var functionName = FindFunctionName(node);
            var parametersNode = FindParametersNode(node);
            CompileInternal(parametersNode);
            switch (functionName)
            {
                case "wait":
                    AddOpcode(Opcode.OpWait);
                    return;

                default:
                    AddOpcode(Opcode.OpCallBuiltin);
                    AddByteCode((byte)parametersNode.ChildNodes.Count);
                    AddId(_resolver.ResolveIdOfFunction(functionName));
                    if (decTop)
                    {
                        AddOpcode(Opcode.OpDecTop);
                    }
                    break;
            }
        }

        private void EmitBuiltInMethodCall(ParseTreeNode node, bool decTop)
        {
            var functionName = FindFunctionName(node);
            var parametersNode = FindParametersNode(node);
            CompileInternal(parametersNode);
            switch (functionName)
            {
                default:
                    EmitOwner(node.ChildNodes[0]);
                    AddOpcode(Opcode.OpCallBuiltinMethod);
                    AddByteCode((byte)parametersNode.ChildNodes.Count);
                    AddId(_resolver.ResolveIdOfMethod(functionName));
                    if (decTop)
                    {
                        AddOpcode(Opcode.OpDecTop);
                    }
                    break;
            }
        }

        private void EmitOwner(ParseTreeNode node)
        {
            if (node.Token == null)
            {
                CompileInternal(node);
                return;
            }
            switch (node.Token.ValueString)
            {
                case "game":
                    AddOpcode(Opcode.OpGetGame);
                    break;

                case "self":
                    AddOpcode(Opcode.OpGetSelf);
                    break;

                case "level":
                    AddOpcode(Opcode.OpGetLevel);
                    break;

                case "anim":
                    AddOpcode(Opcode.OpGetAnim);
                    break;

                default:
                    CompileInternal(node);
                    break;
            }
        }

        private void EmitCall(ParseTreeNode node, bool decTop)
        {
            var functionName = FindFunctionName(node);
            if (node.Term.Name == FunctionCallId && IsBuiltInFunction(functionName))
            {
                EmitBuiltInFunctionCall(node, decTop);
                return;
            }
            if (node.Term.Name == MethodCallId && IsBuiltInMethod(functionName))
            {
                EmitBuiltInMethodCall(node, decTop);
                return;
            }
            
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

        private bool IsBuiltInMethod(string functionName)
        {
            ushort id = _resolver.ResolveIdOfMethod(functionName);
            return id != 0 || functionName == "waittill" || functionName == "notify" || functionName == "endon";
        }

        private bool IsBuiltInFunction(string functionName)
        {
            ushort id = _resolver.ResolveIdOfFunction(functionName);
            return id != 0 || functionName == "wait" || functionName == "waittillframeend";
        }

        private void CreateFunction(ParseTreeNode node)
        {
            var name = node.FindTokenAndGetString();
            var scriptFunction = new ScriptFunction {FunctionName = name, FunctionId = _resolver.ResolveIdOfString(name)};
            _functions.Add(scriptFunction);
            var parameters = FindParametersNode(node)?.ChildNodes
                .Select(e => e.Token.ValueString)
                .ToList();
            //TODO add support for parameters and local variables
            if (parameters != null)
            {
                for (byte index = 0; index < parameters.Count; index++)
                {
                    var parameter = parameters[index];
                    AddOpcode(Opcode.OpSafeCreateVariableFieldCached);
                    AddByteCode(index);
                }
            }
            AddOpcode(Opcode.OpCheckclearparams);
            CompileInternal(node.ChildNodes.Find(e => e.Term.Name == LinesId));
            AddOpcode(Opcode.OpEnd);
        }

        private void AddOpcode(Opcode opcode)
        {
            AddByteCode(_resolver.ResolveIdOfOpcode(opcode));
        }

        private void AddId(ushort id)
        {
            AddByteCode(BitConverter.GetBytes(id));
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