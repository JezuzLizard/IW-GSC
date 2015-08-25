using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Resolver;

namespace DIsassembler.Console
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
                var disassembler = new Disassembler.Disassembler(new DebugResolver(false, Game.Ghosts), dialog.FileName);
            }
        }
    }
}
