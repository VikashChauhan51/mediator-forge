namespace MediatorForge.Abstractions;

/// <summary>
/// Defines a pipeline behavior for handling requests and responses in a mediator pattern.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public interface IPipelineBehavior<in TRequest, TResponse>
        where TRequest : notnull
{
    /// <summary>
    /// Handles the incoming request and produces a response. The method is asynchronous, allowing for non-blocking operations.
    /// The cancellation token can be used to cancel the operation if needed.
    /// </summary>
    /// <param name="request">The request to handle.</param>
    /// <param name="next">The next handler in the pipeline.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The response from the pipeline behavior.</returns>
    Task<TResponse> HandleAsync(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct = default
        );
}
