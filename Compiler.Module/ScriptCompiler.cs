﻿using System;
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
        private readonly string[] _builtIn;
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
            var builtIn = Encoding.ASCII.GetString(Resources.builtin);
            _builtIn = JsonConvert.DeserializeObject<string[]>(builtIn);
        }

        public byte[] Compile()
        {
            foreach (var childNode in _tree.Root.ChildNodes)
            {
                CompileInternal(childNode);
            }
            var buffer = new List<byte>();
            var byteCode = new List<byte>();
            foreach (var scriptFunction in _functions)
            {
                buffer.AddRange(scriptFunction.Buffer);
                byteCode.AddRange(scriptFunction.ByteCode);
            }
            var result = new List<byte>();
            result.AddRange(ZlibStream.CompressBuffer(buffer.ToArray()));
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
            AddByteCode(new byte[2]);
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
            return node.ChildNodes.Find(e => e.Term.Name == ParametersId);
        }

        private string FindFunctionName(ParseTreeNode node)
        {
            return node.ChildNodes.Find(e => e.Term.Name == IdentifierId)?.Token.ValueString.ToLower();
        }

        private void EmitBuiltInCall(ParseTreeNode node, bool decTop)
        {
            var functionName = FindFunctionName(node);
            if (node.Term.Name == FunctionCallId)
            {
                switch (functionName)
                {
                    case "wait":
                        CompileInternal(FindParametersNode(node));
                        AddOpcode(Opcode.OpWait);
                        return;
                }
                AddOpcode(Opcode.OpPreScriptCall);
                CompileInternal(FindParametersNode(node));
                AddOpcode(Opcode.OpCallBuiltin);
                AddByteCode((byte) FindParametersNode(node).ChildNodes.Count);
                AddId(_resolver.ResolveValueForFunction(functionName));
            }
            else if (node.Term.Name == MethodCallId)
            {
            }
            if (decTop)
            {
                AddOpcode(Opcode.OpDecTop);
            }
        }

        private void EmitCall(ParseTreeNode node, bool decTop)
        {
            var functionName = FindFunctionName(node);
            if (IsBuiltIn(functionName))
            {
                EmitBuiltInCall(node, decTop);
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

        private void CreateFunction(ParseTreeNode node)
        {
            var name = node.FindTokenAndGetString();
            var scriptFunction = new ScriptFunction {FunctionName = name};
            _functions.Add(scriptFunction);
            var parameters = FindParametersNode(node)?.ChildNodes
                .Select(e => e.Token.ValueString.ToLower())
                .ToList();

            AddOpcode(Opcode.OpEnd);
            if (parameters == null)
            {
                AddOpcode(Opcode.OpCheckclearparams);
            }
            else
            {
                foreach (var parameter in parameters)
                {
                    //TODO support for parameters
                }
            }
            CompileInternal(node.ChildNodes.Find(e => e.Term.Name == LinesId));
            AddOpcode(Opcode.OpEnd);
        }

        private void AddOpcode(Opcode opcode)
        {
            AddByteCode(_resolver.ResolveValueForOpcode(opcode));
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

        private bool IsBuiltIn(string function)
        {
            function = function.ToLower();
            return _builtIn.Contains(function);
        }
    }
}