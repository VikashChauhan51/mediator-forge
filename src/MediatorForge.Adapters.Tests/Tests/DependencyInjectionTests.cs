using MediatorForge.CQRS.Validators;
using Microsoft.Extensions.DependencyInjection;

namespace MediatorForge.Adapters.Tests.Tests;

[Trait("Category", "Unit")]
public class DependencyInjectionTests
{
    [Fact]
    public void AddMediatorForgeFluentValidatorAdapter_ShouldAddFluentValidatorAdapterToServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();
        // Act
        services.AddMediatorForgeFluentValidatorAdapter();

        // Assert
        var descriptor = services.SingleOrDefault(sd => sd.ServiceType == typeof(IValidator<>)
                                                        && sd.ImplementationType == typeof(FluentValidatorAdapter<>)
                                                        && sd.Lifetime == ServiceLifetime.Transient);
        descriptor.Should().NotBeNull();
    }

    [Fact]
    public void AddMediatorForgeFluentValidatorAdapter_ShouldReturnSameServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddMediatorForgeFluentValidatorAdapter();

        // Assert
        result.Should().BeSameAs(services);
    }
}

