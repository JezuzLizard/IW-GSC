using System;
using System.Linq;

namespace Dumper
{
    public static class Offsets
    {
        public static long AssetsPool { get; private set; }

        public static void SetupOffsetsForGameId(Game.GameId gameId)
        {
            switch (gameId)
            {
                case Game.GameId.Ghosts_MP:
                    AssetsPool = FindPattern(0x140001000, 0x145000000, "\x4C\x8D\x05\x00\x00\x00\x00\xF7\xE3",
                        "xxx????xx");
                    var offset = BitConverter.ToUInt32(Memory.Read(AssetsPool + 3, 4), 0);
                    AssetsPool += offset + 7;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(gameId), gameId, null);
            }
        }

        private static long FindPattern(long startAddress, long endAddress, string pattern, string mask)
        {
            var lpBuffer = new byte[endAddress - startAddress];
            lpBuffer = Memory.Read(startAddress, lpBuffer.Length);
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