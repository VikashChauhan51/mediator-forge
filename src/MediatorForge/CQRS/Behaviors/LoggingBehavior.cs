using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace MediatorForge.CQRS.Behaviors;

/// <summary>
/// Represents a pipeline behavior that logs the handling of a request and its response.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public class LoggingBehavior<TRequest, TResponse>
    (ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull, IRequest<TResponse>
    where TResponse : notnull
{

    /// <summary>
    /// Handles the logging of the request and its response, including performance logging if the request takes more than 3 seconds.
    /// </summary>
    /// <param name="request">The request to be logged.</param>
    /// <param name="next">The next delegate in the pipeline.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the response from the next delegate in the pipeline.</returns>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        logger.LogInformation("[START] Handle request={Request} - Response={Response} - RequestData={RequestData}",
            typeof(TRequest).Name, typeof(TResponse).Name, request);

        var timer = new Stopwatch();
        timer.Start();

        var response = await next();

        timer.Stop();
        var timeTaken = timer.Elapsed;
        if (timeTaken.Seconds > 3) // if the request is greater than 3 seconds, then log the warnings
            logger.LogWarning("[PERFORMANCE] The request {Request} took {TimeTaken} seconds.",
                typeof(TRequest).Name, timeTaken.Seconds);

        logger.LogInformation("[END] Handled {Request} with {Response}", typeof(TRequest).Name, typeof(TResponse).Name);
        return response;
    }
}
