using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NetBash.Sample.Commands
{
    [WebCommand("length", "counts chars in string")]
    public class LengthCommand : IWebCommand
    {
        public bool ReturnHtml
        {
            get { return false; }
        }

        public string Process(string[] args)
        {
            return string.Join(" ", args).Length.ToString();
        }
    }
}