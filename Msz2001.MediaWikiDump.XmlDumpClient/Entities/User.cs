using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Msz2001.MediaWikiDump.XmlDumpClient.Entities
{
    public class User
    {
        public uint? Id = null;
        public string? Name = null;
        public string? IP = null;

        public override string ToString()
        {
            return $"[User #{Id} {Name} | {IP}]";
        }
    }
}
