using Microsoft.Extensions.DependencyInjection;

namespace MediatorForge.Tests.Tests;

[Trait("Category", "Unit")]
public class DependencyInjectionTests
{
    [Fact]
    public void AddMediatorForge_ShouldRegisterMediatR_WithBehaviors()
    {
        // Arrange
        var services = new ServiceCollection();
        var assembly = typeof(DependencyInjectionTests).Assembly;

        // Act
        services.AddMediatorForge(assembly);

        var mediator = services.SingleOrDefault(sd => sd.ServiceType == typeof(IMediator)
                                                     && sd.Lifetime == ServiceLifetime.Transient);
        // Assert MediatR registrations
        mediator.Should().NotBeNull();

        // Assert that behaviors are registered
        var behaviors = services.Where(sd => sd.ServiceType == typeof(IPipelineBehavior<,>)
                                                     && sd.Lifetime == ServiceLifetime.Transient);
        behaviors.Should().ContainSingle(x => x.ImplementationType == typeof(LoggingBehavior<,>));
        behaviors.Should().ContainSingle(x => x.ImplementationType == typeof(AuthorizationBehavior<,>));
        behaviors.Should().ContainSingle(x => x.ImplementationType == typeof(ValidationBehavior<,>));
    }
}
