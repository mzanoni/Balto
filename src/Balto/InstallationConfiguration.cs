using System;
using System.Collections.Generic;

namespace Balto
{
    public class InstallationConfiguration
    {
        internal List<IInstaller> Installers { get; } = new List<IInstaller>();
        
        internal Action<ByConventionConfiguration>? Conventions { get; set; }

        public InstallationConfiguration AddInstallers(params IInstaller[] installers)
        {
            if (installers == null) throw new ArgumentNullException(nameof(installers));

            Installers.AddRange(installers);

            return this;
        }

        public InstallationConfiguration InstallByConvention(Action<ByConventionConfiguration> options)
        {
            Conventions = options ?? throw new ArgumentNullException(nameof(options));

            return this;
        }
    }
}