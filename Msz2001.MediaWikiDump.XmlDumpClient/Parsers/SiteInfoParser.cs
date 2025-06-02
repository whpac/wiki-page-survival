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
    internal partial class SiteInfoParser(ILoggerFactory loggerFactory)
    {
        private readonly ILogger logger = loggerFactory.CreateLogger<SiteInfoParser>();

        internal SiteInfo Parse(XElement elem)
        {
            string? siteName = null, dbName = null, homeUrl = null, software = null;
            CasingScheme? defaultCasing = null;
            List<Namespace> namespaces = [];

            foreach (var child in elem.Elements())
            {
                switch (child.Name.LocalName)
                {
                    case "sitename":
                        siteName = child.Value;
                        break;
                    case "dbname":
                        dbName = child.Value;
                        break;
                    case "base":
                        homeUrl = child.Value;
                        break;
                    case "generator":
                        software = child.Value;
                        break;
                    case "case":
                        defaultCasing = CasingSchemeParser.Parse(child.Value);
                        break;
                    case "namespaces":
                        foreach (var nsElem in child.Descendants())
                        {
                            var ns = NamespaceParser.Parse(nsElem);
                            namespaces.Add(ns);
                        }
                        break;
                    default:
                        LogUnexpectedChildTag(logger, child.Name.LocalName);
                        break;
                }
            }

            List<string> missingTags = [];
            if (siteName is null) missingTags.Add("<sitename>");
            if (dbName is null) missingTags.Add("<dbname>");
            if (homeUrl is null) missingTags.Add("<base>");
            if (software is null) missingTags.Add("<generator>");
            if (defaultCasing is null) missingTags.Add("<case>");

            if (missingTags.Count > 0)
                throw new Exception($"Missing required tags in <siteinfo>: {string.Join(", ", missingTags)}");

            return new SiteInfo(namespaces)
            {
                SiteName = siteName!,
                DatabaseName = dbName!,
                HomeUrl = homeUrl!,
                Software = software!,
                DefaultCasing = defaultCasing!.Value,
            };
        }

        [LoggerMessage(Level = LogLevel.Warning, Message = "Unexpected element in <siteinfo>: <{ElementName}>")]
        static partial void LogUnexpectedChildTag(ILogger logger, string elementName);
    }
}
