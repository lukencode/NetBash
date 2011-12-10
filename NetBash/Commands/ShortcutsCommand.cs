using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetBash.Commands
{
    [WebCommand("shortcuts", "Displays keyboard shortcuts")]
    public class ShortcutsCommand : IWebCommand
    {
        public string Process(string commandText)
        {
            var sb = new StringBuilder();

            sb.AppendFormat("{0} - {1}", "`".PadRight(7), "Opens and focuses NetBash");
            sb.AppendLine();
            sb.AppendFormat("{0} - {1}", "esc".PadRight(7), "Closes NetBash");
            sb.AppendLine();
            sb.AppendFormat("{0} - {1}", "↑".PadRight(7), "Puts last command in text box (only when focuses)");

            return sb.ToString();
        }
    }
}
