using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Resolver;

namespace Disassembler
{
    public class Disassembler
    {
        private readonly BinaryReader _buffer;
        private readonly BinaryReader _bytecode;
        private readonly BaseResolver _resolver;

        /// <summary>
        ///     Creates instance of disassembler
        /// </summary>
        /// <param name="resolver">Resolver</param>
        /// <param name="fileName">path to target file</param>
        public Disassembler(BaseResolver resolver, string fileName)
        {
            _resolver = resolver;
            var path = Path.Combine(Path.GetDirectoryName(fileName), Path.GetFileNameWithoutExtension(fileName));
            var bytecodePath = path + ".bytecode";
            var bufferPath = path + ".buffer";
            if (!File.Exists(bytecodePath))
            {
                throw new InvalidOperationException("File with script bytecode is not exists!");
            }
            if (!File.Exists(bufferPath))
            {
                throw new InvalidOperationException("File with script buffer is not exists!");
            }
            _bytecode = new BinaryReader(File.OpenRead(bytecodePath));
            _buffer = new BinaryReader(File.OpenRead(bufferPath));
        }

        public List<Function> Disassemble()
        {
            var functions = new List<Function>();
            _bytecode.ReadByte();
            while (_bytecode.PeekChar() != -1)
            {
                var function = new Function();
                var functionLength = _buffer.ReadInt32();
                var functionNameId = _buffer.ReadUInt16();
                function.Name = functionNameId == 0
                    ? _buffer.ReadTerminatedString()
                    : _resolver.ResolveStringNamegById(functionNameId);

                if (functionLength > 512000)
                {
                    throw new InvalidOperationException("Unexpected error while reading buffer");
                }
                function.Instructions = DisassembleFunction(_bytecode.ReadBytes(functionLength));
                functions.Add(function);
            }
            return functions;
        }

        private List<Instruction> DisassembleFunction(byte[] functionByteCode)
        {
            var instructions = new List<Instruction>();
            var funcStream = new BinaryReader(new MemoryStream(functionByteCode));
            while (funcStream.PeekChar() != -1)
            {
                var instruction = new Instruction();
                var instructionData = new InstructionData();
                instruction.Data = instructionData;
                instruction.Index =
                    (int) (_bytecode.BaseStream.Position - functionByteCode.Length + funcStream.BaseStream.Position);
                var opcode = _resolver.ResolveOpcodeById(funcStream.ReadByte());
                instruction.Opcode = opcode;
                var currentIndex = instruction.Index + 1;
                switch (opcode)
                {
                    case Opcode.OpCallBuiltin0:
                    case Opcode.OpCallBuiltin1:
                    case Opcode.OpCallBuiltin2:
                    case Opcode.OpCallBuiltin3:
                    case Opcode.OpCallBuiltin4:
                    case Opcode.OpCallBuiltin5:
                    case Opcode.OpGetBuiltinFunction:
                    {
                        var funcId = funcStream.ReadUInt16();
                        instructionData.AddData(funcId);
                        instructionData.DataString = $"function name : {_resolver.ResolveFunctionNameById(funcId)}";
                    }
                        break;
                    case Opcode.OpJump:
                    {
                        var offset = funcStream.ReadInt32();
                        instructionData.AddData(offset);
                        instructionData.DataString =
                            $"offset = 0x{offset:X}, jump to index 0x{currentIndex + 4 + offset:X}";
                    }
                        break;
                    case Opcode.OpJumpOnTrueExpr:
                    case Opcode.OpJumpOnTrue:
                    case Opcode.OpJumpOnFalseExpr:
                    case Opcode.OpJumpOnFalse:
                    {
                        var offset = funcStream.ReadInt16();
                        instructionData.AddData(offset);
                        instructionData.DataString =
                            $"offset = 0x{offset:X}, jump to index 0x{currentIndex + 2 + offset:X}";
                    }
                        break;

                    case Opcode.OpJumpback:
                    {
                        var offset = funcStream.ReadUInt16();
                        instructionData.AddData(offset);
                        instructionData.DataString =
                            $"offset = 0x{offset:X}, jump back to index 0x{currentIndex + 2 - offset:X}";
                    }
                        break;
                    case Opcode.OpCallBuiltinMethod0:
                    case Opcode.OpCallBuiltinMethod1:
                    case Opcode.OpCallBuiltinMethod2:
                    case Opcode.OpCallBuiltinMethod3:
                    case Opcode.OpCallBuiltinMethod4:
                    case Opcode.OpCallBuiltinMethod5:
                    case Opcode.OpGetBuiltinMethod:
                    {
                        var funcId = funcStream.ReadUInt16();
                        instructionData.AddData(funcId);
                        instructionData.DataString = $"method name : {_resolver.ResolveMethodNameById(funcId)}";
                    }
                        break;
                    case Opcode.OpWaittillmatch:
                        funcStream.ReadUInt16();
                        break;

                    case Opcode.OpGetUnsignedShort:
                    {
                        var value = funcStream.ReadUInt16();
                        instructionData.AddData(value);
                        instructionData.DataString = $"value : {value}";
                    }
                        break;
                    case Opcode.OpGetNegUnsignedShort:
                    {
                        var value = funcStream.ReadUInt16();
                        instructionData.AddData(value);
                        instructionData.DataString = $"value : {-value}";
                    }
                        break;

                    case Opcode.OpGetFloat:
                        var f = funcStream.ReadSingle();
                        instructionData.AddData(f);
                        instructionData.DataString = $"value = {f}";
                        break;

                    case Opcode.OpSwitch:
                        funcStream.ReadInt32();
                        break;

                    case Opcode.OpGetInteger:
                        var i = funcStream.ReadInt32();
                        instructionData.AddData(i);
                        instructionData.DataString = $"value = {i}";
                        break;

                    case Opcode.OpScriptLocalThreadCall:
                    case Opcode.OpScriptLocalMethodThreadCall:
                    case Opcode.OpScriptLocalMethodChildThreadCall:
                    case Opcode.OpScriptLocalChildThreadCall:
                        DisassembleLocalCall(instructionData, funcStream, currentIndex, true);
                        break;

                    case Opcode.OpScriptFarMethodThreadCall:
                    case Opcode.OpScriptFarChildThreadCall:
                    case Opcode.OpScriptFarMethodChildThreadCall:
                    case Opcode.OpScriptFarThreadCall:
                        DisassembleFarCall(instructionData, funcStream, true);
                        break;

                    //TODO
                    case Opcode.OpGetVector:
                        funcStream.ReadBytes(12);
                        break;


                    case Opcode.OpScriptLocalMethodCall:
                    case Opcode.OpScriptLocalFunctionCall:
                    case Opcode.OpScriptLocalFunctionCall2:
                    case Opcode.OpGetLocalFunction:
                        DisassembleLocalCall(instructionData, funcStream, currentIndex, false);
                        break;
                    case Opcode.OpCallBuiltinMethod:
                    case Opcode.OpCallBuiltin:
                        funcStream.ReadBytes(3);
                        break;

                    case Opcode.OpScriptFarFunctionCall2:
                    case Opcode.OpScriptFarFunctionCall:
                    case Opcode.OpGetFarFunction:
                    case Opcode.OpScriptFarMethodCall:
                        DisassembleFarCall(instructionData, funcStream, false);
                        break;

                    case Opcode.OpEndswitch:
                        DisassembleEndSwitch(instructionData, funcStream, currentIndex);
                        break;

                    //TODO
                    case Opcode.OpGetAnimation:
                        funcStream.ReadBytes(8);
                        throw new InvalidOperationException("OpGetAnimation is not supported yet");

                    //TODO
                    case Opcode.OpGetAnimTree:
                        _buffer.ReadTerminatedString();
                        funcStream.ReadByte();
                        break;

                    case Opcode.OpGetString:
                    case Opcode.OpGetIString:
                        funcStream.ReadBytes(4);
                        var s = _buffer.ReadTerminatedString();
                        instructionData.AddData(s);
                        instructionData.DataString = $"value = {s}";
                        break;

                    case Opcode.OpEvalSelfFieldVariable:
                    case Opcode.OpSetLevelFieldVariableField:
                    case Opcode.OpClearFieldVariable:
                    case Opcode.OpEvalFieldVariable:
                    case Opcode.OpEvalFieldVariableRef:
                    case Opcode.OpEvalLevelFieldVariable:
                    case Opcode.OpSetAnimFieldVariableField:
                    case Opcode.OpSetSelfFieldVariableField:
                    case Opcode.OpEvalAnimFieldVariableRef:
                    case Opcode.OpEvalLevelFieldVariableRef:
                    case Opcode.OpEvalAnimFieldVariable:
                    case Opcode.OpEvalSelfFieldVariableRef:
                        var fieldId = funcStream.ReadUInt16();
                        string fieldName;
                        if (fieldId <= 0x95A1)
                        {
                            fieldName = _resolver.ResolveFieldNameById(fieldId);
                        }
                        else
                        {
                            fieldId = _buffer.ReadUInt16();
                            fieldName = fieldId == 0
                                ? _buffer.ReadTerminatedString()
                                : _resolver.ResolveFieldNameById(fieldId);
                        }
                        instructionData.AddData(fieldName);
                        instructionData.DataString = $"field name : {fieldName}";
                        break;


                    case Opcode.OpSetNewLocalVariableFieldCached0:
                    case Opcode.OpEvalNewLocalArrayRefCached0:
                    case Opcode.OpSafeCreateVariableFieldCached:
                    case Opcode.OpClearLocalVariableFieldCached:
                    case Opcode.OpSetLocalVariableFieldCached:
                    case Opcode.OpRemoveLocalVariables:
                    case Opcode.OpEvalLocalVariableRefCached:
                    case Opcode.OpEvalLocalArrayRefCached:
                    case Opcode.OpSafeSetVariableFieldCached:
                    case Opcode.OpEvalLocalVariableCached:
                    case Opcode.OpSafeSetWaittillVariableFieldCached:
                    case Opcode.OpCreateLocalVariable:
                    case Opcode.OpEvalLocalVariableObjectCached:
                    case Opcode.OpEvalLocalArrayCached:
                    {
                        int index = funcStream.ReadByte();
                        instructionData.AddData(index);
                        instructionData.DataString = $"index = {index}";
                    }
                        break;

                    case Opcode.OpScriptChildThreadCallPointer:
                    case Opcode.OpCallBuiltinMethodPointer:
                    case Opcode.OpCallBuiltinPointer:
                    case Opcode.OpScriptMethodThreadCallPointer:
                    case Opcode.OpScriptMethodChildThreadCallPointer:
                    case Opcode.OpScriptThreadCallPointer:
                    {
                        int numOfParams = funcStream.ReadByte();
                        instructionData.AddData(numOfParams);
                        instructionData.DataString = $"parameters count = {numOfParams}";
                    }
                        break;

                    case Opcode.OpGetByte:
                    {
                        var b = funcStream.ReadByte();
                        instructionData.AddData(b);
                        instructionData.DataString = $"value = {b}";
                    }
                        break;
                    case Opcode.OpGetNegByte:
                    {
                        var b = funcStream.ReadByte();
                        instructionData.AddData(b);
                        instructionData.DataString = $"value = {-b}";
                    }
                        break;
                }
                instructions.Add(instruction);
            }
            return instructions;
        }

        private void DisassembleEndSwitch(InstructionData instructionData, BinaryReader funcStream, int currentIndex)
        {
            var numOfCases = funcStream.ReadUInt16();
            var builder = new StringBuilder();
            for (var i = 0; i < numOfCases; i++)
            {
                var ptr = funcStream.ReadUInt32();
                if (ptr < 0x40000 && ptr > 0)
                {
                    var switchStatement = _buffer.ReadTerminatedString();
                    builder.Append($"case: {switchStatement}");
                }
                else if (ptr < 0x40000)
                {
                    _buffer.ReadUInt16();
                    builder.Append("case: default");
                }
                else
                {
                    var container = new byte[2];
                    Array.Copy(BitConverter.GetBytes(ptr), container, 2);
                    builder.Append($"case: {BitConverter.ToUInt16(container, 0)}");
                }
                if (i != numOfCases - 1)
                {
                    builder.Append(", ");
                }
                //TODO
                funcStream.ReadBytes(3);
            }
            instructionData.DataString = builder.ToString();
        }

        private void DisassembleLocalCall(InstructionData instructionData, BinaryReader funcStream, int currentIndex,
            bool thread)
        {
            var initial = new byte[4];
            Array.Copy(funcStream.ReadBytes(3), initial, 3);
            var offset = BitConverter.ToInt32(initial, 0);
            offset = offset << 8 >> 10;
            instructionData.AddData(offset);
            if (thread)
            {
                var numOfParams = funcStream.ReadByte();
                instructionData.AddData(numOfParams);
                instructionData.DataString =
                    $"pointer to call = 0x{offset + currentIndex:X}, parameters count = {numOfParams}, offset = {offset}";
            }
            else
            {
                instructionData.DataString = $"pointer to call = 0x{offset + currentIndex:X}, offset = {offset}";
            }
        }

        private void DisassembleFarCall(InstructionData instructionData, BinaryReader funcStream, bool thread)
        {
            var fileNameId = _buffer.ReadUInt16();
            var fileName = fileNameId == 0 ? _buffer.ReadTerminatedString() : _resolver.ResolveStringNamegById(fileNameId);
            var funcNameId = _buffer.ReadUInt16();
            var funcName = funcNameId == 0 ? _buffer.ReadTerminatedString() : _resolver.ResolveStringNamegById(funcNameId);
            instructionData.AddData(fileName);
            instructionData.AddData(funcName);
            funcStream.ReadBytes(3);
            if (thread)
            {
                var numOfParams = funcStream.ReadByte();
                instructionData.DataString = $"{fileName}::{funcName} parameters count = {numOfParams}";
            }
            else
            {
                instructionData.DataString = $"{fileName}::{funcName}";
            }
        }
    }
}