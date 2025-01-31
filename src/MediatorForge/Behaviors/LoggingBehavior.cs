using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace MediatorForge.Behaviors;

/// <summary>
/// Behavior for logging the handling of requests and responses.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public sealed class LoggingBehavior<TRequest, TResponse> : IBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse> where TResponse : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoggingBehavior{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Handles the request and logs the request and response details.
    /// </summary>
    /// <param name="request">The request instance.</param>
    /// <param name="next">The delegate to the next handler in the pipeline.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The response from the next handler in the pipeline.</returns>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling {Request}", typeof(TRequest).Name);

        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("Request Data: {RequestData}", request);
        }

        var sw = Stopwatch.StartNew();
        var response = await next();
        sw.Stop();

        _logger.LogInformation("Handled {Response} in {ResponseMilliseconds} ms", typeof(TResponse).Name, sw.ElapsedMilliseconds);

        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("Response Data: {ResponseData}", response);
        }

        return response;
    }
}
