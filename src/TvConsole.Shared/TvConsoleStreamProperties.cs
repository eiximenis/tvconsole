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
        public bool UseFileApis { get; }

        public TvConsoleStreamProperties(int cp, Encoding encoding, bool redirected)
        {
            CodePage = cp;
            Encoding = encoding;
            IsRedirected = redirected;
            UseFileApis = Encoding != Encoding.Unicode || IsRedirected;
        }
    }
}
