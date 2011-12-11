
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NDesk.Options;

namespace NetBash
{
    public abstract class WebCommandWithArguments : IWebCommand
    {
        public string Process(string commandText)
        {
            if (Options == null)
                throw new ArgumentNullException("No Options Provided");

            var args = commandText.Split(' ');
            var extras = Options.Parse(args);

            return ProcessArguments(extras);
        }

        public abstract string ProcessArguments(List<string> extras);
        public abstract OptionSet Options { get; }
        public abstract bool ReturnHtml { get; }
    }
}
