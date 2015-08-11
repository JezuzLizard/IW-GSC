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
                    return "uw6mp";
                default:
                    throw new ArgumentOutOfRangeException(nameof(gameId), gameId, null);
            }
        }

        public enum GameId
        {
            Ghost = 1
        }
    }
}