using System;
using Balto;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Tests
{
    public class InstallersTests
    {
        [Fact]
        public void NullInstaller_Exception()
        {
            var serviceCollection = new ServiceCollection();

            Assert.Throws<ArgumentNullException>(() => serviceCollection.Install(null));
        }

        [Fact]
        public void Installer_OnlyIPing()
        {
            var serviceCollection = new ServiceCollection();
            
            serviceCollection.Install(install => install.AddInstallers(new Installer()));

            Assert.Single(serviceCollection);

            ServiceProvider provider = serviceCollection.BuildServiceProvider();
            IPing ping = provider.GetService<IPing>();
            Assert.NotNull(ping);
        }

        [Fact]
        public void InstallerAndConventions_OnlyOneIPing()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.Install(install => install
                .AddInstallers(new Installer())
                .InstallByConvention(c => c
                    .AddFromAssemblyOfThis<IPing>()));

            Assert.Single(serviceCollection);

            ServiceProvider provider = serviceCollection.BuildServiceProvider();
            IPing ping = provider.GetService<IPing>();
            Assert.NotNull(ping);
        }

        public class Installer : IInstaller
        {
            public void Install(IServiceCollection serviceCollection)
            {
                serviceCollection.AddTransient<IPing, Ping>();
            }
        }
    }
}
