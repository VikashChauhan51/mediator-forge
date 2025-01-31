using MediatorForge.Abstraction;
using MediatorForge.Behaviors;
using MediatorForge.Commands;
using MediatorForge.Queries;
using MediatR;
using Moq;
using ResultifyCore;

namespace MediatorForge.Tests;

[Trait("Category", "Unit")]
public class AuthorizationBehaviorTests
{
    private readonly Mock<IAuthorization> _authorizationMock;
    private readonly Mock<RequestHandlerDelegate<string>> _nextMock;
    private readonly AuthorizationBehavior<IRequest<string>, string> _behavior;

    public AuthorizationBehaviorTests()
    {
        _authorizationMock = new Mock<IAuthorization>();
        _nextMock = new Mock<RequestHandlerDelegate<string>>();
        _behavior = new AuthorizationBehavior<IRequest<string>, string>(_authorizationMock.Object);
    }

    [Fact]
    public async Task Handle_AuthorizationFails_ReturnsUnauthorizedOutcome()
    {
        // Arrange
        var authorizationResult = new AuthorizationResult(false, "Unauthorized");
        _authorizationMock.Setup(a => a.AuthorizeAsync()).ReturnsAsync(authorizationResult);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _behavior.Handle(Mock.Of<IRequest<string>>(), _nextMock.Object, CancellationToken.None));
        _nextMock.Verify(n => n(), Times.Never);
    }

    [Fact]
    public async Task Handle_AuthorizationSucceeds_CallsNextDelegate()
    {
        // Arrange
        var authorizationResult = new AuthorizationResult(true, "");
        _authorizationMock.Setup(a => a.AuthorizeAsync()).ReturnsAsync(authorizationResult);
        _nextMock.Setup(n => n()).ReturnsAsync("Success");

        // Act
        var result = await _behavior.Handle(Mock.Of<IRequest<string>>(), _nextMock.Object, CancellationToken.None);

        // Assert
        Assert.Equal("Success", result);
        _nextMock.Verify(n => n(), Times.Once);
    }

    [Fact]
    public async Task Handle_CommandUnauthorized_ReturnsUnauthorizedResult()
    {
        var _authorizationMock = new Mock<IAuthorization>();
        var _commandBehavior = new AuthorizationBehavior<ICommand<Unit>, Unit>(_authorizationMock.Object);
        var _queryBehavior = new AuthorizationBehavior<IQuery<Outcome<string>>, Outcome<string>>(_authorizationMock.Object);
        // Arrange
        _authorizationMock.Setup(a => a.AuthorizeAsync()).ReturnsAsync(new AuthorizationResult(false, "Unauthorized"));
        var commandMock = new Mock<ICommand<Unit>>();
        var next = new RequestHandlerDelegate<Unit>(() => Task.FromResult(Unit.Value));

        // Act
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _commandBehavior.Handle(commandMock.Object, next, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_QueryUnauthorized_ReturnsUnauthorizedOutcome()
    {
        var _authorizationMock = new Mock<IAuthorization>();
        var _commandBehavior = new AuthorizationBehavior<ICommand<Unit>, Unit>(_authorizationMock.Object);
        var _queryBehavior = new AuthorizationBehavior<IQuery<Outcome<string>>, Outcome<string>>(_authorizationMock.Object);

        // Arrange
        _authorizationMock.Setup(a => a.AuthorizeAsync()).ReturnsAsync(new AuthorizationResult(false, "Unauthorized"));
        var queryMock = new Mock<IQuery<Outcome<string>>>();
        var next = new RequestHandlerDelegate<Outcome<string>>(() => Task.FromResult(new Outcome<string>("Success")));

        // Act
        var result = await _queryBehavior.Handle(queryMock.Object, next, CancellationToken.None);

        // Assert
        Assert.Equal(ResultState.Unauthorized, result.Status);
        Assert.Equal("Unauthorized", result.Errors.First().Message);
    }

    [Fact]
    public async Task Handle_CommandAuthorized_ExecutesNextDelegate()
    {
        var _authorizationMock = new Mock<IAuthorization>();
        var _commandBehavior = new AuthorizationBehavior<ICommand<Unit>, Unit>(_authorizationMock.Object);
        var _queryBehavior = new AuthorizationBehavior<IQuery<Outcome<string>>, Outcome<string>>(_authorizationMock.Object);

        // Arrange
        _authorizationMock.Setup(a => a.AuthorizeAsync()).ReturnsAsync(new AuthorizationResult(true, ""));
        var commandMock = new Mock<ICommand<Unit>>();
        var next = new RequestHandlerDelegate<Unit>(() => Task.FromResult(Unit.Value));

        // Act
        var result = await _commandBehavior.Handle(commandMock.Object, next, CancellationToken.None);

        // Assert
        Assert.Equal(Unit.Value, result);
    }

    [Fact]
    public async Task Handle_QueryAuthorized_ExecutesNextDelegate()
    {
        var _authorizationMock = new Mock<IAuthorization>();
        var _commandBehavior = new AuthorizationBehavior<ICommand<Unit>, Unit>(_authorizationMock.Object);
        var _queryBehavior = new AuthorizationBehavior<IQuery<Outcome<string>>, Outcome<string>>(_authorizationMock.Object);

        // Arrange
        _authorizationMock.Setup(a => a.AuthorizeAsync()).ReturnsAsync(new AuthorizationResult(true, ""));
        var queryMock = new Mock<IQuery<Outcome<string>>>();
        var next = new RequestHandlerDelegate<Outcome<string>>(() => Task.FromResult(new Outcome<string>("Success")));

        // Act
        var result = await _queryBehavior.Handle(queryMock.Object, next, CancellationToken.None);

        // Assert
        Assert.Equal(ResultState.Success, result.Status);
        Assert.Equal("Success", result.Value);
    }
}
