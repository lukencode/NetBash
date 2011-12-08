using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetBash.Commands
{
    [WebCommand("time", "Displays server time")]
    public class TimeCommand : IWebCommand
    {
        public string Process(string commandText)
        {
            return DateTime.Now.ToString();
        }
    }
}
