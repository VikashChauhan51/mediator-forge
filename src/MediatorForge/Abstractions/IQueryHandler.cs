namespace MediatorForge.Abstractions;

/// <summary>
/// Represents a handler that processes a query of type <typeparamref name="TQuery"/> and produces a response of type <typeparamref name="TResponse"/>.
///  This interface is used to define the structure of query handlers in the mediator pattern, allowing for decoupling between the sender and receiver of the query.
/// Implementing this interface indicates that a class is a query handler that can process a specific type of query and return a response.
/// </summary>
/// <typeparam name="TQuery">The type of the query.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public interface IQueryHandler<in TQuery, TResponse> : IHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>;
