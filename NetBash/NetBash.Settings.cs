using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Web;
using System.Reflection;

namespace NetBash
{
    public partial class NetBash
    {        
        /// <summary>
        /// Various configuration properties.
        /// </summary>
        public static class Settings
        {
            public static string RouteBasePath { get; set; }
            public static string Hash { get; private set; }
            public static string Version { get; private set; }
            public static string WelcomeMessage { get; set; }
            public static bool HiddenByDefault { get; set; }
            public static Func<HttpRequest, bool> Authorize { get; set; }

            static Settings()
            {
                byte[] contents = System.IO.File.ReadAllBytes(typeof(Settings).Assembly.Location);
                var md5 = System.Security.Cryptography.MD5.Create();
                Guid hash = new Guid(md5.ComputeHash(contents));
                Hash = hash.ToString();

                var v = Assembly.GetExecutingAssembly().GetName().Version;
                Version = string.Format("{0}.{1}.{2}", v.Major, v.Minor, v.Build);

                WelcomeMessage = string.Format("<strong><a href=\"http://github.com/lukencode/NetBash\" target=\"_blank\">NetBash {0}</a></strong> - Type \"help\" to list commands", Version);

                RouteBasePath = "~/";

                HiddenByDefault = false;
            }
        }
    }
}
