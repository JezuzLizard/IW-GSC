using System;

namespace Dumper
{
    public static class Game
    {
        public enum GameId
        {
            Ghosts_MP,
            Ghosts_Server
        }

        public static string ProcessForGameId(GameId gameId)
        {
            switch (gameId)
            {
                case GameId.Ghosts_MP:
                    return "iw6mp64_ship";
                case GameId.Ghosts_Server:
                    return "iw6_ds";
                default:
                    throw new ArgumentOutOfRangeException(nameof(gameId), gameId, null);
            }
        }
    }
}