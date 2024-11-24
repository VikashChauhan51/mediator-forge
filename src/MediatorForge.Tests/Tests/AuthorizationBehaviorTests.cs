

namespace MediatorForge.Adapters.Tests.Tests;

//[Trait("Category", "Unit")]
//public class AuthorizationBehaviorTests
//{
//    private readonly Mock<ILogger<AuthorizationBehavior<TestRequest, TestResponse>>> _loggerMock;
//    private readonly Mock<IAuthorizer<TestRequest>> _authorizerMock;
//    private readonly AuthorizationBehavior<TestRequest, TestResponse> _authorizationBehavior;

//    public AuthorizationBehaviorTests()
//    {
//        _loggerMock = new Mock<ILogger<AuthorizationBehavior<TestRequest, TestResponse>>>();
//        _authorizerMock = new Mock<IAuthorizer<TestRequest>>();
//        var authorizers = new List<IAuthorizer<TestRequest>> { _authorizerMock.Object };
//        _authorizationBehavior = new AuthorizationBehavior<TestRequest, TestResponse>(authorizers, _loggerMock.Object);
//    }

//    [Fact]
//    public async Task Handle_AuthorizationSucceeds_ShouldCallNextDelegate()
//    {
//        // Arrange
//        var request = AutoFaker.Generate<TestRequest>();
//        var nextDelegate = new Mock<RequestHandlerDelegate<TestResponse>>();
//        nextDelegate.Setup(nd => nd()).ReturnsAsync(new TestResponse());
//        _authorizerMock.Setup(a => a.AuthorizeAsync(request)).ReturnsAsync(new AuthorizationResult { IsAuthorized = true });

//        // Act
//        var result = await _authorizationBehavior.Handle(request, nextDelegate.Object, CancellationToken.None);

//        // Assert
//        result.Should().NotBeNull();
//        nextDelegate.Verify(nd => nd(), Times.Once);
//    }

//    [Fact]
//    public async Task Handle_AuthorizationFails_ShouldThrowAuthorizationException()
//    {
//        // Arrange
//        var request = AutoFaker.Generate<TestRequest>();
//        var nextDelegate = new Mock<RequestHandlerDelegate<TestResponse>>();
//        var authorizationErrors = new List<AuthorizationError> { new AuthorizationError("Error", "Not authorized") };
//        _authorizerMock.Setup(a => a.AuthorizeAsync(request)).ReturnsAsync(new AuthorizationResult { IsAuthorized = false, Errors = authorizationErrors });

//        // Act
//        Func<Task> act = async () => await _authorizationBehavior.Handle(request, nextDelegate.Object, CancellationToken.None);

//        // Assert
//        await act.Should().ThrowAsync<AuthorizationException>();
//        nextDelegate.Verify(nd => nd(), Times.Never);
//    }
//}
