using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.ObjectPool;
using System.Diagnostics;
using FluentAssertions.Execution;

namespace Testing
{
    /// <summary>
    /// Helper class for testing IServiceCollection registrations in unit tests.
    /// </summary>
    public static class AssertServiceCollectionExtensions
    {
        /// <summary>
        /// Checks that a service of type T can be resolved by the service collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public static T ShouldResolve<T>(this ServiceProvider serviceProvider)
        {
            var actual = serviceProvider.GetService<T>();
            actual.Should().NotBeNull($"{typeof(T)} should be registered");
            return actual;
        }

        /// <summary>
        /// Checks that a service of type T can be resolved by the service collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceProvider"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static void ShouldResolve(this ServiceProvider serviceProvider, Type type)
        {
            try
            {
                var actual = serviceProvider.GetService(type);
                actual.Should().NotBeNull($"{type} should be registered");
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Failed to resolve service of type {type}. Error: {e.Message}", e);
            }
        }

        /// <summary>
        /// Checks that all services from provided assemblies can be resolved by the service collection.
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <param name="assemblies">Optionally provide assemblies for types to be tested</param>
        /// <exception cref="InvalidOperationException"></exception>
        public static void ShouldResolveAllRegisteredServices(this IServiceCollection serviceCollection, params Assembly[] assemblies)
        {
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var assemblyHash = new HashSet<Assembly>(assemblies);
            foreach (var service in serviceCollection)
            {
                if (service.ServiceType.IsGenericTypeDefinition || (assemblyHash.Any() && !assemblyHash.Contains(service.ServiceType.Assembly)))
                    continue;

                try
                {
                    using var scope = serviceProvider.CreateScope();
                    var resolvedService = scope.ServiceProvider.GetService(service.ServiceType);

                    if (service.Lifetime == ServiceLifetime.Scoped || service.Lifetime == ServiceLifetime.Singleton)
                    {
                        resolvedService.Should().NotBeNull($"Service of type {service.ServiceType} could not be resolved as {service.Lifetime}");
                    }
                }
                catch (Exception e)
                {
                    throw new InvalidOperationException($"Failed to resolve service of type {service.ServiceType}. Error: {e.Message}", e);
                }
            }
        }

        /// <summary>
        /// Checks that all controllers from provided assemblies can be resolved by the service collection.
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <param name="assemblies">Assemblies containing controllers for types to be tested</param>
        public static void ShouldHaveRegisteredControllersFrom(this IServiceCollection serviceCollection, params Assembly[] assemblies)
            => ShouldHaveRegisteredControllersFrom(serviceCollection, _ => true, assemblies);
        
        /// <summary>
        /// Checks that all controllers from provided assemblies that match the predicate can be resolved by the service collection.
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <param name="shouldBeChecked"></param>
        /// <param name="assemblies"></param>
        public static void ShouldHaveRegisteredControllersFrom(this IServiceCollection serviceCollection, Predicate<Type> shouldBeChecked, params Assembly[] assemblies)
        {
            if (!assemblies.Any())
            {
                throw new AssertionFailedException("At least one assembly must be provided to check controller registrations");
            }
            foreach (var assembly in assemblies)
            {
                RegisterAllControllers(serviceCollection, assembly);
            }

            RegisterMVCDependencies(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            foreach (var controllerType in assemblies.SelectMany(GetAllControllerTypes))
            {
                if (shouldBeChecked(controllerType))
                {
                    serviceProvider.ShouldResolve(controllerType);
                }
            }
        }
        
        /// <summary>
        /// Registers dependencies required for MVC functionality into the specified service collection.
        /// </summary>
        /// <param name="serviceCollection">The <see cref="IServiceCollection"/> to which the dependencies will be added. Cannot be null.</param>
        private static void RegisterMVCDependencies(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
            serviceCollection.AddSingleton<DiagnosticSource, DiagnosticListener>(_ => new DiagnosticListener("TestDiagnosticListener"));
        }

        /// <summary>
        /// Register all controllers from the provided assembly
        /// </summary>
        private static void RegisterAllControllers(IServiceCollection services, Assembly assembly)
        {
            var controllerTypes = GetAllControllerTypes(assembly);
            foreach (var controllerType in controllerTypes)
            {
                services.AddScoped(controllerType);
            }
        }

        /// <summary>
        /// Find all types that are controllers (inherited from ControllerBase)
        /// </summary>
        private static Type[] GetAllControllerTypes(Assembly assembly)
        {
            return assembly.GetTypes()
                .Where(type => typeof(ControllerBase).IsAssignableFrom(type) && !type.IsAbstract)
                .ToArray();
        }

    }
}
