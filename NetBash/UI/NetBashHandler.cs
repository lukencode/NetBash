using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Routing;
using System.Web.Script.Serialization;

namespace NetBash.UI
{
    public class NetBashHandler : IRouteHandler, IHttpHandler
    {
        internal static HtmlString RenderIncludes()
        {
            if (NetBash.Settings.Authorize != null && !NetBash.Settings.Authorize(HttpContext.Current.Request))
                return new HtmlString(""); //not authorized dont render

            const string format =
@"<link rel=""stylesheet"" type=""text/css"" href=""{0}netbash-style-css?v={1}"">
<script type=""text/javascript"">
    if (!window.jQuery) document.write(unescape(""%3Cscript src='{0}netbash-jquery-js' type='text/javascript'%3E%3C/script%3E""));
    if(!window.key) document.write(unescape(""%3Cscript src='{0}netbash-keymaster-js' type='text/javascript'%3E%3C/script%3E""));
</script>
<script type=""text/javascript"" src=""{0}netbash-script-js?v={1}""></script>
<script type=""text/javascript"">var netbash = new NetBash(jQuery, window, {{ welcomeMessage: '{2}', version: '{3}', isHidden: {4}, routeBasePath: '{5}' }});</script>";

            var result = "";
            result = string.Format(format, 
                                   ensureTrailingSlash(VirtualPathUtility.ToAbsolute(NetBash.Settings.RouteBasePath)), 
                                   NetBash.Settings.Hash, NetBash.Settings.WelcomeMessage, 
                                   NetBash.Settings.Version, 
                                   NetBash.Settings.HiddenByDefault.ToString().ToLower(), 
                                   NetBash.Settings.RouteBasePath.Replace("~", ""));

            return new HtmlString(result);
        }

        internal static void RegisterRoutes()
        {
            var urls = new[] 
            {  
                "netbash",
                "netbash-export",
                "netbash-jquery-js",
                "netbash-keymaster-js",
                "netbash-style-css",
                "netbash-script-js"
            };

            var routes = RouteTable.Routes;
            var handler = new NetBashHandler();
            var prefix = ensureTrailingSlash((NetBash.Settings.RouteBasePath ?? "").Replace("~/", ""));

            using (routes.GetWriteLock())
            {
                foreach (var url in urls)
                {
                    var route = new Route(prefix + url, handler)
                    {
                        // we have to specify these, so no MVC route helpers will match, e.g. @Html.ActionLink("Home", "Index", "Home")
                        Defaults = new RouteValueDictionary(new { controller = "NetBashHandler", action = "ProcessRequest" })
                    };

                    // put our routes at the beginning, like a boss
                    routes.Insert(0, route);
                }
            }
        }

        private static string ensureTrailingSlash(string input)
        {
            if (string.IsNullOrEmpty(input)) return "";
            return Regex.Replace(input, "/+$", "") + "/";
        }

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return this; // elegant? I THINK SO.
        }

        /// <summary>
        /// Try to keep everything static so we can easily be reused.
        /// </summary>
        public bool IsReusable
        {
            get { return true; }
        }

        /// <summary>
        /// Returns either includes' css/javascript or results' html.
        /// </summary>
        public void ProcessRequest(HttpContext context)
        {
            string output;
            string path = context.Request.AppRelativeCurrentExecutionFilePath;

            switch (Path.GetFileNameWithoutExtension(path).ToLower())
            {
                case "netbash-jquery-js":
                case "netbash-script-js":
                case "netbash-style-css":
                case "netbash-keymaster-js":
                    output = Includes(context, path);
                    break;

                case "netbash":
                    output = RenderCommand(context);
                    break;

                case "netbash-export":
                    output = ExportCommand(context);
                    break;

                default:
                    output = NotFound(context);
                    break;
            }

            context.Response.Write(output);
        }

        private static string RenderCommand(HttpContext context)
        {
            if (NetBash.Settings.Authorize != null && !NetBash.Settings.Authorize(HttpContext.Current.Request))
                throw new UnauthorizedAccessException();

            string commandResponse;
            var success = true;
            var isHtml = true;

            try
            {
                var result = NetBash.Current.Process(context.Request.Params["Command"]);
                if (result.IsHtml)
                {
                    //on your way
                    commandResponse = result.Result;
                }
                else
                {
                    //encode it
                    commandResponse = HttpUtility.HtmlEncode(result.Result);
                }
                isHtml = result.IsHtml;
            }
            catch (Exception ex)
            {
                success = false;
                commandResponse = ex.Message;
            }

            var response = new { Success = success, IsHtml = isHtml, Content = commandResponse };

            context.Response.ContentType = "application/json";

            var sb = new StringBuilder();
            var serializer = new JavaScriptSerializer();
            serializer.Serialize(response, sb);

            return sb.ToString();
        }

        private static string ExportCommand(HttpContext context)
        {
            if (NetBash.Settings.Authorize != null && !NetBash.Settings.Authorize(HttpContext.Current.Request))
                throw new UnauthorizedAccessException();

            try
            {
                var result = NetBash.Current.Process(context.Request.Params["Command"]);
                context.Response.ContentType = "application/octet-stream";
                context.Response.AddHeader(
                    "Content-Disposition",
                    "attachment; filename=" + (string.IsNullOrEmpty(result.FileName) ? "result.txt" : result.FileName));
                return result.Result;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Handles rendering static content files.
        /// </summary>
        private static string Includes(HttpContext context, string path)
        {
            var response = context.Response;
            var extension = "." + path.Split('-').LastOrDefault();

            switch (extension)
            {
                case ".js":
                    response.ContentType = "application/javascript";
                    break;
                case ".css":
                    response.ContentType = "text/css";
                    break;
                case ".gif":
                    response.ContentType = "image/gif";
                    break;
                default:
                    return NotFound(context);
            }

            var cache = response.Cache;
            cache.SetCacheability(System.Web.HttpCacheability.Public);
            cache.SetExpires(DateTime.Now.AddDays(7));
            cache.SetValidUntilExpires(true);

            var embeddedFile = Path.GetFileName(path).Replace("netbash-", "") + extension;
            return GetResource(embeddedFile);
        }

        private static string GetResource(string filename)
        {
            string result;

            if (!_ResourceCache.TryGetValue(filename, out result))
            {
                using (var stream = typeof(NetBashHandler).Assembly.GetManifestResourceStream("NetBash.UI." + filename))
                using (var reader = new StreamReader(stream))
                {
                    result = reader.ReadToEnd();
                }

                _ResourceCache[filename] = result;
            }

            return result;
        }

        /// <summary>
        /// Embedded resource contents keyed by filename.
        /// </summary>
        private static readonly Dictionary<string, string> _ResourceCache = new Dictionary<string, string>();

        /// <summary>
        /// Helper method that sets a proper 404 response code.
        /// </summary>
        private static string NotFound(HttpContext context, string contentType = "text/plain", string message = null)
        {
            context.Response.StatusCode = 404;
            context.Response.ContentType = contentType;

            return message;
        }
    }
}
