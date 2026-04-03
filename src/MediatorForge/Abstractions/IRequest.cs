namespace MediatorForge.Abstractions;

/// <summary>
/// Represents a request that can be sent through a mediator to receive a response of type <typeparamref name="TResponse"/>.
///  This interface is used to define the structure of requests in the mediator pattern, allowing for decoupling between the sender and receiver of the request.
///  Implementing this interface indicates that a class is a request that can be processed by a request handler to produce a response.
/// </summary>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public interface IRequest<TResponse>;
