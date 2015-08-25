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
                    case Opcode.OpJumpOnTrueExpr:
                    case Opcode.OpGetUnsignedShort:
                    case Opcode.OpJumpOnTrue:
                    case Opcode.OpJumpOnFalseExpr:
                    case Opcode.OpGetBuiltinFunction:
                    case Opcode.OpJumpback:
                    case Opcode.OpGetBuiltinMethod:
                    case Opcode.OpWaittillmatch:
                    case Opcode.OpGetNegUnsignedShort:
                    case Opcode.OpCallBuiltinMethod0:
                    case Opcode.OpCallBuiltinMethod1:
                    case Opcode.OpCallBuiltinMethod2:
                    case Opcode.OpCallBuiltinMethod3:
                    case Opcode.OpCallBuiltinMethod4:
                    case Opcode.OpCallBuiltinMethod5:
                    case Opcode.OpJumpOnFalse:
                        funcStream.ReadInt16();
                        break;

                    case Opcode.OpGetFloat:
                    case Opcode.OpScriptLocalThreadCall:
                    case Opcode.OpSwitch:
                    case Opcode.OpScriptLocalMethodThreadCall:
                    case Opcode.OpScriptLocalMethodChildThreadCall:
                    case Opcode.OpJump:
                    case Opcode.OpScriptLocalChildThreadCall:
                    case Opcode.OpGetInteger:
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

                    case Opcode.OpEndswitch:
                        throw new ArgumentOutOfRangeException("OpEndswitch is not supported yet");

                    case Opcode.OpGetAnimation:
                        funcStream.ReadBytes(8);
                        break;

                    case Opcode.OpGetString:
                    case Opcode.OpGetIString:
                        funcStream.ReadBytes(4);
                        _buffer.ReadString();
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
                        funcStream.ReadInt16();
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
                    case Opcode.OpGetNegByte:
                    case Opcode.OpCallBuiltinMethodPointer:
                    case Opcode.OpEvalLocalArrayCached:
                    case Opcode.OpGetByte:
                    case Opcode.OpScriptChildThreadCallPointer:
                        funcStream.ReadByte();
                        break;
                }
                instructions.Add(instruction);
            }
            return instructions;
        }
    }
}