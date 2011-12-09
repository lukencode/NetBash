using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetBash.Commands
{
    [WebCommand("load-commands", "Reloads command assemblies")]
    public class ReloadCommands : IWebCommand
    {
        public string Process(string commandText)
        {
            NetBash.Current.LoadCommands();
            return "Commands loaded, type 'help' to see them";
        }
    }
}
