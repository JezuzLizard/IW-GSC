using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
            string assetsPath = Path.Combine(StartupPath, "Assets");
            if (!Directory.Exists(assetsPath))
            {
                Directory.CreateDirectory(assetsPath);
            }
            foreach (var scriptFile in assets)
            {
                using (var file = File.Create(Path.Combine(assetsPath, scriptFile.Name)))
                {
                    file.Write(scriptFile.Buffer, 0, scriptFile.Buffer.Length);
                    file.Write(scriptFile.ByteCode, 0, scriptFile.ByteCode.Length);
                }
            }
            Console.ReadKey();
        }

        private static string StartupPath => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
    }
}
