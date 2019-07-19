# Balto

A simple convention based extention for Microsoft.Extensions.DependencyInjection

### Installing Balto

You should install Balto with NuGet:

> Install-Package Balto

Run this command from the NuGet Package Manager Console

### Using Balto

Scans assemblies and adds default implementations of interfaces to the container. You can add from multiple assemblies and you can freely control the lifestyle. To use, with an `IServiceCollection` instance:

```
services.Install(install => install
                .AddInstallers(new Installer())
                .InstallByConvention(c => c
                    .AddFromAssemblyOfThis<Startup>()));
```

A default implementation is a class with the same name as the interface (minus the I) that lives in the exact same namespace and assembly as the interface.