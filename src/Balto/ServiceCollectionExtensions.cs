using System;
using System.Linq;
using System.Reflection;
using Balto.Infrastructure.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Balto
{
    /// <summary>
    /// Extensions to scan for default implementations of interfaces and registers them and install by installers.
    /// - Can scan for all default implementations of interfaces and registers them as <see cref="ServiceLifetime.Singleton"/> or what you specify
    /// - Installs all dependencies configured via <see cref="IInstaller"/>
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers dependencies via installers and/or default conventions
        /// </summary>
        /// <param name="serviceCollection">Service collection</param>
        /// <param name="options">Configuration for what to install in the Service collection</param>        
        /// <returns>Service collection</returns>
        public static IServiceCollection Install(this IServiceCollection serviceCollection, Action<InstallationConfiguration> options)
        {
            if (serviceCollection == null) throw new ArgumentNullException(nameof(serviceCollection));
            if (options == null) throw new ArgumentNullException(nameof(options));

            var config = new InstallationConfiguration();
            options.Invoke(config);

            config.Installers
                .Distinct()
                .ToList()
                .ForEach(installer => installer.Install(serviceCollection));

            if (config.Conventions != null)
                serviceCollection.AddByConvention(config.Conventions);

            return serviceCollection;
        }

        private static void AddByConvention(this IServiceCollection serviceCollection, Action<ByConventionConfiguration> options)
        {
            if (serviceCollection == null) throw new ArgumentNullException(nameof(serviceCollection));
            if (options == null) throw new ArgumentNullException(nameof(options));

            var config = new ByConventionConfiguration();
            options.Invoke(config);

            foreach (Assembly assembly in config.Assemblies.Distinct())
            {
                serviceCollection.AddByConvention(assembly, config);
            }
        }

        private static void AddByConvention(this IServiceCollection serviceCollection, Assembly assembly, ByConventionConfiguration configuration)
        {
            if (serviceCollection == null) throw new ArgumentNullException(nameof(serviceCollection));
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            Type[] interfaces = assembly.ExportedTypes
                .Where(x => x.IsInterface && x.IsPublic && !configuration.IgnoredTypes.Contains(x))
                .ToArray();
            Type[] classes = assembly.GetTypes()
                .Where(x => !x.IsInterface && !x.IsAbstract && !configuration.IgnoredTypes.Contains(x))
                .ToArray();

            foreach (Type @interface in interfaces)
            {
                Type[] implementations = classes
                    .Where(@class =>
                        @interface.IsAssignableFrom(@class) &&
                        @interface.Assembly.Equals(@class.Assembly) &&
                        @interface.IsInExactNamespace(@class) &&
                        @interface.Name.Equals($"I{@class.Name}"))
                    .ToArray();

                if (implementations.Length == 0) continue;
                if (implementations.Length > 1) continue;

                serviceCollection.TryAdd(new ServiceDescriptor(@interface, implementations.First(), configuration.Lifetime));
            }
        }
    }
}