using Microsoft.Extensions.Logging;

using Msz2001.MediaWikiDump.XmlDumpClient.Entities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Msz2001.MediaWikiDump.XmlDumpClient.Parsers
{
    internal partial class UserParser(ILoggerFactory loggerFactory)
    {
        private readonly ILogger logger = loggerFactory.CreateLogger<UserParser>();

        internal User Parse(XElement elem)
        {
            uint? id = null;
            string? name = null, ip = null;

            foreach (var child in elem.Elements())
            {
                switch (child.Name.LocalName)
                {
                    case "id":
                        id = uint.Parse(child.Value);
                        break;
                    case "username":
                        name = child.Value;
                        break;
                    case "ip":
                        ip = child.Value;
                        break;
                    default:
                        LogUnexpectedChildTag(logger, id, child.Name.LocalName);
                        break;
                }
            }

            if (id is null && name is null && ip is null)
                throw new InvalidOperationException("User element should have either <ip> or <id> and <username> tags.");

            if ((id is null && name is not null) || (id is not null && name is null))
                throw new InvalidOperationException("Both <id> and <username> have to be set if one is present " + ((id is null) ? $"(username: {name})." : $"(id: {id})."));

            if ((id is not null || name is not null) && ip is not null)
                throw new InvalidOperationException($"User's <ip> cannot be specified together with <ip> or <id> (user ID: {id}).");

            return new User
            {
                Id = id,
                Name = name,
                IP = ip,
            };
        }

        [LoggerMessage(Level = LogLevel.Warning, Message = "Unexpected element in <contributor> (ID: {id}): <{ElementName}>")]
        static partial void LogUnexpectedChildTag(ILogger logger, uint? id, string elementName);
    }
}
