using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NetBash.Sample.Commands
{
    [WebCommand("length", "counts chars in string")]
    public class LengthCommand : IWebCommand
    {
        public string Process(string commandText)
        {
            return commandText.Length.ToString();
        }
    }
}