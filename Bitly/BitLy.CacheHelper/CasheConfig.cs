using System;
using System.Configuration;

namespace BitLy.CacheHelper
{
    public class CasheConfig
    {
        public static int LinksDefaultCachePeriodInMinutes
        {
            get { return Convert.ToInt32(ConfigurationManager.AppSettings["LinksDefaultCachePeriodInMinutes"]); }
        }
    }
}
