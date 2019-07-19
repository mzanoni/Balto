using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Balto.Infrastructure.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Balto
{
    public static class ServiceCollectionExtensions
    {
        public static void Install(this IServiceCollection serviceCollection, Action<InstallationConfiguration> options)
        {
            if (serviceCollection == null) throw new ArgumentNullException(nameof(serviceCollection));
            if (options == null) throw new ArgumentNullException(nameof(options));

            var config = new InstallationConfiguration();
            options.Invoke(config);

            config.Installers.ForEach(installer => installer.Install(serviceCollection));

            if (config.Conventions != null)
                serviceCollection.AddByConvention(config.Conventions);
        }

        private static void AddByConvention(this IServiceCollection serviceCollection, Action<ByConventionConfiguration> options)
        {
            if (serviceCollection == null) throw new ArgumentNullException(nameof(serviceCollection));
            if (options == null) throw new ArgumentNullException(nameof(options));

            var config = new ByConventionConfiguration();
            options.Invoke(config);

            foreach (Assembly assembly in config.Assemblies.Distinct())
            {
                serviceCollection.AddByConvention(assembly, config.Lifetime);
            }
        }

        private static void AddByConvention(this IServiceCollection serviceCollection, Assembly assembly, ServiceLifetime lifetime = ServiceLifetime.Singleton)
        {
            if (serviceCollection == null) throw new ArgumentNullException(nameof(serviceCollection));
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));

            List<Type> interfaces = assembly.ExportedTypes
                .Where(x => x.IsInterface && x.IsPublic)
                .ToList();
            List<Type> classes = assembly.GetTypes()
                .Where(x => !x.IsInterface && !x.IsAbstract)
                .ToList();

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

                serviceCollection.TryAdd(new ServiceDescriptor(@interface, implementations.First(), lifetime));
            }
        }
    }
}