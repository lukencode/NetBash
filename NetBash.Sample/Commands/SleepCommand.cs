using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;

namespace NetBash.Sample.Commands
{
    [WebCommand("sleep", "Sleeps for given amount of time")]
    public class SleepCommand : IWebCommand
    {
        public bool ReturnHtml
        {
            get { return false; }
        }

        public string Process(string[] args)
        {
            int sleeptime = 1000;

            if(args.Any())
                int.TryParse(args[0].Trim(), out sleeptime);

            Thread.Sleep(sleeptime);

            return string.Format("Slept for {0} milliseconds", sleeptime);
        }
    }
}