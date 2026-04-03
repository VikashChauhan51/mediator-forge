namespace MediatorForge.Abstractions;

/// <summary>
/// Represents a handler that processes a request of type <typeparamref name="TRequest"/> and produces a response of type <typeparamref name="TResponse"/>.
///  This interface is used to define the structure of handlers in the mediator pattern, allowing for decoupling between the sender and receiver of the request.
/// Implementing this interface indicates that a class is a handler that can process a specific type of request and return a response.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public interface IHandler<in TRequest, TResponse>
     where TRequest : IRequest<TResponse>
{
    /// <summary>
    /// Handles the incoming request and produces a response. The method is asynchronous, allowing for non-blocking operations.
    ///  The cancellation token can be used to cancel the operation if needed.
    ///  The implementation of this method will contain the logic to process the request and generate the appropriate response based on the type of request being handled.
    /// </summary>
    /// <param name="request">The request to handle.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The response from the handler.</returns>
    Task<TResponse> HandleAsync(TRequest request, CancellationToken ct = default);

}
