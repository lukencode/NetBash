using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NetBash.Formatting;

namespace NetBash.Sample.Commands
{
    [WebCommand("spark", "Turns this: 1 2 3 4 5 into this: ▁▂▃▄▅▆▇")]
    public class SparkCommand : IWebCommand
    {
        public string Process(string[] args)
        {
            if (!args.Any())
                throw new ApplicationException("To use this command list values like this: 1 2 81 232 13");

            if (args.Count() == 1)
                return args.First().Spark();

            return args.Spark();
        }

        public bool ReturnHtml
        {
            get { return false; }
        }
    }
}