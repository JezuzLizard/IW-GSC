using Irony.Parsing;

namespace Compiler.Module
{
    internal static class IronyExtensions
    {
        public static string FindTokenAndGetString(this ParseTreeNode node)
        {
            return node.FindTokenAndGetText()?.ToLower();
        }
    }
}