namespace MediatorForge.Abstractions;

/// <summary>
/// Represents a query that can be sent through a mediator to receive a response of type <typeparamref name="TResponse"/>.
///  This interface is used to define the structure of queries in the mediator pattern, allowing for decoupling between the sender and receiver of the query.
///  Implementing this interface indicates that a class is a query that can be processed by a query handler to produce a response.
/// </summary>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public interface IQuery<TResponse> : IRequest<TResponse>;
