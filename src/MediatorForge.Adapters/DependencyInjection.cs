using Microsoft.Extensions.DependencyInjection;

namespace MediatorForge.Adapters;

public static class DependencyInjection
{
    public static IServiceCollection AddMediatorForgeAdapters(this IServiceCollection services)
    {
        services.AddTransient(typeof(CQRS.IValidator<>), typeof(FluentValidatorAdapter<>));

        return services;
    }
}
