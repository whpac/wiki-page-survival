using Microsoft.Extensions.Logging;

using Msz2001.MediaWikiDump.XmlDumpClient.Entities;
using Msz2001.MediaWikiDump.XmlDumpClient.Parsers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Msz2001.MediaWikiDump.XmlDumpClient
{
    public abstract partial class DumpReader<TContent>(XmlReader XmlReader, ILoggerFactory loggerFactory) where TContent : IDumpEntry
    {
        private readonly ILogger logger = loggerFactory.CreateLogger<DumpReader<TContent>>();
        private readonly SiteInfoParser SiteInfoParser = new(loggerFactory);
        public SiteInfo? SiteInfo { get; private set; }

        public IEnumerable<TContent> GetItems()
        {
            Queue<XElement> contentElems = new();
            SiteInfo? siteinfo = null;
            XElement? element;

            while (XmlReader.Read())
            {
                if (XmlReader.NodeType != XmlNodeType.Element)
                    continue;

                switch (XmlReader.LocalName)
                {
                    case "mediawiki":
                        continue; // Do nothing, this tag is expected. We want to get inside it

                    case "siteinfo":
                        element = XNode.ReadFrom(XmlReader) as XElement ??
                            throw new Exception("Failed to read siteinfo element: it's not an XML element");

                        siteinfo = SiteInfoParser.Parse(element);

                        // If we have any XML elems that were held, it's time to process them
                        foreach (var elem in contentElems)
                        {
                            TContent? item;
                            try
                            {
                                item = ProcessContentElement(elem, siteinfo);
                            }
                            catch (Exception ex)
                            {
                                LogProcessingFailed(logger, element.Name.LocalName, ex);
                                continue;
                            }

                            if (item is not null)
                                yield return item;
                        }
                        break;

                    default:
                        element = XNode.ReadFrom(XmlReader) as XElement;
                        if (element is null)
                            continue;

                        if (siteinfo is not null)
                        {
                            TContent? item;
                            try
                            {
                                item = ProcessContentElement(element, siteinfo);
                            }
                            catch (Exception ex)
                            {
                                LogProcessingFailed(logger, element.Name.LocalName, ex);
                                continue;
                            }

                            if (item is not null)
                                yield return item;
                        }
                        else
                        {
                            contentElems.Enqueue(element);
                        }
                        break;
                }
            }
        }

        public IEnumerator<TContent> GetEnumerator()
        {
            return GetItems().GetEnumerator();
        }

        protected abstract TContent? ProcessContentElement(XElement element, SiteInfo siteinfo);

        [LoggerMessage(Level = LogLevel.Warning, Message = "Failed to process element <{TagName}>")]
        static partial void LogProcessingFailed(ILogger logger, string tagName, Exception ex);
    }
}
