using System.Text;

namespace Compiler.Module
{
    internal static class StringExtensions
    {
        internal static byte[] ToByteArray(this string s)
        {
            return Encoding.ASCII.GetBytes(s + '\0');
        }
    }
}