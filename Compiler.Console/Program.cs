using System;
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
                var compiler = new ScriptCompiler(dialog.FileName, new FakeResolver(false, Game.Ghosts));
                compiler.Compile();
            }
        }
    }
}