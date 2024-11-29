using MediatorForge.Results;

namespace MediatorForge.Tests.Tests;

[Trait("Category", "Unit")]
public class ValidationBehaviorTests
{
    private readonly List<Mock<IValidator<TestRequest>>> _validatorMock;
    private readonly Mock<ILogger<ValidationBehavior<TestRequest, TestResponse>>> _loggerMock;
    private readonly ValidationBehavior<TestRequest, TestResponse> _behavior;
    private readonly TestRequest _testRequest;
    private readonly RequestHandlerDelegate<TestResponse> _next;

    public ValidationBehaviorTests()
    {
        _validatorMock = new List<Mock<IValidator<TestRequest>>>()
        {
            new Mock<IValidator<TestRequest>>(),
            new  Mock<IValidator<TestRequest>>()
        };
        _loggerMock = new Mock<ILogger<ValidationBehavior<TestRequest, TestResponse>>>();
        _behavior = new ValidationBehavior<TestRequest, TestResponse>(_validatorMock.Select(x=>x.Object), _loggerMock.Object);
        _testRequest = new TestRequest { RequestData = "Sample data" };
        _next = Mock.Of<RequestHandlerDelegate<TestResponse>>();
    }

    [Fact]
    public async Task Handle_ShouldProceedToNextDelegate_WhenAuthorizationSucceeds()
    {
        // Arrange
        var validationResult = ValidationResult.Success;
        foreach (var validator in _validatorMock)
        {
            validator.Setup(v => v.ValidateAsync(_testRequest, It.IsAny<CancellationToken>())).ReturnsAsync(validationResult);
        }

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
        var ValidationResults = new List<ValidationResult> { new ValidationResult("Code", "Message") };
        var validationResult = ValidationResult.Failure(ValidationResults);
        var _testRequest = new TestRequestResult
        {
            RequestData = "Result request"
        };
        var _authorizerMock = new Mock<IValidator<TestRequestResult>>();
        var _loggerMock = new Mock<ILogger<ValidationBehavior<TestRequestResult, Result<TestResponse>>>>();
        var _next = Mock.Of<RequestHandlerDelegate<Result<TestResponse>>>();
        var _behavior = new ValidationBehavior<TestRequestResult, Result<TestResponse>>(_authorizerMock.Object, _loggerMock.Object);
        _authorizerMock.Setup(a => a.AuthorizeAsync(_testRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

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
        var ValidationResults = new List<ValidationResult> { new ValidationResult("Code", "Message") };
        var validationResult = validationResult.Failure(ValidationResults);
        var _authorizerMock = new Mock<IValidator<TestRequestOption>>();
        var _testRequest = new TestRequestOption { RequestData = "Sample data" };
        var _loggerMock = new Mock<ILogger<ValidationBehavior<TestRequestOption, Option<TestResponse>>>>();
        var _next = Mock.Of<RequestHandlerDelegate<Option<TestResponse>>>();
        var behavior = new ValidationBehavior<TestRequestOption, Option<TestResponse>>(_authorizerMock.Object, _loggerMock.Object);
        _authorizerMock.Setup(a => a.AuthorizeAsync(_testRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

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
        var ValidationResults = new List<ValidationResult> { new ValidationResult("Code", "Message") };
        var validationResult = validationResult.Failure(ValidationResults);
        var _authorizerMock = new Mock<IValidator<TestRequestResult>>();
        var _testRequest = new TestRequestResult { RequestData = "Sample data" };
        var _loggerMock = new Mock<ILogger<ValidationBehavior<TestRequestResult, Result<TestResponse>>>>();
        var _next = Mock.Of<RequestHandlerDelegate<Result<TestResponse>>>();
        var behavior = new ValidationBehavior<TestRequestResult, Result<TestResponse>>(_authorizerMock.Object, _loggerMock.Object);
        _authorizerMock.Setup(a => a.AuthorizeAsync(_testRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

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
        var ValidationResults = new List<ValidationResult> { new ValidationResult("Code", "Message") };
        var validationResult = validationResult.Failure(ValidationResults);
        var behavior = new ValidationBehavior<TestRequest, TestResponse>(_validatorMock.Object, _loggerMock.Object);
        _validatorMock.Setup(a => a.AuthorizeAsync(_testRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        // Act & Assert
        await Assert.ThrowsAsync<AuthorizationException>(() => behavior.Handle(_testRequest, _next, CancellationToken.None));
    }
}
