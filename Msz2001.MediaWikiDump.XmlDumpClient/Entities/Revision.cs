using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Msz2001.MediaWikiDump.XmlDumpClient.Entities
{
    public class Revision
    {
        public required uint Id { get; init; }
        public required uint? ParentId { get; init; }
        public required Timestamp Timestamp { get; init; }
        public required User? User { get; init; }
        public required bool IsMinor { get; init; }
        public required string? Comment { get; init; }
        public required string ContentModel { get; init; }
        public required string ContentFormat { get; init; }
        public required uint Length { get; init; } // in bytes
    }
}
