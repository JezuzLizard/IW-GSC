using System;
using System.IO;
using System.Reflection;
using NativeHelper;

namespace Dumper
{
    internal class Program
    {
        private static string StartupPath => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        private static void Main(string[] args)
        {
            Console.WriteLine("Select game id...");
            Console.WriteLine("1. Ghosts MP\n2. Ghosts Server");
            var result = 0;
            var gameId = Game.GameId.Ghosts_MP;
            if (int.TryParse(Console.ReadLine(), out result))
            {
                switch (result)
                {
                    case 1:
                        gameId = Game.GameId.Ghosts_MP;
                        break;

                    case 2:
                        gameId = Game.GameId.Ghosts_Server;
                        break;

                    default:
                        Console.WriteLine("Game is not supported");
                        Console.ReadKey();
                        return;
                }
            }
            Console.WriteLine("Waiting for game...");
            var native = new Native();
            while (!native.ConnectToGame(gameId)) ;
            Console.Clear();
            DumpGsc(native);
            Console.ReadKey();
        }

        private static void DumpGsc(Native native)
        {
            var assets = new AssetsReader<ScriptFile>(XAssetType.ScriptFile, native).ReadAssets();
            var assetsPath = Path.Combine(StartupPath, "Assets");
            if (!Directory.Exists(assetsPath))
            {
                Directory.CreateDirectory(assetsPath);
            }
            foreach (var scriptFile in assets)
            {
                var scriptName = Path.Combine(assetsPath, scriptFile.Name);
                File.WriteAllBytes(scriptName + ".buffer", scriptFile.Buffer);
                File.WriteAllBytes(scriptName + ".bytecode", scriptFile.ByteCode);
            }
            Console.WriteLine("GSC files successfully dumped!");
        }
    }
}