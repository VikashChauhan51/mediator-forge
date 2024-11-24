﻿using MediatorForge.CQRS.Behaviors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
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
            configuration.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            configuration.AddBehavior(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehavior<,>));
            configuration.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ResultAuthorizationBehavior<,>));
            configuration.AddBehavior(typeof(IPipelineBehavior<,>), typeof(OptionAuthorizationBehavior<,>));
            configuration.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            configuration.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ResultValidationBehavior<,>));
            configuration.AddBehavior(typeof(IPipelineBehavior<,>), typeof(OptionValidationBehavior<,>));
        });

        return services;
    }
}
