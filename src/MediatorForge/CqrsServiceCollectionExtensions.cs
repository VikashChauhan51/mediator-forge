using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MediatorForge.Abstractions;

namespace MediatorForge;

/// <summary>
/// The <see cref="CqrsServiceCollectionExtensions"/> class provides extension methods for registering CQRS-related services in the dependency injection container. It includes methods for adding the dispatcher, handlers, and pipeline behaviors from specified assemblies. This allows for a clean and modular way to set up the CQRS infrastructure in your application.
/// </summary>
public static class CqrsServiceCollectionExtensions
{

    /// <summary>
    /// Adds the CQRS services to the dependency injection container. This method registers the <see cref="IDispatcher"/> implementation, which is responsible for dispatching commands and queries to their respective handlers. It also allows for the registration of handlers and pipeline behaviors from specified assemblies using the other extension methods provided in this class.
    /// </summary>
    /// <param name="services">The service collection to which to add the CQRS services.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddCqrs(this IServiceCollection services)
    {
        services.AddScoped<IDispatcher, Dispatcher>();
        services.AddScoped<ICommandDispatcher, Dispatcher>();
        services.AddScoped<IQueryDispatcher, Dispatcher>();
        services.AddScoped<IEventDispatcher, Dispatcher>();

        return services;
    }

    /// <summary>
    /// Adds handlers from the assembly containing the specified type to the dependency injection container. This method uses an <see cref="AssemblyScanner"/> to find all types that implement the <see cref="IHandler{TRequest, TResponse}"/> interface in the specified assembly and registers them with the specified lifetime.
    /// This allows for a clean and modular way to register handlers without having to manually specify each one.
    /// </summary>
    /// <typeparam name="T">The type whose assembly contains the handlers to register.</typeparam>
    /// <param name="services">The service collection to which to add the handlers.</param>
    /// <param name="lifetime">The lifetime of the registered services.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddHandlersFromAssemblyContaining<T>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Transient)
    {
        var handlerTypes = new AssemblyScanner(typeof(IHandler<,>), typeof(T).Assembly.GetTypes());

        foreach (var handlerType in handlerTypes)
        {
            services.TryAdd(new ServiceDescriptor(handlerType.InterfaceType, handlerType.ImplementationType, lifetime));
        }

        return services;
    }

    /// <summary>
    /// Adds pipeline behaviors from the assembly containing the specified type to the dependency injection container.
    ///  This method uses an <see cref="AssemblyScanner"/> to find all types that implement the <see cref="IPipelineBehavior{TRequest, TResponse}"/> interface in the specified assembly and registers them with the specified lifetime.
    /// This allows for a clean and modular way to register pipeline behaviors without having to manually specify each one.
    /// </summary>
    /// <typeparam name="T">The type whose assembly contains the pipeline behaviors to register.</typeparam>
    /// <param name="services">The service collection to which to add the pipeline behaviors.</param>
    /// <param name="lifetime">The lifetime of the registered services.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddPipelineBehaviorsFromAssemblyContaining<T>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Transient)
    {
        var pipelineBehaviorTypes = new AssemblyScanner(typeof(IPipelineBehavior<,>), typeof(T).Assembly.GetTypes());

        foreach (var pipelineBehaviorType in pipelineBehaviorTypes)
        {
            services.TryAdd(new ServiceDescriptor(pipelineBehaviorType.InterfaceType, pipelineBehaviorType.ImplementationType, lifetime));
        }

        return services;
    }

    /// <summary>
    /// Adds event notification handlers from the assembly containing the specified type to the dependency injection container. This method uses an <see cref="AssemblyScanner"/> to find all types that implement the <see cref="IEventNotificationHandler{TEvent}"/> interface in the specified assembly and registers them with the specified lifetime.
    /// This allows for a clean and modular way to register event notification handlers without having to manually specify
    /// </summary>
    /// <typeparam name="T">The type whose assembly contains the event notification handlers to register.</typeparam>
    /// <param name="services">The service collection to which to add the event notification handlers.</param>
    /// <param name="lifetime">The lifetime of the registered services.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddEventHandlersFromAssemblyContaining<T>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Transient)
    {
        var eventHandlerTypes = new AssemblyScanner(typeof(IEventNotificationHandler<>), typeof(T).Assembly.GetTypes());

        foreach (var eventHandlerType in eventHandlerTypes)
        {
            services.TryAdd(new ServiceDescriptor(eventHandlerType.InterfaceType, eventHandlerType.ImplementationType, lifetime));
        }

        return services;
    }
}
