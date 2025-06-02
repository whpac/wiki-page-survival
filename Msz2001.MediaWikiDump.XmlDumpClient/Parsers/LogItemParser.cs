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
    internal partial class LogItemParser(ILoggerFactory loggerFactory)
    {
        private readonly ILogger logger = loggerFactory.CreateLogger<LogItemParser>();
        private readonly UserParser UserParser = new(loggerFactory);

        internal LogItem Parse(XElement elem, SiteInfo siteinfo)
        {
            uint? id = null;
            Timestamp? timestamp = null;
            User? user = null;
            string? comment = null, type = null, action = null, parameters = null;
            Title? title = null;

            foreach (var child in elem.Elements())
            {
                switch (child.Name.LocalName)
                {
                    case "id":
                        id = uint.Parse(child.Value);
                        break;
                    case "timestamp":
                        timestamp = Timestamp.Parse(child.Value);
                        break;
                    case "contributor":
                        // If the user is deleted, don't create an object for it
                        if (child.Attribute("deleted") is null)
                            user = UserParser.Parse(child);

                        break;
                    case "comment":
                        // If the comment is deleted, keep it null
                        if (child.Attribute("deleted") is null)
                            comment = child.Value;
                        break;
                    case "type":
                        type = child.Value;
                        break;
                    case "action":
                        action = child.Value;
                        break;
                    case "logtitle":
                        title = Title.FromText(child.Value, siteinfo);
                        break;
                    case "params":
                        parameters = child.Value;
                        break;
                    case "text":
                        // Special for deleted params and target
                        // We don't have to do anything, just suppress the warning
                        break;
                    default:
                        LogUnexpectedChildTag(logger, id, child.Name.LocalName);
                        break;
                }
            }

            List<string> missingTags = [];
            if (id is null) missingTags.Add("<id>");
            if (timestamp is null) missingTags.Add("<timestamp>");
            if (type is null) missingTags.Add("<type>");
            if (action is null) missingTags.Add("<action>");

            if (missingTags.Count > 0)
                throw new Exception($"Missing tags in <logitem> (ID: {id}): {string.Join(", ", missingTags)}");

            return new LogItem(parameters)
            {
                Id = id!.Value,
                Timestamp = timestamp!.Value,
                User = user,
                Comment = comment,
                Type = type!,
                Action = action!,
                Title = title,
            };
        }

        [LoggerMessage(Level = LogLevel.Warning, Message = "Unexpected element in <logitem> (ID: {id}): <{ElementName}>")]
        static partial void LogUnexpectedChildTag(ILogger logger, uint? id, string elementName);
    }
}
