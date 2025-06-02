using Msz2001.MediaWikiDump.XmlDumpClient.Entities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Msz2001.MediaWikiDump.XmlDumpClient.Parsers
{
    internal class NamespaceParser
    {

        internal static Namespace Parse(XElement elem)
        {
            var idAttribute = elem.Attribute("key") ??
                throw new Exception("Namespace element is missing 'key' attribute, which is required.");

            if (!int.TryParse(idAttribute.Value, out int id))
                throw new Exception("Namespace element has invalid 'key' attribute value. Expected a number.");

            string Name = elem.Value;

            var caseAttribute = elem.Attribute("case");
            CasingScheme? casing = null;
            if (caseAttribute is not null)
                casing = CasingSchemeParser.Parse(caseAttribute.Value);

            return new Namespace
            {
                Id = id,
                Name = Name,
                TitleCasing = casing,
            };
        }
    }
}
