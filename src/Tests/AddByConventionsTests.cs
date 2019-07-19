using System;
using Balto;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Tests
{
    public class AddByConventionsTests
    {
        private readonly IServiceProvider _provider;

        public AddByConventionsTests()
        {
            var services = new ServiceCollection();

            services.Install(x => x
                .InstallByConvention(c => c
                    .AddFromAssemblyOfThis<AddByConventionsTests>()));

            _provider = services.BuildServiceProvider();

        }

        [Fact]
        public void ShouldResolvePing()
        {
            IPing ping = _provider.GetService<IPing>();
            Assert.NotNull(ping);
        }

        [Fact]
        public void ShouldNotResolvePong()
        {
            IPong pong = _provider.GetService<IPong>();
            Assert.Null(pong);
        }

        [Fact]
        public void ShouldNotResolveZong()
        {
            IZong zong = _provider.GetService<IZong>();
            Assert.Null(zong);
        }
    }
}
