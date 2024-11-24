using MediatorForge.CQRS.Behaviors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace MediatorForge;

public static class DependencyInjection
{
    public static IServiceCollection AddMediatorForge(this IServiceCollection services, Assembly assembly)
    {
        // Register MediatR
        services.AddMediatR((configuration) =>
        {
            configuration.RegisterServicesFromAssemblies(assembly);
            configuration.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            configuration.AddBehavior(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehavior<,>));
            configuration.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ResultValidationBehavior<,>));
            configuration.AddBehavior(typeof(IPipelineBehavior<,>), typeof(OptionValidationBehavior<,>));
            configuration.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });

        return services;
    }
}
