NetBash is a drop in (think mvc mini profiler) command line for your web app.

Download from NuGet - **PM> Install-Package NetBash**

#### Set up
On application start call NetBash.Init() to initilize the routes. You can optionally set the Authorize action, this action is run to determine whether to show the console.

```csharp
protected void Application_Start()
{
	AreaRegistration.RegisterAllAreas();

	RegisterGlobalFilters(GlobalFilters.Filters);
	RegisterRoutes(RouteTable.Routes);

	NetBash.Init();
	NetBash.Settings.Authorize = (request) =>
		{
			return request.IsLocal;
		};
}
```

You also need to add the render includes code somewhere on your page (_Layout.cshtml is proabably easiest).

```
@NetBash.RenderIncludes()
```
	
#### Usage
NetBash commands are sent using this format - "[command name] [arg1] [arg2] etc". You can see which commands are currently loaded by typing "help". There are also a few keyboard shortcuts (which can be viewed with "shortcuts" the most useful being "`" to open and focust the console.
[todo link to blog post]

#### Creating a Command
NetBash will look for any implementation of the interface IWebCommand with a WebCommand attribute on first request. To create a command simply implement IWebCommand and add the WebCommand Attribtue.

```csharp
[WebCommand("length", "Returns number of characters in given arguments")]
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
```

This silly example just returns the number of chars in the arguments you pass. The first parameter of WebCommand is the name you use to invoke the command, the second is the description that shows up in help.

#### Commands

Over on the wiki is a list of commands people have made - https://github.com/lukencode/NetBash/wiki/Commands

