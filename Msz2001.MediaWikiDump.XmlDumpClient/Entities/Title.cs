using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Msz2001.MediaWikiDump.XmlDumpClient.Entities
{
    public class Title
    {
        public readonly string PageName;
        public readonly Namespace Namespace;

        public Title(Namespace ns, string pageName)
        {
            Namespace = ns;
            PageName = pageName;
        }

        public static Title FromText(string text, SiteInfo siteInfo)
        {
            string nsText, baseTitle;

            if (text.Contains(':'))
            {
                var portions = text.Split(':');
                nsText = portions[0];
                baseTitle = portions[1];

                foreach (var ns in siteInfo.Namespaces.Values)
                {
                    if (ns.Name == nsText)
                        return new Title(ns, baseTitle);
                }
            }

            return new Title(siteInfo.GetMainNamespace(), text);
        }

        public override string ToString()
        {
            if (Namespace.Id == 0)
                return PageName;
            return Namespace.Name + ":" + PageName;
        }
    }
}
