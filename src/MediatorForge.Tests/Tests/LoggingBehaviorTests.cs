
namespace MediatorForge.Tests.Tests;

[Trait("Category", "Unit")]
public class LoggingBehaviorTests
{
    private readonly Mock<ILogger<LoggingBehavior<TestRequest, TestResponse>>> _loggerMock;
    private readonly LoggingBehavior<TestRequest, TestResponse> _behavior;
    private readonly TestRequest _testRequest;
    private readonly RequestHandlerDelegate<TestResponse> _next;

    public LoggingBehaviorTests()
    {
        _loggerMock = new Mock<ILogger<LoggingBehavior<TestRequest, TestResponse>>>();
        _behavior = new LoggingBehavior<TestRequest, TestResponse>(_loggerMock.Object);
        _testRequest = new TestRequest { RequestData = "Sample data" };
        _next = Mock.Of<RequestHandlerDelegate<TestResponse>>();
    }

    [Fact]
    public async Task Handle_ShouldLogStartAndEnd_WhenProcessingRequest()
    {
        // Arrange
        var testResponse = new TestResponse { ResponseData = "Response data" };
        Mock.Get(_next).Setup(n => n()).ReturnsAsync(testResponse);

        // Act
        var response = await _behavior.Handle(_testRequest, _next, CancellationToken.None);

        // Assert
        response.Should().BeSameAs(testResponse);
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("[START] Handle request")),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
            Times.Once);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("[END] Handled")),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldLogPerformanceWarning_WhenRequestTakesMoreThanThreeSeconds()
    {
        // Arrange
        var testResponse = new TestResponse { ResponseData = "Response data" };
        Mock.Get(_next).Setup(n => n()).Returns(async () =>
        {
            await Task.Delay(4000); // Simulate long-running operation
            return testResponse;
        });

        // Act
        var response = await _behavior.Handle(_testRequest, _next, CancellationToken.None);

        // Assert
        response.Should().BeSameAs(testResponse);
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("[PERFORMANCE] The request")),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
            Times.Once);
    }
}
