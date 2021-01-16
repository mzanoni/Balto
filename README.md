# Balto

Convention based assembly scanning and installers for Microsoft.Extensions.DependencyInjection.

### Installing Balto

You should install Balto with NuGet:

> Install-Package Balto

Run this command from the NuGet Package Manager Console to install the NuGet package.

### Using Balto

Balto can scan assemblies and adds default implementations of interfaces to the container. 
A default implementation is a class with the same name as the interface (minus the I) that lives in the exact same namespace and assembly as the interface.
You can add from multiple assemblies and you can freely control the lifestyle - the default lifestyle is `Singleton`. Use with an `IServiceCollection` instance:

```csharp
services.Install(install => install
                .InstallByConvention(c => c
                    .AddFromAssemblyOfThis<Startup>()));
```

This will register the following default convention implementation in the ServiceCollection:

```csharp
public interface IPing
{
	string PingIt();
}

class Ping : IPing
{
	public string PingIt()
	{
		return "Ping!";
	}
}
```


Balto also support simple installers to keep the `Startup`-class light. Use with an `IServiceCollection` instance:

```csharp
services.Install(install => install
                .AddInstallers(new Installer()));

public class Installer : IInstaller
{
	public void Install(IServiceCollection serviceCollection)
	{
		serviceCollection.AddTransient<IPing, Ping>();
	}
}				
```

You can add as many installers as you like.
