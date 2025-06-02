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
    public class RevisionHistoryDumpEntry : PageHistoryDumpEntry
    {
        public long RevisionId { get; }
        public long? RevisionParentId { get; }
        public bool RevisionMinorEdit { get; }
        private RawArray revisionDeletedPartsRaw;
        public string[] RevisionDeletedParts => revisionDeletedPartsRaw.Value;
        public bool RevisionDeletedPartsSuppressed { get; }
        public long? RevisionTextBytes { get; }
        public long? RevisionTextBytesDiff { get; }
        public string RevisionTextSha1 { get; }
        public string RevisionContentModel { get; }
        public string RevisionContentFormat { get; }
        public bool RevisionIsDeletedByPageDeletion { get; }
        private RawDateTime revisionDeletedByPageDeletionTimestampRaw;
        public DateTime? RevisionDeletedByPageDeletionTimestamp => revisionDeletedByPageDeletionTimestampRaw.Value;
        public bool RevisionIsIdentityReverted { get; }
        public long? RevisionFirstIdentityRevertingRevisionId { get; }
        public long? RevisionSecondsToIdentityRevert { get; }
        public bool RevisionIsIdentityRevert { get; }
        public bool RevisionIsFromBeforePageCreation { get; }
        private RawArray revisionTagsRaw;
        public string[] RevisionTags => revisionTagsRaw.Value;

        internal RevisionHistoryDumpEntry(string[] columns) : base(columns)
        {
            RevisionId = ParseLong(columns[56]);
            RevisionParentId = ParseLongNullable(columns[57]);
            RevisionMinorEdit = ParseBool(columns[58]);
            revisionDeletedPartsRaw = new RawArray(columns[59]);
            RevisionDeletedPartsSuppressed = ParseBool(columns[60]);
            RevisionTextBytes = ParseLongNullable(columns[61]);
            RevisionTextBytesDiff = ParseLongNullable(columns[62]);
            RevisionTextSha1 = columns[63];
            RevisionContentModel = columns[64];
            RevisionContentFormat = columns[65];
            RevisionIsDeletedByPageDeletion = ParseBool(columns[66]);
            revisionDeletedByPageDeletionTimestampRaw = new RawDateTime(columns[67]);
            RevisionIsIdentityReverted = ParseBool(columns[68]);
            RevisionFirstIdentityRevertingRevisionId = ParseLongNullable(columns[69]);
            RevisionSecondsToIdentityRevert = ParseLongNullable(columns[70]);
            RevisionIsIdentityRevert = ParseBool(columns[71]);
            RevisionIsFromBeforePageCreation = ParseBool(columns[72]);
            revisionTagsRaw = new RawArray(columns[73]);
        }
    }
}
