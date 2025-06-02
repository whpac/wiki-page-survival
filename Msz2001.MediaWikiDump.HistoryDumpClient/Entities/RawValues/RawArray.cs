using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Msz2001.MediaWikiDump.HistoryDumpClient.Entities.RawValues
{
    internal struct RawArray(string value)
    {
        private string[] parsedValue = [];
        private bool parsed = string.IsNullOrEmpty(value); // short-circuit for empty arrays

        public string[] Value
        {
            get
            {
                if (!parsed)
                {
                    parsedValue = value.Split(',');
                    parsed = true;
                }
                return parsedValue;
            }
        }
    }
}
