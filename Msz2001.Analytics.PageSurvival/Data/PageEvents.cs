using Msz2001.MediaWikiDump.XmlDumpClient.Entities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Msz2001.Analytics.PageSurvival.Data
{
    internal class PageEvents(Timestamp created)
    {
        public Timestamp Created = created;
        public Timestamp? Deleted;
        public CreatorType Creator = CreatorType.Unknown;
        public Timestamp? CreatorRegistration;

        public enum CreatorType
        {
            Unknown,
            User,
            Anonymous,
            CrossWikiUser
        }
    }
}
