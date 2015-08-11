using System;

namespace Dumper
{
    public static class Game
    {
        public enum GameId
        {
            Ghosts_MP
        }

        public static string ProcessForGameId(GameId gameId)
        {
            switch (gameId)
            {
                case GameId.Ghosts_MP:
                    return "iw6mp64_ship";
                default:
                    throw new ArgumentOutOfRangeException(nameof(gameId), gameId, null);
            }
        }
    }
}