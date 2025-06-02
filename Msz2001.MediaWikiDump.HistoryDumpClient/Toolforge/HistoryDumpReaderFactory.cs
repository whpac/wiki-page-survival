using Microsoft.Extensions.Logging;

using Msz2001.MediaWikiDump.HistoryDumpClient.Reader;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Msz2001.MediaWikiDump.HistoryDumpClient.Toolforge
{
    public class HistoryDumpReaderFactory(ILoggerFactory loggerFactory, string dumpDirectory = "/public/dumps/public/other/mediawiki_history")
    {
        public HistoryDumpReader CreateReader(string wikiDB)
        {
            return CreateReader(wikiDB, GetNewestMonth());
        }

        public HistoryDumpReader CreateReader(string wikiDB, string dumpMonth)
        {
            string monthDirectory = Path.Combine(dumpDirectory, dumpMonth);
            if (!Directory.Exists(monthDirectory)) {
                throw new Exception($"Dumps for `{dumpMonth}` are not available.");
            }

            string wikiDirectory = Path.Combine(monthDirectory, wikiDB);
            if (!Directory.Exists(wikiDirectory)) {
                throw new Exception($"Dumps for `{wikiDB}` are not available in `{monthDirectory}`.");
            }

            var dumpFiles = Directory.GetFiles(wikiDirectory, "*.tsv.bz2");
            dumpFiles = [.. dumpFiles.OrderBy(name => name)];
            return new HistoryDumpReader(dumpFiles, loggerFactory);
        }

        public string GetNewestMonth()
        {
            var subdirs = Directory.GetDirectories(dumpDirectory);
            var newest = subdirs
                .Select(name => new DirectoryInfo(name).Name)
                .Where(name => DateTime.TryParseExact(name, "yyyy-MM",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out var _))
                .OrderByDescending(name => name)
                .FirstOrDefault();

            if (newest != null) {
                return newest;
            }

            throw new Exception("There are no dumps available.");
        }
    }
}
