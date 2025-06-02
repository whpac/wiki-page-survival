using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Msz2001.MediaWikiDump.XmlDumpClient.Entities
{
    public class Page : IDumpEntry
    {
        public required uint Id { get; init; }
        public required Title Title { get; init; }
        public readonly List<Revision> Revisions = [];

        public Page(IEnumerable<Revision> revisions)
        {
            Revisions.AddRange(revisions);
        }
    }
}
