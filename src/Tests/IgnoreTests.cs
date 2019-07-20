using Balto;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Tests
{
    public class IgnoreTests
    {
        [Fact]
        public void ShouldIgnoreIPing()
        {
            var services = new ServiceCollection();

            services.Install(install => install
                .InstallByConvention(x => x
                    .AddFromAssemblyOfThis<AddByConventionsTests>()
                    .Ignore<IPing>()));

            Assert.Empty(services);
        }
    }
}
