using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Balto
{
    public class ByConventionConfiguration
    {
        internal ServiceLifetime Lifetime { get; private set; } = ServiceLifetime.Scoped;
        internal List<Assembly> Assemblies { get; } = new();
        internal List<Type> IgnoredTypes { get; } = new();

        public ByConventionConfiguration AddFromAssemblyOfThis<T>()
        {
            Assemblies.Add(typeof(T).Assembly);

            return this;
        }

        public ByConventionConfiguration AsSingleton()
        {
            Lifetime = ServiceLifetime.Singleton;

            return this;
        }

        public ByConventionConfiguration AsScoped()
        {
            Lifetime = ServiceLifetime.Scoped;

            return this;
        }

        public ByConventionConfiguration AsTransient()
        {
            Lifetime = ServiceLifetime.Transient;

            return this;
        }

        public ByConventionConfiguration Ignore<T>()
        {
            IgnoredTypes.Add(typeof(T));

            return this;
        }
    }
}