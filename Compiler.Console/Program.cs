using System;
using System.IO;
using System.Windows.Forms;
using Compiler.Module;
using Resolver;

namespace Compiler.Console
{
    internal class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            using (var dialog = new OpenFileDialog())
            {
                if (dialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }
                var compiler = new ScriptCompiler(dialog.FileName, new BaseResolver(false, Game.Ghosts));
                var result = compiler.CompileToByteArray();
                var fileNameWithoutExtension = Path.Combine(Path.GetDirectoryName(dialog.FileName), Path.GetFileNameWithoutExtension(dialog.FileName));
                string compiledFileName = fileNameWithoutExtension + ".xasset";
                File.WriteAllBytes(compiledFileName, result);
            }
        }
    }
}