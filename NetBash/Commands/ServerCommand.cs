using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace NetBash.Commands
{
    [WebCommand("server", "Displays server info")]
    public class ServerCommand : IWebCommand
    {
        public string Process(string commandText)
        {
            var sb = new StringBuilder();
            var context = HttpContext.Current;

            sb.AppendLine("Name - " + context.Server.MachineName);
            sb.AppendLine("IP - " + context.Request.ServerVariables["LOCAL_ADDR"]);
            sb.AppendLine("Domain - " + context.Request.ServerVariables["Server_Name"]);
            sb.AppendLine("Port - " + context.Request.ServerVariables["Server_Port"]);

            return sb.ToString();
        }
    }
}
