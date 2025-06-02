using Msz2001.MediaWikiDump.XmlDumpClient.Parsers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Msz2001.MediaWikiDump.XmlDumpClient.Entities
{
    public class SiteInfo
    {
        public required string SiteName { get; init; }
        public required string DatabaseName { get; init; }
        public required string HomeUrl { get; init; }
        public required string Software { get; init; }
        public required CasingScheme DefaultCasing { get; init; }
        public readonly Dictionary<int, Namespace> Namespaces = [];

        public SiteInfo(IEnumerable<Namespace> namespaces)
        {
            foreach (var ns in namespaces)
                Namespaces.Add(ns.Id, ns);
        }

        public Namespace GetMainNamespace()
        {
            return Namespaces.Values.First(ns => ns.Id == 0);
        }
    }
}
