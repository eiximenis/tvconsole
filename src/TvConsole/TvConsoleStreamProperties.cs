using System;
using System.Collections.Generic;
using System.Text;

namespace TvConsole
{
    public class TvConsoleStreamProperties
    {
        public int CodePage { get; }
        public Encoding Encoding { get; }

        public bool IsRedirected { get; }
        internal bool UseFileApis { get; }

        public TvConsoleStreamProperties(int cp, Encoding encoding, bool redirected, bool forceFileApis = false)
        {
            CodePage = cp;
            Encoding = encoding;
            IsRedirected = redirected;
            UseFileApis = forceFileApis || (Encoding != Encoding.Unicode || IsRedirected);
        }
    }
}
