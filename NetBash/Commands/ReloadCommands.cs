using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetBash.Commands
{
    [WebCommand("load-commands", "Reloads command assemblies")]
    public class ReloadCommands : IWebCommand
    {
        public bool ReturnHtml
        {
            get { return false; }
        }

        public string Process(string[] args)
        {
            NetBash.Current.LoadCommands();
            return "Commands loaded, type 'help' to see them";
        }
    }
}
