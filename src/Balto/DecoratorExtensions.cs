using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Balto
{
    public static class DecoratorExtensions
    {
        /// <summary>
        /// Registers a <typeparamref name="TService"/> decorator on top of the previous registration of that type.
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="decoratorFactory">Constructs a new instance based on the the instance to decorate and the <see cref="IServiceProvider"/>.</param>
        /// <param name="lifetime">If no lifetime is provided, the lifetime of the previous registration is used.</param>
        public static IServiceCollection AddDecorator<TService>(
            this IServiceCollection services,
            Func<IServiceProvider, TService, TService> decoratorFactory,
            ServiceLifetime? lifetime = null)
            where TService : class
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (decoratorFactory == null) throw new ArgumentNullException(nameof(decoratorFactory));

            // By convention, the last registration wins
            ServiceDescriptor? previousRegistration = services.LastOrDefault(descriptor => descriptor.ServiceType == typeof(TService));

            if (previousRegistration is null)
                throw new InvalidOperationException($"Tried to register a decorator for type {typeof(TService).Name} when no such type was registered.");

            // Get a factory to produce the original implementation
            Func<IServiceProvider, object> decoratedServiceFactory = GetFactory<TService>(previousRegistration);

            var registration = new ServiceDescriptor(
                typeof(TService), 
                CreateDecorator, 
                lifetime ?? previousRegistration.Lifetime);

            services.Add(registration);

            return services;

            TService CreateDecorator(IServiceProvider serviceProvider)
            {
                TService decoratedInstance = (TService)decoratedServiceFactory(serviceProvider);
                TService decorator = decoratorFactory(serviceProvider, decoratedInstance);
                return decorator;
            }
        }

        /// <summary>
        /// Registers a <typeparamref name="TService"/> decorator on top of the previous registration of that type.
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="lifetime">If no lifetime is provided, the lifetime of the previous registration is used.</param>
        public static IServiceCollection AddDecorator<TService, TImplementation>(
            this IServiceCollection services,
            ServiceLifetime? lifetime = null)
            where TService : class
            where TImplementation : TService
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            return AddDecorator<TService>(
                services,
                (serviceProvider, decoratedInstance) =>
                    ActivatorUtilities.CreateInstance<TImplementation>(serviceProvider, decoratedInstance),
                lifetime);
        }

        private static Func<IServiceProvider, object> GetFactory<TService>(ServiceDescriptor previousRegistration) where TService : class
        {
            Func<IServiceProvider, object>? decoratedServiceFactory = previousRegistration.ImplementationFactory;
            if (decoratedServiceFactory is null && previousRegistration.ImplementationInstance != null)
                decoratedServiceFactory = _ => previousRegistration.ImplementationInstance;
            if (decoratedServiceFactory is null && previousRegistration.ImplementationType != null)
                decoratedServiceFactory = serviceProvider => ActivatorUtilities.CreateInstance(
                    serviceProvider, previousRegistration.ImplementationType, Array.Empty<object>());

            if (decoratedServiceFactory is null) // Should not be null here, but we are checking to be sure
                throw new Exception($"Tried to register a decorator for type {typeof(TService).Name}, but the registration being wrapped specified no implementation at all.");

            return decoratedServiceFactory;
        }
    }
}