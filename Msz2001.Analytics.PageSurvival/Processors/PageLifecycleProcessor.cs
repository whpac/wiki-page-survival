using Microsoft.Extensions.Logging;

using Msz2001.Analytics.PageSurvival.Data;
using Msz2001.MediaWikiDump.XmlDumpClient;
using Msz2001.MediaWikiDump.XmlDumpClient.Entities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Msz2001.Analytics.PageSurvival.Processors
{
    internal partial class PageLifecycleProcessor(LogDumpReader logReader, ILoggerFactory loggerFactory)
    {
        private readonly ILogger logger = loggerFactory.CreateLogger<PageLifecycleProcessor>();

        private Dictionary<string, PageEvents> existingPages = [];
        private List<PageEvents> deletedPages = [];
        private Dictionary<uint, (Timestamp, bool)> userRegistrations = []; // (userId, (registrationDate, isCrossWiki))

        public IEnumerable<PageEvents> Process()
        {
            int i = 0;
            foreach (var logItem in logReader)
            {
                i++;
                if (i % 5000 == 0)
                    DisplayProgressMessage(i);

                if (i % 1_000_000 == 0)
                {
                    var cutoffDate = logItem.Timestamp - TimeSpan.FromDays(720);
                    RemoveOldRegistrations(cutoffDate);
                }

                if (logItem.Title is null) continue;

                if (logItem.Type == "create" && logItem.Title.Namespace.Id == 0)
                {
                    RecordPageCreation(logItem.Title.ToString(), logItem.Timestamp, logItem.User);
                }
                else if (logItem.Type == "move")
                {
                    var oldTitle = logItem.Title.ToString();
                    var newTitle = "";
                    if (logItem.Params is string paramsText)
                    {
                        newTitle = paramsText;
                    }
                    else if (logItem.Params is Dictionary<object, object?> paramsDict)
                    {
                        newTitle = paramsDict.GetValueOrDefault("4::target") as string ?? "";
                    }

                    if (string.IsNullOrEmpty(newTitle)) continue;

                    var isOldTitleMainNS = logItem.Title.Namespace.Id == 0;
                    var isNewTitleMainNS = !newTitle.Contains(':');
                    if (logReader.SiteInfo is not null)
                    {
                        var newTitleObject = Title.FromText(newTitle, logReader.SiteInfo);
                        isNewTitleMainNS = newTitleObject.Namespace.Id == 0;
                    }

                    if (isOldTitleMainNS == isNewTitleMainNS)
                    {
                        RecordPageMove(oldTitle, newTitle);
                    }
                    else if (isOldTitleMainNS && !isNewTitleMainNS)
                    {
                        RecordPageDeletion(oldTitle, logItem.Timestamp);
                    }
                    else if (!isOldTitleMainNS && isNewTitleMainNS)
                    {
                        RecordPageCreation(newTitle, logItem.Timestamp, logItem.User);
                    }
                }
                else if (logItem.Type == "delete" && logItem.Action == "delete")
                {
                    RecordPageDeletion(logItem.Title.ToString(), logItem.Timestamp);
                }
                else if (logItem.Type == "newusers")
                {
                    uint userId = 0;
                    string? paramsString = logItem.Params as string;
                    if (logItem.Params is null || paramsString == "")
                    {
                        userId = logItem.User?.Id ?? 0;
                    }
                    else if (paramsString is not null && uint.TryParse(paramsString, out userId))
                    { }
                    else if (logItem.Params is Dictionary<object, object?> paramsDict)
                    {
                        if (paramsDict.TryGetValue("4::userid", out var userIdObj) && userIdObj is int id)
                        {
                            userId = (uint)id;
                        }
                    }

                    if (userId == 0) continue;
                    userRegistrations[userId] = (logItem.Timestamp, logItem.Action == "autocreate");
                }
            }

            LogProcessingEnd(logger, i, Environment.WorkingSet / (1024.0 * 1024.0));

            return deletedPages.Concat(existingPages.Values);
        }

        void RecordPageCreation(string title, Timestamp date, User? user)
        {
            Timestamp? userRegistration = null;
            bool isCrossWiki = false;

            if (userRegistrations.TryGetValue(user?.Id ?? 0, out var registration))
            {
                userRegistration = registration.Item1;
                isCrossWiki = registration.Item2;
            }

            var creatorType = user switch
            {
                null => PageEvents.CreatorType.Unknown,
                { Name: null } => PageEvents.CreatorType.Anonymous,
                _ => isCrossWiki ? PageEvents.CreatorType.CrossWikiUser : PageEvents.CreatorType.User,
            };

            var page = new PageEvents(date)
            {
                CreatorRegistration = userRegistration,
                Creator = creatorType
            };

            existingPages[title] = page;
        }

        void RecordPageMove(string oldTitle, string newTitle)
        {
            if (!existingPages.TryGetValue(oldTitle, out var page)) return;

            existingPages.Remove(oldTitle);
            existingPages[newTitle] = page;
        }

        void RecordPageDeletion(string title, Timestamp date)
        {
            if (!existingPages.TryGetValue(title, out var page)) return;

            page.Deleted = date;
            deletedPages.Add(page);
            existingPages.Remove(title);
        }

        void DisplayProgressMessage(int iteration)
        {
            var memory = Environment.WorkingSet / (1024.0 * 1024.0);
            LogProcessingProgress(logger, iteration, memory);
        }

        void RemoveOldRegistrations(Timestamp cutoffDate)
        {
            userRegistrations = userRegistrations
                .Where(kvp => kvp.Value.Item1 >= cutoffDate || kvp.Value.Item2)
                .ToDictionary();
        }

        [LoggerMessage(Level = LogLevel.Information, Message = "Processed {Iteration} entries, using {Memory:F2} MB")]
        static partial void LogProcessingProgress(ILogger logger, int iteration, double memory);

        [LoggerMessage(Level = LogLevel.Information, Message = "Processing completed after {Iteration} entries, using {Memory:F2} MB")]
        static partial void LogProcessingEnd(ILogger logger, int iteration, double memory);
    }
}
