using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NDesk.Options;
using System.Text;
using System.IO;

namespace NetBash.Sample.Commands
{
    //implemented example from http://tirania.org/blog/archive/2008/Oct-14.html using NDesk.Options

    [WebCommand("greet", "Usage: greet [OPTIONS]+ message")]
    public class GreetCommand : IWebCommand
    {
        bool show_help = false;
        List<string> names = new List<string>();
        int repeat = 1;
        int verbosity;

        public bool ReturnHtml
        {
            get { return false; }
        }

        public string Process(string[] args)
        {
            var sw = new StringWriter();

            var p = new OptionSet() {
                { "n|name=", "the name of someone to greet.",
                  v => names.Add (v) },
                { "r|repeat=", "the number of times to repeat the greeting.",
                  (int v) => repeat = v },
                { "v", "increase debug message verbosity",
                  v => { if (v != null) ++verbosity; } },
                { "h|help",  "show this message and exit", 
                  v => show_help = v != null },
            };

            List<string> extras;
            try
            {
                extras = p.Parse(args);
            }
            catch (OptionException e)
            {
                sw.Write("greet: ");
                sw.WriteLine(e.Message);
                sw.WriteLine("Try `greet --help' for more information.");
                return sw.ToString();
            }

            if (show_help)
                return showHelp(p);
            
            string message;
            if (extras.Count > 0)
            {
                message = string.Join(" ", extras.ToArray());
                debug(sw, "Using new message: {0}", message);
            }
            else
            {
                message = "Hello {0}!";
                debug(sw, "Using default message: {0}", message);
            }

            foreach (string name in names)
            {
                for (int i = 0; i < repeat; ++i)
                    sw.WriteLine(message, name);
            }

            return sw.ToString();
        }

        private string showHelp(OptionSet p)
        {
            var sb = new StringWriter();

            sb.WriteLine("Usage: greet [OPTIONS]+ message");
            sb.WriteLine("Greet a list of individuals with an optional message.");
            sb.WriteLine("If no message is specified, a generic greeting is used.");
            sb.WriteLine();
            sb.WriteLine("Options:");

            p.WriteOptionDescriptions(sb);

            return sb.ToString();
        }
        
        private void debug(StringWriter sw, string format, params object[] args)
        {
            if (verbosity > 0)
            {
                sw.Write("# ");
                sw.WriteLine(format, args);
            }
        }
    }
}