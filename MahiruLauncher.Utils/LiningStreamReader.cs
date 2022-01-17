using System.Diagnostics;
using System.IO;
using System.Text;

namespace MahiruLauncher.Utils
{
    public static class LiningStreamReader
    {
        public static string ReadLineWithEnding(this StreamReader stream)
        {
            var sb = new StringBuilder();
            while (true)
            {
                var ch = stream.Read();
                if (ch == -1) break;
                sb.Append((char)ch);
                if (ch == '\r' || ch == '\n')
                {
                    return sb.ToString();
                }
            }
            return sb.Length > 0 ? sb.ToString() : null;
        }
    }
}