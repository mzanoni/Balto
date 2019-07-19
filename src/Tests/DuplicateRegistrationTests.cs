using Balto;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Tests
{
    public class DuplicateRegistrationTests
    {
        [Fact]
        public void ShouldNotAddIPing()
        {
            var services = new ServiceCollection();

            services.AddTransient<IPing, Ping>();

            services.Install(install => install
                .InstallByConvention(x => x
                    .AddFromAssemblyOfThis<AddByConventionsTests>()));

            Assert.Single(services);
        }
    }
}
