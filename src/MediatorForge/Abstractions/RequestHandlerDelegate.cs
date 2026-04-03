namespace MediatorForge.Abstractions;

/// <summary>
/// Represents a delegate that defines the signature for handling a request and returning a response asynchronously. This delegate is used in the context of a mediator pattern to process requests and generate responses, allowing for decoupling between the sender and receiver of the request.
/// </summary>
/// <typeparam name="TResponse">The type of the response.</typeparam>
/// <param name="ct">The cancellation token.</param>
/// <returns>A task representing the asynchronous operation.</returns>
public delegate Task<TResponse> RequestHandlerDelegate<TResponse>(
    CancellationToken ct = default);
