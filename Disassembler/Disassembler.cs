using System;
using System.Collections.Generic;
using System.IO;
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
                throw new ArgumentOutOfRangeException("File with script bytecode is not exists!");
            }
            if (!File.Exists(bufferPath))
            {
                throw new ArgumentOutOfRangeException("File with script buffer is not exists!");
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
                function.Name = _resolver.ResolveStringById(_buffer.ReadUInt16());
                if (functionLength > 512000)
                {
                    throw new InvalidOperationException("Incorrect buffer reading");
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
                //just skip data for now
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
                                $"offset = 0x{offset:X}, jump to index 0x{instruction.Index + 2 + offset:X}";
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
                            $"offset = 0x{offset:X}, jump to index 0x{instruction.Index + 2 + offset:X}";
                    }
                        break;

                    case Opcode.OpJumpback:
                    {
                        var offset = funcStream.ReadUInt16();
                        instructionData.AddData(offset);
                        instructionData.DataString =
                            $"offset = 0x{offset:X}, jump back to index 0x{instruction.Index + 1 - offset:X}";
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
                        funcStream.ReadSingle();
                        break;
                    case Opcode.OpScriptLocalThreadCall:
                    case Opcode.OpSwitch:
                    case Opcode.OpScriptLocalMethodThreadCall:
                    case Opcode.OpScriptLocalMethodChildThreadCall:
                    case Opcode.OpScriptLocalChildThreadCall:
                    case Opcode.OpGetInteger:
                        funcStream.ReadInt32();
                        break;

                    case Opcode.OpScriptFarMethodThreadCall:
                    case Opcode.OpScriptFarChildThreadCall:
                    case Opcode.OpScriptFarMethodChildThreadCall:
                    case Opcode.OpScriptFarThreadCall:
                        DisassembleFarCall(instructionData);
                        funcStream.ReadInt32();
                        break;

                    case Opcode.OpGetVector:
                        funcStream.ReadBytes(12);
                        break;

                    case Opcode.OpCallBuiltin:
                    case Opcode.OpScriptLocalMethodCall:
                    case Opcode.OpScriptLocalFunctionCall:
                    case Opcode.OpScriptLocalFunctionCall2:
                    case Opcode.OpGetLocalFunction:
                    case Opcode.OpCallBuiltinMethod:
                        funcStream.ReadBytes(3);
                        break;

                    case Opcode.OpScriptFarFunctionCall2:
                    case Opcode.OpScriptFarFunctionCall:
                    case Opcode.OpGetFarFunction:
                    case Opcode.OpScriptFarMethodCall:
                        DisassembleFarCall(instructionData);
                        funcStream.ReadBytes(3);
                        break;

                    case Opcode.OpEndswitch:
                        throw new InvalidOperationException("OpEndswitch is not supported yet");

                    case Opcode.OpGetAnimation:
                        funcStream.ReadBytes(8);
                        throw new InvalidOperationException("OpGetAnimation is not supported yet");

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
                        var fieldName = fieldId <= 0x95A1
                            ? _resolver.ResolveFieldNameById(fieldId)
                            : _buffer.ReadTerminatedString();
                        instructionData.AddData(fieldName);
                        instructionData.DataString = $"field name : {fieldName}";
                        break;

                    case Opcode.OpSetNewLocalVariableFieldCached0:
                    case Opcode.OpEvalNewLocalArrayRefCached0:
                    case Opcode.OpCallBuiltinPointer:
                    case Opcode.OpSafeCreateVariableFieldCached:
                    case Opcode.OpClearLocalVariableFieldCached:
                    case Opcode.OpScriptMethodThreadCallPointer:
                    case Opcode.OpSetLocalVariableFieldCached:
                    case Opcode.OpRemoveLocalVariables:
                    case Opcode.OpEvalLocalVariableRefCached:
                    case Opcode.OpEvalLocalArrayRefCached:
                    case Opcode.OpSafeSetVariableFieldCached:
                    case Opcode.OpScriptMethodChildThreadCallPointer:
                    case Opcode.OpEvalLocalVariableCached:
                    case Opcode.OpSafeSetWaittillVariableFieldCached:
                    case Opcode.OpScriptThreadCallPointer:
                    case Opcode.OpCreateLocalVariable:
                    case Opcode.OpEvalLocalVariableObjectCached:
                    case Opcode.OpCallBuiltinMethodPointer:
                    case Opcode.OpEvalLocalArrayCached:
                    case Opcode.OpScriptChildThreadCallPointer:
                        funcStream.ReadByte();
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

        private void DisassembleFarCall(InstructionData instructionData)
        {
            var fileNameId = _buffer.ReadUInt16();
            var fileName = fileNameId == 0 ? _buffer.ReadTerminatedString() : _resolver.ResolveStringById(fileNameId);
            var funcNameId = _buffer.ReadUInt16();
            var funcName = funcNameId == 0 ? _buffer.ReadTerminatedString() : _resolver.ResolveStringById(funcNameId);
            instructionData.AddData(fileName);
            instructionData.AddData(funcName);
            instructionData.DataString = $"{fileName}::{funcName}";
        }
    }
}