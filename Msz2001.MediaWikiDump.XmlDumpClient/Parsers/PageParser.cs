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
    internal partial class PageParser(ILoggerFactory loggerFactory)
    {
        private readonly ILogger logger = loggerFactory.CreateLogger<PageParser>();
        private readonly RevisionParser RevisionParser = new(loggerFactory);

        internal Page Parse(XElement elem, SiteInfo siteInfo)
        {
            uint? id = null;
            Title? title = null;
            List<Revision> revisions = [];

            foreach (var child in elem.Elements())
            {
                switch (child.Name.LocalName)
                {
                    case "id":
                        id = uint.Parse(child.Value);
                        break;
                    case "title":
                        title = Title.FromText(child.Value, siteInfo);
                        break;
                    case "revision":
                        revisions.Add(RevisionParser.Parse(child));
                        break;
                    case "ns":
                    case "redirect":
                        // Ignore namespace, it's in title
                        // We don't use <redirect title="" /> for now
                        break;
                    default:
                        LogUnexpectedChildTag(logger, id, child.Name.LocalName);
                        break;
                }
            }

            List<string> missingTags = [];
            if (id is null) missingTags.Add("<id>");
            if (title is null) missingTags.Add("<title>");
            if (revisions.Count == 0) missingTags.Add("<revision>");

            if (missingTags.Count > 0)
                throw new Exception($"Missing tags in <page> (ID: {id}): {string.Join(", ", missingTags)}");

            return new Page(revisions)
            {
                Title = title!,
                Id = id!.Value,
            };
        }

        [LoggerMessage(Level = LogLevel.Warning, Message = "Unexpected element in <page> (ID: {id}): <{ElementName}>")]
        static partial void LogUnexpectedChildTag(ILogger logger, uint? id, string elementName);
    }
}
