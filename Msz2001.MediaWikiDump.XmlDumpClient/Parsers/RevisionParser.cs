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
    internal partial class RevisionParser(ILoggerFactory loggerFactory)
    {
        private readonly ILogger logger = loggerFactory.CreateLogger<RevisionParser>();
        private readonly UserParser UserParser = new(loggerFactory);

        internal Revision Parse(XElement elem)
        {
            uint? id = null, parentId = null, length = null;
            Timestamp? timestamp = null;
            User? user = null;
            bool isMinor = false;
            string? comment = null, contentModel = null, contentFormat = null;

            foreach (var child in elem.Elements())
            {
                switch (child.Name.LocalName)
                {
                    case "id":
                        id = uint.Parse(child.Value);
                        break;
                    case "parentid":
                        parentId = uint.Parse(child.Value);
                        break;
                    case "timestamp":
                        timestamp = Timestamp.Parse(child.Value);
                        break;
                    case "contributor":
                        // If the user is deleted, don't create an object for it
                        if (child.Attribute("deleted") is null)
                            user = UserParser.Parse(child);

                        break;
                    case "minor":
                        isMinor = true;
                        break;
                    case "comment":
                        // If the comment is deleted, keep it null
                        if (child.Attribute("deleted") is null)
                            comment = child.Value;
                        break;
                    case "model":
                        contentModel = child.Value;
                        break;
                    case "format":
                        contentFormat = child.Value;
                        break;
                    case "text":
                        var bytesAttr = child.Attribute("bytes");
                        if (bytesAttr is not null)
                            length = uint.Parse(bytesAttr.Value);
                        // TODO: We could also load the revision text here
                        break;
                    case "sha1":
                    case "origin":
                        break; // Ignore without triggering warnings. What's origin?
                    default:
                        LogUnexpectedChildTag(logger, id, child.Name.LocalName);
                        break;
                }
            }

            List<string> missingTags = [];
            if (id is null) missingTags.Add("<id>");
            if (timestamp is null) missingTags.Add("<timestamp>");
            if (contentModel is null) missingTags.Add("<model>");
            if (contentFormat is null) missingTags.Add("<format>");
            if (length is null) missingTags.Add("length attribute on <text> tag");

            if (missingTags.Count > 0)
                throw new Exception($"Missing tags in <revision> (ID: {id}): {string.Join(", ", missingTags)}");

            return new Revision
            {
                Id = id!.Value,
                ParentId = parentId,
                Timestamp = timestamp!.Value,
                User = user,
                IsMinor = isMinor,
                Comment = comment,
                ContentModel = contentModel!,
                ContentFormat = contentFormat!,
                Length = length!.Value,
            };
        }

        [LoggerMessage(Level = LogLevel.Warning, Message = "Unexpected element in <revision> (ID: {id}): <{ElementName}>")]
        static partial void LogUnexpectedChildTag(ILogger logger, uint? id, string elementName);
    }
}
