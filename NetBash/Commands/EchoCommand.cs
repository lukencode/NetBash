using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetBash.Commands
{
    [WebCommand("echo", "says what you says")]
    public class EchoCommand : IWebCommand
    {
        public string Process(string commandText)
        {
            return commandText;
        }
    }
}
