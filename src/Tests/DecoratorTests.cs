using System;
using Balto;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Tests
{
    public class DecoratorTests
    {
        private readonly IServiceProvider _provider;

        public DecoratorTests()
        {
            var services = new ServiceCollection();
            
            services.AddSingleton<IDecorator, StandardDecorator>();
            services.AddDecorator<IDecorator, WrapperDecorator>();
            services.AddDecorator<IDecorator, MegaWrapperDecorator>();

            _provider = services.BuildServiceProvider();
        }

        [Fact]
        public void ShouldReturnInstance_ResultShouldBeWrappedInRegisteredOrder()
        {
            IDecorator decorator = _provider.GetService<IDecorator>();

            Assert.NotNull(decorator);
            Assert.Equal("Mega Wrapped Dummy", decorator.DecoratedResult);
        }

        public interface IDecorator
        {
            public string DecoratedResult { get; }
        }

        public class WrapperDecorator : IDecorator
        {
            private readonly IDecorator _decoratedInstance;

            public WrapperDecorator(IDecorator decoratedInstance)
            {
                _decoratedInstance = decoratedInstance;
            }

            public string DecoratedResult => "Wrapped " + _decoratedInstance.DecoratedResult;
        }

        public class MegaWrapperDecorator : IDecorator
        {
            private readonly IDecorator _decoratedInstance;

            public MegaWrapperDecorator(IDecorator decoratedInstance)
            {
                _decoratedInstance = decoratedInstance;
            }

            public string DecoratedResult => "Mega " + _decoratedInstance.DecoratedResult;
        }

        public class StandardDecorator : IDecorator
        {
            public string DecoratedResult => "Dummy";
        }
    }
}
