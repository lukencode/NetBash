using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;
using System.Web;
using NetBash.UI;

namespace NetBash
{
    public partial class NetBash
    {
        private List<Type> _commandTypes;
        private static Type _attributeType = typeof(WebCommandAttribute);
        private static Type _interfaceType = typeof(IWebCommand);

        public void Init()
        {
            NetBashHandler.RegisterRoutes();

            _interfaceType = typeof(IWebCommand);
            var results = from a in AppDomain.CurrentDomain.GetAssemblies().ToList()
                          from t in a.GetTypes()
                          where _interfaceType.IsAssignableFrom(t)
                          select t;

            _commandTypes = results.ToList();
        }

        public string Process(string commandText)
        {
            if (string.IsNullOrWhiteSpace(commandText))
                throw new ArgumentNullException("Command text cannot be empty");

            var split = commandText.Split(' ');
            var command = (split.FirstOrDefault() ?? commandText).ToLower();

            if (command == "help")
                return renderHelp();

            var commandType = (from c in _commandTypes
                              let attr = (WebCommandAttribute)c.GetCustomAttributes(_attributeType, false).FirstOrDefault()
                              where attr != null
                              && attr.Name.ToLower() == command
                              select c).FirstOrDefault();

            if(commandType == null)
                throw new ArgumentException(string.Format("Command '{0}' not found", command.ToUpper()));

            var webCommand = (IWebCommand)Activator.CreateInstance(commandType);
            return webCommand.Process(string.Join(" ", split.Skip(1)));
        }

        private string renderHelp()
        {
            var sb = new StringBuilder();

            foreach (var t in _commandTypes)
            {
                var attr = (WebCommandAttribute)t.GetCustomAttributes(_attributeType, false).FirstOrDefault();

                if(attr == null)
                    continue;

                sb.AppendLine(string.Format("{0} - {1}", attr.Name.ToUpper(), attr.Description));
            }

            return sb.ToString();
        }

        public static IHtmlString RenderIncludes()
        {
            return NetBashHandler.RenderIncludes();
        }

        #region singleton
        static readonly NetBash instance= new NetBash();

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static NetBash()
        {
        }

        NetBash()
        {
        }

        public static NetBash Current
        {
            get
            {
                return instance;
            }
        }
        #endregion
    }
}
