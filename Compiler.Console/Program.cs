using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Compiler.Module;

namespace Compiler.Console
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            using (var dialog = new OpenFileDialog())
            {
                if (dialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }
                var compiler = new ScriptCompiler(dialog.FileName);
                compiler.Compile();
            }
        }
    }
}
