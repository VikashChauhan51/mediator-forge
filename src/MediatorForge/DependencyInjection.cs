using MediatorForge.CQRS.Authorizers;
using MediatorForge.CQRS.Behaviors;
using MediatorForge.CQRS.Validators;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;

namespace MediatorForge;

/// <summary>
/// Provides extension methods for configuring the MediatorForge library services.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds and configures the MediatorForge library services to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="assembly">The assembly containing the MediatR handlers and validators.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddMediatorForge(this IServiceCollection services, Assembly assembly)
    {
        // Register MediatR
        services.AddMediatR((configuration) =>
        {
            configuration.RegisterServicesFromAssemblies(assembly);
            configuration.AddOpenBehavior(typeof(LoggingBehavior<,>));
            configuration.AddOpenBehavior(typeof(AuthorizationBehavior<,>));
            configuration.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        // Register default authorizer and validator
        services.AddTransient(typeof(IAuthorizer<>), typeof(DefaultAuthorizer<>));
        services.AddTransient(typeof(IValidator<>), typeof(DefaultValidator<>));
        return services;
    }
}
