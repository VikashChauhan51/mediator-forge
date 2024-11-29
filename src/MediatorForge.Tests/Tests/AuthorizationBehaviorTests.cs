using MediatorForge.CQRS.Commands;
using MediatorForge.Results;

namespace MediatorForge.Tests;

[Trait("Category", "Unit")]
public class AuthorizationBehaviorTests
{
    private readonly Mock<IAuthorizer<TestRequest>> _authorizerMock;
    private readonly Mock<ILogger<AuthorizationBehavior<TestRequest, TestResponse>>> _loggerMock;
    private readonly AuthorizationBehavior<TestRequest, TestResponse> _behavior;
    private readonly TestRequest _testRequest;
    private readonly RequestHandlerDelegate<TestResponse> _next;

    public AuthorizationBehaviorTests()
    {
        _authorizerMock = new Mock<IAuthorizer<TestRequest>>();
        _loggerMock = new Mock<ILogger<AuthorizationBehavior<TestRequest, TestResponse>>>();
        _behavior = new AuthorizationBehavior<TestRequest, TestResponse>(_authorizerMock.Object, _loggerMock.Object);
        _testRequest = new TestRequest { RequestData = "Sample data" };
        _next = Mock.Of<RequestHandlerDelegate<TestResponse>>();
    }

    [Fact]
    public async Task Handle_ShouldProceedToNextDelegate_WhenAuthorizationSucceeds()
    {
        // Arrange
        var authorizationResult = AuthorizationResult.Success;
        _authorizerMock.Setup(a => a.AuthorizeAsync(_testRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(authorizationResult);

        // Act
        var response = await _behavior.Handle(_testRequest, _next, CancellationToken.None);

        // Assert

        _loggerMock.Verify(
            x => x.Log(LogLevel.Information,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Authorizing request")),
            It.IsAny<Exception>(),
            It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailureResult_WhenAuthorizationFails_AndTResponseIsResult()
    {
        // Arrange
        var authorizationErrors = new List<AuthorizationError> { new AuthorizationError("Code", "Message") };
        var authorizationResult = AuthorizationResult.Failure(authorizationErrors);
        var _testRequest = new TestRequestResult
        {
            RequestData = "Result request"
        };
        var _authorizerMock = new Mock<IAuthorizer<TestRequestResult>>();
        var _loggerMock = new Mock<ILogger<AuthorizationBehavior<TestRequestResult, Result<TestResponse>>>>();
        var _next = Mock.Of<RequestHandlerDelegate<Result<TestResponse>>>();
        var _behavior = new AuthorizationBehavior<TestRequestResult, Result<TestResponse>>(_authorizerMock.Object, _loggerMock.Object);
        _authorizerMock.Setup(a => a.AuthorizeAsync(_testRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(authorizationResult);

        // Act
        var result = await _behavior.Handle(_testRequest, _next, CancellationToken.None);

        // Assert
        result.Should().BeOfType<Result<TestResponse>>();
        ((Result<TestResponse>)(object)result).IsSuccess.Should().BeFalse();
        ((Result<TestResponse>)(object)result).Exception.Should().BeOfType<AuthorizationException>();
    }

    [Fact]
    public async Task Handle_ShouldReturnOptionNone_WhenAuthorizationFails_AndTResponseIsOption()
    {
        // Arrange
        var authorizationErrors = new List<AuthorizationError> { new AuthorizationError("Code", "Message") };
        var authorizationResult = AuthorizationResult.Failure(authorizationErrors);
        var _authorizerMock = new Mock<IAuthorizer<TestRequestOption>>();
        var _testRequest = new TestRequestOption { RequestData = "Sample data" };
        var _loggerMock = new Mock<ILogger<AuthorizationBehavior<TestRequestOption, Option<TestResponse>>>>();
        var _next = Mock.Of<RequestHandlerDelegate<Option<TestResponse>>>();
        var behavior = new AuthorizationBehavior<TestRequestOption, Option<TestResponse>>(_authorizerMock.Object, _loggerMock.Object);
        _authorizerMock.Setup(a => a.AuthorizeAsync(_testRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(authorizationResult);

        // Act
        var result = await behavior.Handle(_testRequest, _next, CancellationToken.None);

        // Assert
        result.Should().BeOfType<Option<TestResponse>>();
        ((Option<TestResponse>)(object)result).IsSome.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_ShouldReturnResultError_WhenAuthorizationFails_AndTResponseIsResult()
    {
        // Arrange
        var authorizationErrors = new List<AuthorizationError> { new AuthorizationError("Code", "Message") };
        var authorizationResult = AuthorizationResult.Failure(authorizationErrors);
        var _authorizerMock = new Mock<IAuthorizer<TestRequestResult>>();
        var _testRequest = new TestRequestResult { RequestData = "Sample data" };
        var _loggerMock = new Mock<ILogger<AuthorizationBehavior<TestRequestResult, Result<TestResponse>>>>();
        var _next = Mock.Of<RequestHandlerDelegate<Result<TestResponse>>>();
        var behavior = new AuthorizationBehavior<TestRequestResult, Result<TestResponse>>(_authorizerMock.Object, _loggerMock.Object);
        _authorizerMock.Setup(a => a.AuthorizeAsync(_testRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(authorizationResult);

        // Act
        var result = await behavior.Handle(_testRequest, _next, CancellationToken.None);

        // Assert
        result.Should().BeOfType<Result<TestResponse>>();
        ((Result<TestResponse>)(object)result).IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenAuthorizationFails_AndTResponseIsNotResultOrOption()
    {
        // Arrange
        var authorizationErrors = new List<AuthorizationError> { new AuthorizationError("Code", "Message") };
        var authorizationResult = AuthorizationResult.Failure(authorizationErrors);
        var behavior = new AuthorizationBehavior<TestRequest, TestResponse>(_authorizerMock.Object, _loggerMock.Object);
        _authorizerMock.Setup(a => a.AuthorizeAsync(_testRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(authorizationResult);

        // Act & Assert
        await Assert.ThrowsAsync<AuthorizationException>(() => behavior.Handle(_testRequest, _next, CancellationToken.None));
    }

}

public class TestRequest : ICommand<TestResponse>
{
    public string RequestData { get; set; }
}

public class TestRequestOption : ICommand<Option<TestResponse>>
{
    public string RequestData { get; set; }
}

public class TestRequestResult : ICommand<Result<TestResponse>>
{
    public string RequestData { get; set; }
}

public class TestResponse
{
    public string ResponseData { get; set; }
}

