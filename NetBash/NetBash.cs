using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;
using System.Web;
using NetBash.UI;
using System.Reflection;
using System.Web.Compilation;
using NetBash.Helpers;

namespace NetBash
{
    public partial class NetBash
    {
        private List<Type> _commandTypes;
        private static Type _attributeType = typeof(WebCommandAttribute);
        private static Type _interfaceType = typeof(IWebCommand);

        public static void Init()
        {
            NetBashHandler.RegisterRoutes();
        }

        private static Type[] TryGetTypes(Assembly assembly) 
		{
		    try 
    	    {
    	        return assembly.GetTypes();
    	    } 
    	    catch (ReflectionTypeLoadException) 
    	    {
    	        return Type.EmptyTypes;
    	    }
    	}

        internal void LoadCommands()
        {
              _interfaceType = typeof(IWebCommand);
              var assemblies = AssemblyLocator.GetAssemblies();

              var results = from a in assemblies
    	        	        from t in TryGetTypes(a)
    	      	            where _interfaceType.IsAssignableFrom(t)
    	         	        select t;

              _commandTypes = results.ToList();

            //if we still cant find any throw exception
            if (_commandTypes == null || !_commandTypes.Any())
                throw new ApplicationException("No commands found");
        }

        internal CommandResult Process(string commandText)
        {
            if (_commandTypes == null || !_commandTypes.Any())
                LoadCommands();

            if (string.IsNullOrWhiteSpace(commandText))
                throw new ArgumentNullException("commandText", "Command text cannot be empty");

            var split = commandText.SplitCommandLine();
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

            var result = new CommandResult() { IsHtml = webCommand.ReturnHtml };
            result.Result = webCommand.Process(split.Skip(1).ToArray());

            return result;
        }

        private CommandResult renderHelp()
        {
            var sb = new StringBuilder();

            sb.AppendLine("CLEAR           - Clears current console window");

            foreach (var t in _commandTypes)
            {
                var attr = (WebCommandAttribute)t.GetCustomAttributes(_attributeType, false).FirstOrDefault();

                if(attr == null)
                    continue;

                sb.AppendLine(string.Format("{0} - {1}", attr.Name.ToUpper().PadRight(15, ' '), attr.Description));
            }

            return new CommandResult { Result = sb.ToString(), IsHtml = false };
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
