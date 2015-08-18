using System;
using System.Collections.Generic;

namespace Compiler.Module
{
    internal class ScriptFunction
    {
        private readonly List<byte> _byteCode;
        private readonly List<object> _dataMembers;

        public ScriptFunction()
        {
            _dataMembers = new List<object>();
            _byteCode = new List<byte>();
        }

        public int FunctionLength { get; set; }
        public short FunctionId { get; set; }
        public string FunctionName { get; set; }
        public byte[] ByteCode => _byteCode.ToArray();

        public byte[] Buffer
        {
            get
            {
                var bytes = new List<byte>();
                bytes.AddRange(BitConverter.GetBytes(FunctionLength));
                bytes.AddRange(BitConverter.GetBytes(FunctionId));
                if (FunctionId == 0)
                {
                    bytes.AddRange(FunctionName.ToByteArray());
                }
                foreach (var dataMember in _dataMembers)
                {
                    if (dataMember is short)
                    {
                        bytes.AddRange(BitConverter.GetBytes((short) dataMember));
                    }
                    else if (dataMember is string)
                    {
                        bytes.AddRange((dataMember as string).ToByteArray());
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException(
                            $"Found unexpected data member with type {dataMember.GetType()}");
                    }
                }
                return bytes.ToArray();
            }
        }

        public void AddByteCode(params byte[] bytes)
        {
            _byteCode.AddRange(bytes);
        }

        public void AddDataMember(object member)
        {
            if (!(member is short) && !(member is string))
            {
                throw new ArgumentOutOfRangeException($"member with type: {member.GetType()} is not supported");
            }
            _dataMembers.Add(member);
        }
    }
}