using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dumper
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Select game id...");
            Console.WriteLine("1. Ghost");
            int result = 0;
            Game.GameId gameId = Game.GameId.Ghost;
            if (int.TryParse(Console.ReadLine(), out result))
            {
                switch (result)
                {
                    case 1:
                        gameId = Game.GameId.Ghost;
                        break;

                    default:
                        Console.WriteLine("Game is not supported");
                        Console.ReadKey();
                        return;
                }
            }
            Console.WriteLine("Waiting for game...");
            while (!Memory.ConnectToGame(gameId)) ;
            Console.Clear();
            Offsets.SetupOffsetsForGameId(gameId);
            var assets = new AssetsReader<ScriptFile>(XAssetType.ScriptFile).ReadAssets();
            foreach (var scriptFile in assets)
            {
                Console.WriteLine(scriptFile.Name);
            }
            Console.ReadKey();
        }
    }
}
