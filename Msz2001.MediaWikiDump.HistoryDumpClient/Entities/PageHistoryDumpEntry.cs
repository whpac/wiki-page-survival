using Msz2001.MediaWikiDump.HistoryDumpClient.Entities.RawValues;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Msz2001.MediaWikiDump.HistoryDumpClient.Entities
{
    public class PageHistoryDumpEntry : HistoryDumpEntry
    {
        public long? PageId { get; }
        public string PageTitleHistorical { get; }
        public string PageTitle { get; }
        public int? PageNamespaceHistorical { get; }
        public bool? PageNamespaceIsContentHistorical { get; }
        public int? PageNamespace { get; }
        public bool? PageNamespaceIsContent { get; }
        public bool? PageIsRedirect { get; }
        public bool? PageIsDeleted { get; }
        private RawDateTime pageCreationTimestampRaw;
        public DateTime? PageCreationTimestamp => pageCreationTimestampRaw.Value;
        private RawDateTime pageFirstEditTimestampRaw;
        public DateTime? PageFirstEditTimestamp => pageFirstEditTimestampRaw.Value;
        public long? PageRevisionCount { get; }
        public long? PageSecondsSincePreviousRevision { get; }

        internal PageHistoryDumpEntry(string[] columns) : base(columns)
        {
            PageId = ParseLongNullable(columns[25]);
            PageTitleHistorical = columns[26];
            PageTitle = columns[27];
            PageNamespaceHistorical = ParseIntNullable(columns[28]);
            PageNamespaceIsContentHistorical = ParseBoolNullable(columns[29]);
            PageNamespace = ParseIntNullable(columns[30]);
            PageNamespaceIsContent = ParseBoolNullable(columns[31]);
            PageIsRedirect = ParseBoolNullable(columns[32]);
            PageIsDeleted = ParseBoolNullable(columns[33]);
            pageCreationTimestampRaw = new RawDateTime(columns[34]);
            pageFirstEditTimestampRaw = new RawDateTime(columns[35]);
            PageRevisionCount = ParseLongNullable(columns[36]);
            PageSecondsSincePreviousRevision = ParseLongNullable(columns[37]);
        }
    }
}
