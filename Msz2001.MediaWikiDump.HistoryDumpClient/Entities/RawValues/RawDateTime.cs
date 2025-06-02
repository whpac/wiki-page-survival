using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Msz2001.MediaWikiDump.HistoryDumpClient.Entities.RawValues
{
    internal struct RawDateTime(string value)
    {
        private DateTime? parsedValue;
        private bool parsed = false;

        public DateTime? Value
        {
            get
            {
                if (!parsed)
                {
                    if (DateTime.TryParseExact(value, "yyyy-MM-dd HH:mm:ss.f", CultureInfo.InvariantCulture,
                        DateTimeStyles.AssumeUniversal, out DateTime result))
                    {
                        parsedValue = result;
                    }
                    parsed = true;
                }
                return parsedValue;
            }
        }
    }
}
