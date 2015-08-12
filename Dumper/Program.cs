using System;
using System.IO;
using System.Reflection;

namespace Dumper
{
    internal class Program
    {
        private static string StartupPath => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        private static void Main(string[] args)
        {
            Console.WriteLine("Select game id...");
            Console.WriteLine("1. Ghosts_MP");
            var result = 0;
            var gameId = Game.GameId.Ghosts_MP;
            if (int.TryParse(Console.ReadLine(), out result))
            {
                switch (result)
                {
                    case 1:
                        gameId = Game.GameId.Ghosts_MP;
                        break;

                    default:
                        Console.WriteLine("Game is not supported");
                        Console.ReadKey();
                        return;
                }
            }
            Console.WriteLine("Waiting for game...");
            Native native = new Native();
            while (!native.ConnectToGame(gameId)) ;
            Console.Clear();
            DumpGSC(native);
            Console.ReadKey();
        }

        private static void DumpGSC(Native native)
        {
            var assets = new AssetsReader<ScriptFile>(XAssetType.ScriptFile, native).ReadAssets();
            var assetsPath = Path.Combine(StartupPath, "Assets");
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
            Console.WriteLine("GSC files successfully dumped!");
        }
    }
}