using MediatorForge.Abstraction;
using MediatorForge.Behaviors;
using MediatR;
using Moq;


namespace MediatorForge.Tests;

[Trait("Category", "Unit")]
public class AuthorizationExceptionBehaviorTests
{
    private readonly Mock<IAuthorization> _mockAuthorization;
    private readonly AuthorizationExceptionBehavior<IRequest<string>, string> _behavior;

    public AuthorizationExceptionBehaviorTests()
    {
        _mockAuthorization = new Mock<IAuthorization>();
        _behavior = new AuthorizationExceptionBehavior<IRequest<string>, string>(_mockAuthorization.Object);
    }

    [Fact]
    public async Task Handle_AuthorizationFails_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var authorizationResult = new AuthorizationResult(false, "Unauthorized");
        _mockAuthorization.Setup(a => a.AuthorizeAsync()).ReturnsAsync(authorizationResult);
        var request = Mock.Of<IRequest<string>>();
        RequestHandlerDelegate<string> next = () => Task.FromResult("Success");

        // Act & Assert
        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _behavior.Handle(request, next, CancellationToken.None));
        Assert.Equal("Unauthorized", exception.Message);
    }

    [Fact]
    public async Task Handle_AuthorizationSucceeds_CallsNextDelegate()
    {
        // Arrange
        var authorizationResult = new AuthorizationResult(true, "");
        _mockAuthorization.Setup(a => a.AuthorizeAsync()).ReturnsAsync(authorizationResult);
        var request = Mock.Of<IRequest<string>>();
        var nextCalled = false;
        RequestHandlerDelegate<string> next = () =>
        {
            nextCalled = true;
            return Task.FromResult("Success");
        };

        // Act
        var response = await _behavior.Handle(request, next, CancellationToken.None);

        // Assert
        Assert.True(nextCalled);
        Assert.Equal("Success", response);
    }
}
