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
        public string Process(string commandText)
        {
            int sleeptime = 1000;

            int.TryParse(commandText.Trim(), out sleeptime);

            Thread.Sleep(sleeptime);

            return string.Format("Slept for {0} milliseconds", sleeptime);
        }
    }
}