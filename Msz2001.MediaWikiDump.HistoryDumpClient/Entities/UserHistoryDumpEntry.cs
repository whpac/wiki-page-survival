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
    public class UserHistoryDumpEntry : HistoryDumpEntry
    {
        public long UserId { get; }
        public string UserTextHistorical { get; }
        public string UserText { get; }
        private RawArray userBlocksHistoricalRaw;
        public string[] UserBlocksHistorical => userBlocksHistoricalRaw.Value;
        private RawArray userBlocksRaw;
        public string[] UserBlocks => userBlocksRaw.Value;
        private RawArray userGroupsHistoricalRaw;
        public string[] UserGroupsHistorical => userGroupsHistoricalRaw.Value;
        private RawArray userGroupsRaw;
        public string[] UserGroups => userGroupsRaw.Value;
        private RawArray userIsBotByHistoricalRaw;
        public string[] UserIsBotByHistorical => userIsBotByHistoricalRaw.Value;
        private RawArray userIsBotByRaw;
        public string[] UserIsBotBy => userIsBotByRaw.Value;
        public bool UserIsCreatedBySelf { get; }
        public bool UserIsCreatedBySystem { get; }
        public bool UserIsCreatedByPeer { get; }
        public bool UserIsAnonymous { get; }
        public bool UserIsTemporary { get; }
        public bool UserIsPermanent { get; }
        private RawDateTime userRegistrationTimestampRaw;
        public DateTime? UserRegistrationTimestamp => userRegistrationTimestampRaw.Value;
        private RawDateTime userCreationTimestampRaw;
        public DateTime? UserCreationTimestamp => userCreationTimestampRaw.Value;
        private RawDateTime userFirstEditTimestampRaw;
        public DateTime? UserFirstEditTimestamp => userFirstEditTimestampRaw.Value;

        internal UserHistoryDumpEntry(string[] columns) : base(columns)
        {
            UserId = ParseLong(columns[38]);
            UserTextHistorical = columns[39];
            UserText = columns[40];
            userBlocksHistoricalRaw = new RawArray(columns[41]);
            userBlocksRaw = new RawArray(columns[42]);
            userGroupsHistoricalRaw = new RawArray(columns[43]);
            userGroupsRaw = new RawArray(columns[44]);
            userIsBotByHistoricalRaw = new RawArray(columns[45]);
            userIsBotByRaw = new RawArray(columns[46]);
            UserIsCreatedBySelf = ParseBool(columns[47]);
            UserIsCreatedBySystem = ParseBool(columns[48]);
            UserIsCreatedByPeer = ParseBool(columns[49]);
            UserIsAnonymous = ParseBool(columns[50]);
            UserIsTemporary = ParseBool(columns[51]);
            UserIsPermanent = ParseBool(columns[52]);
            userRegistrationTimestampRaw = new RawDateTime(columns[53]);
            userCreationTimestampRaw = new RawDateTime(columns[54]);
            userFirstEditTimestampRaw = new RawDateTime(columns[55]);
        }
    }
}
