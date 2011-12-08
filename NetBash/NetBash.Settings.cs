using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Web;

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
            public static string Version { get; private set; }
            public static Func<HttpRequest, bool> Authorize { get; set; }

            static Settings()
            {
                byte[] contents = System.IO.File.ReadAllBytes(typeof(Settings).Assembly.Location);
                var md5 = System.Security.Cryptography.MD5.Create();
                Guid hash = new Guid(md5.ComputeHash(contents));
                Version = hash.ToString();

                RouteBasePath = "~/";
            }
        }
    }
}
