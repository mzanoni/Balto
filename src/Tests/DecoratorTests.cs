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
            
            services.AddSingleton<IDecorator, DecoratedClass>();
            services.AddDecorator<IDecorator, InnerDecorator>();
            services.AddDecorator<IDecorator, OuterDecorator>();

            services.AddSingleton<IScopedDecorator, ScopeDecoratedClass>();
            services.AddDecorator<IScopedDecorator, ScopedDecorator>(ServiceLifetime.Scoped);

            _provider = services.BuildServiceProvider();
        }

        [Fact]
        public void ShouldReturnInstance_ResultShouldBeWrappedInRegisteredOrder()
        {
            IDecorator decorator = _provider.GetService<IDecorator>();

            Assert.NotNull(decorator);
            Assert.Equal("Outer Inner Dummy", decorator.DecoratedResult);
        }

        [Fact]
        public void ShouldReturnInstance_ResultsShouldBeWrappedAndDifferent()
        {
            string result1;
            string result2;

            using (_provider.CreateScope())
            {
                IScopedDecorator decorator = _provider.GetRequiredService<IScopedDecorator>();
                result1 = decorator.DecoratedResult;
            }

            using (_provider.CreateScope())
            {
                IScopedDecorator decorator = _provider.GetRequiredService<IScopedDecorator>();
                result2 = decorator.DecoratedResult;

            }

            Assert.NotEqual(result1, result2);
        }

        public interface IDecorator
        {
            public string DecoratedResult { get; }
        }

        public class InnerDecorator : IDecorator
        {
            private readonly IDecorator _decoratedInstance;

            public InnerDecorator(IDecorator decoratedInstance)
            {
                _decoratedInstance = decoratedInstance;
            }

            public string DecoratedResult => "Inner " + _decoratedInstance.DecoratedResult;
        }

        public class OuterDecorator : IDecorator
        {
            private readonly IDecorator _decoratedInstance;

            public OuterDecorator(IDecorator decoratedInstance)
            {
                _decoratedInstance = decoratedInstance;
            }

            public string DecoratedResult => "Outer " + _decoratedInstance.DecoratedResult;
        }

        public class DecoratedClass : IDecorator
        {
            public string DecoratedResult => "Dummy";
        }


        public interface IScopedDecorator
        {
            public string DecoratedResult { get; }
        }

        public class ScopedDecorator : IScopedDecorator
        {
            private readonly IScopedDecorator _decoratedInstance;

            public ScopedDecorator(IScopedDecorator decoratedInstance)
            {
                _decoratedInstance = decoratedInstance;
            }

            public string DecoratedResult => $"{Guid.NewGuid()} " + _decoratedInstance.DecoratedResult;
        }

        public class ScopeDecoratedClass : IScopedDecorator
        {
            public string DecoratedResult => "Dummy";
        }
    }
}
