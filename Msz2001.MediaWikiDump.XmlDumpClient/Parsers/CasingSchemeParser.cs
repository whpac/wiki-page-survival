using Msz2001.MediaWikiDump.XmlDumpClient.Entities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Msz2001.MediaWikiDump.XmlDumpClient.Parsers
{
    internal class CasingSchemeParser
    {
        internal static CasingScheme Parse(string scheme)
        {
            return scheme switch
            {
                "first-letter" => CasingScheme.FirstLetterCaseInsensitive,
                "case-sensitive" => CasingScheme.CaseSensitive,
                "case-insensitive" => CasingScheme.CaseInsensitive,
                _ => throw new Exception($"Unknown casing scheme: {scheme}")
            };
        }
    }
}
