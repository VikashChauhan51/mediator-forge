using MediatorForge.Abstraction;
using MediatorForge.Behaviors;
using MediatorForge.Queries;
using MediatR;
using Moq;

namespace MediatorForge.Tests;

[Trait("Category", "Unit")]
public class QueryCachingBehaviorTests
{
    private readonly Mock<ICache> _cacheMock;
    private readonly Mock<RequestHandlerDelegate<string>> _nextMock;
    private readonly QueryCachingBehavior<IQuery<string>, string> _cachingBehavior;

    public QueryCachingBehaviorTests()
    {
        _cacheMock = new Mock<ICache>();
        _nextMock = new Mock<RequestHandlerDelegate<string>>();
        _cachingBehavior = new QueryCachingBehavior<IQuery<string>, string>(_cacheMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnCachedResponse_WhenResponseIsCached()
    {
        // Arrange
        var request = new Mock<IQuery<string>>();
        request.Setup(c=> c.GetHashCode()).Returns(1);  
        var cachedResponse = "cached response";
        _cacheMock.Setup(c => c.GetAsync<string>(It.IsAny<string>())).ReturnsAsync(cachedResponse);

        // Act
        var response = await _cachingBehavior.Handle(request.Object, _nextMock.Object, CancellationToken.None);

        // Assert
        Assert.Equal(cachedResponse, response);
        _nextMock.Verify(n => n(), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldInvokeNextAndCacheResponse_WhenResponseIsNotCached()
    {
        // Arrange
        var request = new Mock<IQuery<string>>();
        request.Setup(c => c.GetHashCode()).Returns(1);
        var response = "response";
        _cacheMock.Setup(c => c.GetAsync<string>(It.IsAny<string>())).ReturnsAsync((string)null);
        _nextMock.Setup(n => n()).ReturnsAsync(response);

        // Act
        var result = await _cachingBehavior.Handle(request.Object, _nextMock.Object, CancellationToken.None);

        // Assert
        Assert.Equal(response, result);
        _nextMock.Verify(n => n(), Times.Once);
        _cacheMock.Verify(c => c.SetAsync(It.IsAny<string>(), response), Times.Once);
    }
}
