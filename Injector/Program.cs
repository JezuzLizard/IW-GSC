using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Compiler.Module;
using NativeHelper;
using Resolver;
using Game = NativeHelper.Game;

namespace Injector
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var native = new Native();
            while (!native.ConnectToGame(Game.GameId.Ghosts_MP)) ;
            var assets = new AssetsReader<ScriptFile>(XAssetType.ScriptFile, native).ReadAssets();
            var scriptFile = assets.Find(e => e.Name == "1351");
            if (scriptFile == null)
            {
                return;
            }
            using (var dialog = new OpenFileDialog())
            {
                if (dialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }
                var compiledFile = new ScriptCompiler(dialog.FileName, new BaseResolver(false, Resolver.Game.Ghosts)).CompileToAsset();
                scriptFile.Buffer = compiledFile.Buffer;
                scriptFile.ByteCode = compiledFile.ByteCode;
                scriptFile.Length = compiledFile.UncompressedLength;
                scriptFile.ByteCodeLength = compiledFile.ByteCodeLength;
                scriptFile.CompressedLength = compiledFile.CompressedLength;
            }
        }
    }
}
