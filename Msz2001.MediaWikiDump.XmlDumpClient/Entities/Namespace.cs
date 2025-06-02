using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Msz2001.MediaWikiDump.XmlDumpClient.Entities
{
    public class Namespace
    {
        public required int Id { get; init; }
        public required string Name { get; init; }
        public required CasingScheme? TitleCasing { get; init; }
    }
}
