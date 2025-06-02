using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Msz2001.MediaWikiDump.XmlDumpClient.Toolforge
{
    public class LogDumpReaderFactory(ILoggerFactory loggerFactory, string dumpDirectory = "/public/dumps/public")
    {
        public LogDumpReader CreateReader(string wikiDB)
        {
            return CreateReader(wikiDB, GetNewestDate(wikiDB));
        }

        public LogDumpReader CreateReader(string wikiDB, string dumpDate)
        {
            string wikiDirectory = Path.Combine(dumpDirectory, wikiDB);
            if (!Directory.Exists(wikiDirectory))
            {
                throw new Exception($"Dumps for `{wikiDB}` are not available.");
            }

            string dateDirectory = Path.Combine(wikiDirectory, dumpDate);
            if (!Directory.Exists(dateDirectory))
            {
                throw new Exception($"Dumps for `{dumpDate}` are not available in `{wikiDirectory}`.");
            }

            var dumpFile = $"{wikiDB}-{dumpDate}-pages-logging.xml.gz";
            var dumpPath = Path.Combine(dateDirectory, dumpFile);

            var fileStream = new FileStream(dumpPath, FileMode.Open, FileAccess.Read);
            var gzipStream = new GZipStream(fileStream, CompressionMode.Decompress);
            var xmlReader = XmlReader.Create(gzipStream);
            return new LogDumpReader(xmlReader, loggerFactory);
        }

        public string GetNewestDate(string wikiDB)
        {
            var wikiDirectory = Path.Combine(dumpDirectory, wikiDB);
            if (!Directory.Exists(wikiDirectory))
            {
                throw new Exception($"Dumps for `{wikiDB}` are not available.");
            }

            var subdirs = Directory.GetDirectories(wikiDirectory);
            var newest = subdirs
                .Select(name => new DirectoryInfo(name).Name)
                .OrderByDescending(name => name)
                .FirstOrDefault();

            if (newest != null)
            {
                return newest;
            }

            throw new Exception($"There are no dumps available for `{wikiDB}`.");
        }
    }
}
