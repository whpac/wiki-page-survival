using Msz2001.MediaWikiDump.XmlDumpClient.Parsers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Msz2001.MediaWikiDump.XmlDumpClient.Entities
{
    public class LogItem(string? rawParams) : IDumpEntry
    {
        public required uint Id { get; init; }
        public required Timestamp Timestamp { get; init; }
        public required string Type { get; init; }
        public required string Action { get; init; }
        public required User? User { get; init; }
        public required string? Comment { get; init; }
        public required Title? Title { get; init; }

        private object? _params = null;
        public object? Params {
            get
            {
                if (this._params is null && rawParams is not null)
                {
                    // Serialized params are always an array and will start with "a:"
                    // Deserialization is rather slow, so try to avoid it, unless we have to do it
                    if (rawParams.StartsWith("a:") && PhpDeserializer.TryDeserialize(rawParams, out var result))
                    {
                        this._params = result;
                    }
                    else
                    {
                        this._params = rawParams;
                    }
                }
                return _params;
            }
        }



        public override string ToString()
        {
            return $"LogItem {Id} {Timestamp} {User} {Comment} {Type} {Action} {Title} {Params}";
        }
    }
}
