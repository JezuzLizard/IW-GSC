using System;

namespace Dumper
{
    public static class Game
    {
        public static string ProcessForGameId(GameId gameId)
        {
            switch (gameId)
            {
                case GameId.Ghost:
                    return "iw6mp64_ship";
                default:
                    throw new ArgumentOutOfRangeException(nameof(gameId), gameId, null);
            }
        }

        public enum GameId
        {
            Ghost
        }
    }
}