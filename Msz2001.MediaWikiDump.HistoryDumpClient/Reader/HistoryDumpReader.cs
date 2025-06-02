using ICSharpCode.SharpZipLib.BZip2;

using Microsoft.Extensions.Logging;

using Msz2001.MediaWikiDump.HistoryDumpClient.Entities;


namespace Msz2001.MediaWikiDump.HistoryDumpClient.Reader
{
    public partial class HistoryDumpReader(string[] dumpPaths, ILoggerFactory loggerFactory)
    {
        private readonly ILogger logger = loggerFactory.CreateLogger<HistoryDumpReader>();

        public HistoryDumpReader(string dumpPath, ILoggerFactory loggerFactory) : this([ dumpPath ], loggerFactory) { }

        public IEnumerable<HistoryDumpEntry> ReadEntries()
        {
            foreach (var dumpPath in dumpPaths) {
                LogFileOpen(logger, dumpPath);

                using var stream = new BZip2InputStream(File.OpenRead(dumpPath));
                using var reader = new StreamReader(stream);

                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    var parts = line.Split('\t');
                    if (parts.Length < 74)
                    {
                        LogIncompleteEntry(logger);
                        continue;
                    }

                    yield return CreateDumpEntry(parts);
                }
            }
        }

        private HistoryDumpEntry CreateDumpEntry(string[] fields)
        {
            var eventEntity = fields[1];

            return eventEntity switch
            {
                "revision" => new RevisionHistoryDumpEntry(fields),
                "page" => new PageHistoryDumpEntry(fields),
                "user" => new UserHistoryDumpEntry(fields),
                _ => throw new InvalidOperationException($"Unknown entry type: {eventEntity}"),
            };
        }

        [LoggerMessage(Level = LogLevel.Information, Message = "Opened file {fileName}")]
        static partial void LogFileOpen(ILogger logger, string fileName);

        [LoggerMessage(Level = LogLevel.Warning, Message = "Incomplete entry")]
        static partial void LogIncompleteEntry(ILogger logger);
    }
}
