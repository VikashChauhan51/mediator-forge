using MediatR;
using MediatorForge.Behaviors;
using Microsoft.Extensions.Logging;
using Moq;


namespace MediatorForge.Tests;

[Trait("Category", "Unit")] 
public class LoggingBehaviorTests
{
    private readonly Mock<ILogger<LoggingBehavior<IRequest<string>, string>>> _loggerMock;
    private readonly Mock<RequestHandlerDelegate<string>> _nextMock;
    private readonly LoggingBehavior<IRequest<string>, string> _behavior;

    public LoggingBehaviorTests()
    {
        _loggerMock = new Mock<ILogger<LoggingBehavior<IRequest<string>, string>>>();
        _nextMock = new Mock<RequestHandlerDelegate<string>>();
        _behavior = new LoggingBehavior<IRequest<string>, string>(_loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldLogRequestAndResponse()
    {
        // Arrange
        var requestMock = new Mock<IRequest<string>>();
        _nextMock.Setup(x => x()).ReturnsAsync("Response");

        // Act
        var response = await _behavior.Handle(requestMock.Object, _nextMock.Object, CancellationToken.None);

        // Assert
        _loggerMock.Verify(x => x.Log(
            LogLevel.Information,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Handling")),
            It.IsAny<Exception>(),
            It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);

        _loggerMock.Verify(x => x.Log(
            LogLevel.Information,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Handled")),
            It.IsAny<Exception>(),
            It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);

        Assert.Equal("Response", response);
    }

    [Fact]
    public async Task Handle_ShouldLogDebugInformation_WhenDebugEnabled()
    {
        // Arrange
        var requestMock = new Mock<IRequest<string>>();
        _nextMock.Setup(x => x()).ReturnsAsync("Response");
        _loggerMock.Setup(x => x.IsEnabled(LogLevel.Debug)).Returns(true);

        // Act
        var response = await _behavior.Handle(requestMock.Object, _nextMock.Object, CancellationToken.None);

        // Assert
        _loggerMock.Verify(x => x.Log(
            LogLevel.Debug,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Request Data")),
            It.IsAny<Exception>(),
            It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);

        _loggerMock.Verify(x => x.Log(
            LogLevel.Debug,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Response Data")),
            It.IsAny<Exception>(),
            It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);

        Assert.Equal("Response", response);
    }
}
