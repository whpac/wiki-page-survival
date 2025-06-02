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
    public class HistoryDumpEntry
    {
        // Event global fields
        public string WikiDB { get; }
        public string EventEntity { get; }
        public string EventType { get; }
        private RawDateTime eventTimestampRaw;
        public DateTime EventTimestamp => (DateTime)eventTimestampRaw.Value!;
        public string EventComment { get; }

        // Event user fields
        public long? EventUserId { get; }
        public string EventUserTextHistorical { get; }
        public string EventUserTextCurrent { get; }
        private RawArray eventUserBlocksHistoricalRaw;
        public string[] EventUserBlocksHistorical => eventUserBlocksHistoricalRaw.Value;
        private RawArray eventUserBlocksRaw;
        public string[] EventUserBlocks => eventUserBlocksRaw.Value;
        private RawArray eventUserGroupsHistoricalRaw;
        public string[] EventUserGroupsHistorical => eventUserGroupsHistoricalRaw.Value;
        private RawArray eventUserGroupsRaw;
        public string[] EventUserGroups => eventUserGroupsRaw.Value;
        private RawArray eventUserIsBotByHistoricalRaw;
        public string[] EventUserIsBotByHistorical => eventUserIsBotByHistoricalRaw.Value;
        private RawArray eventUserIsBotByRaw;
        public string[] EventUserIsBotBy => eventUserIsBotByRaw.Value;
        public bool? EventUserIsCreatedBySelf { get; }
        public bool? EventUserIsCreatedBySystem { get; }
        public bool? EventUserIsCreatedByPeer { get; }
        public bool? EventUserIsAnonymous { get; }
        public bool? EventUserIsTemporary { get; }
        public bool? EventUserIsPermanent { get; }
        private RawDateTime eventUserRegistrationTimestampRaw;
        public DateTime? EventUserRegistrationTimestamp => eventUserRegistrationTimestampRaw.Value;
        private RawDateTime eventUserCreationTimestampRaw;
        public DateTime? EventUserCreationTimestamp => eventUserCreationTimestampRaw.Value;
        private RawDateTime eventUserFirstEditTimestampRaw;
        public DateTime? EventUserFirstEditTimestamp => eventUserFirstEditTimestampRaw.Value;
        public long? EventUserRevisionCount { get; }
        public long? EventUserSecondsSincePreviousRevision { get; }

        internal HistoryDumpEntry(string[] columns)
        {
            WikiDB = columns[0];
            EventEntity = columns[1];
            EventType = columns[2];
            eventTimestampRaw = new RawDateTime(columns[3]);
            EventComment = columns[4];
            EventUserId = ParseLongNullable(columns[5]);
            EventUserTextHistorical = columns[6];
            EventUserTextCurrent = columns[7];
            eventUserBlocksHistoricalRaw = new RawArray(columns[8]);
            eventUserBlocksRaw = new RawArray(columns[9]);
            eventUserGroupsHistoricalRaw = new RawArray(columns[10]); // at the time of edit/skips logs from meta
            eventUserGroupsRaw = new RawArray(columns[11]); // now
            eventUserIsBotByHistoricalRaw = new RawArray(columns[12]);
            eventUserIsBotByRaw = new RawArray(columns[13]);
            EventUserIsCreatedBySelf = ParseBoolNullable(columns[14]);
            EventUserIsCreatedBySystem = ParseBoolNullable(columns[15]);
            EventUserIsCreatedByPeer = ParseBoolNullable(columns[16]);
            EventUserIsAnonymous = ParseBoolNullable(columns[17]);
            EventUserIsTemporary = ParseBoolNullable(columns[18]);
            EventUserIsPermanent = ParseBoolNullable(columns[19]);
            eventUserRegistrationTimestampRaw = new RawDateTime(columns[20]);
            eventUserCreationTimestampRaw = new RawDateTime(columns[21]);
            eventUserFirstEditTimestampRaw = new RawDateTime(columns[22]);
            EventUserRevisionCount = ParseLongNullable(columns[23]);
            EventUserSecondsSincePreviousRevision = ParseLongNullable(columns[24]);
        }

        protected bool? ParseBoolNullable(string raw)
        {
            if (bool.TryParse(raw, out bool result))
            {
                return result;
            }
            return null;
        }

        protected bool ParseBool(string raw)
        {
            return bool.Parse(raw);
        }

        protected int? ParseIntNullable(string raw)
        {
            if (int.TryParse(raw, out int result))
            {
                return result;
            }
            return null;
        }

        protected long? ParseLongNullable(string raw)
        {
            if (long.TryParse(raw, out long result))
            {
                return result;
            }
            return null;
        }

        protected long ParseLong(string raw)
        {
            return long.Parse(raw);
        }
    }
}
