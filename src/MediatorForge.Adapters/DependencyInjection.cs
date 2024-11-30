using MediatorForge.CQRS.Validators;
using Microsoft.Extensions.DependencyInjection;

namespace MediatorForge.Adapters;

/// <summary>
/// Provides extension methods for configuring MediatorForge adapters.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds and configures the MediatorForge <see cref="FluentValidatorAdapter{T}"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddMediatorForgeFluentValidatorAdapter(this IServiceCollection services)
    {
        services.AddTransient(typeof(IValidator<>), typeof(FluentValidatorAdapter<>));

        return services;
    }
}
