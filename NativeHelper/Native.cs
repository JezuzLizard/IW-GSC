using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace NativeHelper
{
    //TODO make generic read method
    public class Native
    {
        private const int Bytes = 0;
        private IntPtr _handle;
        public long AssetsPool { get; private set; }

        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        private static extern bool ReadProcessMemory(IntPtr hProcess, long lpBaseAddress, [Out] byte[] lpBuffer,
            int dwSize, [Out] int lpNumberOfBytesRead);

        [DllImport("kernel32.dll")]
        private static extern bool WriteProcessMemory(IntPtr hProcess, long lpBaseAddress, byte[] lpBuffer, uint nSize,
            [Out] int lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern long VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize,
            uint flAllocationType, uint flProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern int VirtualProtectEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flNewProtect,
            out uint lpflOldProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, uint dwStackSize,
            IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, [Out] uint lpThreadId);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern int VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint dwFreeType);

        public bool ConnectToGame(Game.GameId gameId)
        {
            var process = Process.GetProcessesByName(Game.ProcessForGameId(gameId)).FirstOrDefault();
            if (process != null)
            {
                _handle = OpenProcess(0x1f0fff, false, process.Id);
                SetupOffsetsForGameId(gameId);
                return true;
            }
            return false;
        }

        public void Free(long pointer, int length)
        {
            VirtualFreeEx(_handle, (IntPtr) pointer, (uint) length, 0x8000);
        }

        public void CreateRemoteThread(long pointer)
        {
            CreateRemoteThread(_handle, IntPtr.Zero, 0, (IntPtr) pointer, IntPtr.Zero, 0, Bytes);
        }

        public long Malloc(long length)
        {
            return VirtualAllocEx(_handle, IntPtr.Zero, (uint) length, 0x3000, 0x40);
        }

        public void WriteInt(long pointer, int value)
        {
            Write(pointer, BitConverter.GetBytes(value));
        }

        public long ReadLong(long pointer)
        {
            var buffer = new byte[8];
            ReadProcessMemory(_handle, pointer, buffer, 8, Bytes);
            return BitConverter.ToInt64(buffer, 0);
        }

        public int ReadInt(long pointer)
        {
            var buffer = new byte[4];
            ReadProcessMemory(_handle, pointer, buffer, 4, Bytes);
            return BitConverter.ToInt32(buffer, 0);
        }

        public byte[] Read(long pointer, int length)
        {
            var buffer = new byte[length];
            ReadProcessMemory(_handle, pointer, buffer, buffer.Length, Bytes);
            return buffer;
        }

        public string ReadString(long pointer)
        {
            var strPointer = ReadInt(pointer);
            var sb = new StringBuilder();
            while (Read(strPointer, 1)[0] != 0)
            {
                sb.Append(Convert.ToChar((Read(strPointer, 1)[0])));
                strPointer++;
            }
            return sb.ToString();
        }

        public void WriteLong(long pointer, long value)
        {
            Write(pointer, BitConverter.GetBytes(value));
        }

        public void Write(long pointer, byte[] b)
        {
            uint oldprotect;
            VirtualProtectEx(_handle, (IntPtr) pointer, (uint) b.Length, 0x40, out oldprotect);
            WriteProcessMemory(_handle, pointer, b, (uint) b.Length, Bytes);
        }

        private void SetupOffsetsForGameId(Game.GameId gameId)
        {
            switch (gameId)
            {
                case Game.GameId.Ghosts_MP:
                case Game.GameId.Ghosts_Server:
                    AssetsPool = FindPattern(0x140001000, 0x145000000, "\x4C\x8D\x05\x00\x00\x00\x00\xF7\xE3",
                        "xxx????xx");
                    var offset = BitConverter.ToUInt32(Read(AssetsPool + 3, 4), 0);
                    AssetsPool += offset + 7;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(gameId), gameId, null);
            }
        }

        private long FindPattern(long startAddress, long endAddress, string pattern, string mask)
        {
            var lpBuffer = new byte[endAddress - startAddress];
            lpBuffer = Read(startAddress, lpBuffer.Length);
            for (var i = 0; i < lpBuffer.Length; i++)
            {
                if (
                    pattern.TakeWhile((t, j) => (lpBuffer[i + j] == t) || (mask[j] == '?'))
                        .Where((t, j) => j == (pattern.Length - 1))
                        .Any())
                {
                    return (startAddress + i);
                }
            }
            return -1;
        }
    }
}