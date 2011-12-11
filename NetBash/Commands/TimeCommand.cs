using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetBash.Commands
{
    [WebCommand("time", "Displays server time")]
    public class TimeCommand : IWebCommand
    {
        public bool ReturnHtml
        {
            get { return false; }
        }

        public string Process(string[] args)
        {
            return DateTime.Now.ToString();
        }
    }
}
