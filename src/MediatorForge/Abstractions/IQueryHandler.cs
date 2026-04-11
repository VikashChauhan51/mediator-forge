namespace MediatorForge.Abstractions;

/// <summary>
/// Represents a handler that processes a query of type <typeparamref name="TQuery"/>
/// and produces a response of type <typeparamref name="TResponse"/>.
/// </summary>
/// <typeparam name="TQuery">The type of the query.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
/// <remarks>
/// <para>
/// Query handlers are responsible for handling read-only operations.
/// They retrieve data without modifying the state of the system.
/// </para>
/// <para>
/// Each query must have exactly one corresponding handler.
/// </para>
/// <para>
/// It is recommended to return DTOs (Data Transfer Objects) instead of domain entities
/// to maintain separation between read and write models.
/// </para>
/// <para><b>Example:</b></para>
/// <code>
/// // Query
/// public sealed record GetCustomerByIdQuery(Guid CustomerId) : IQuery&lt;CustomerDto&gt;;
///
/// // Response DTO
/// public sealed record CustomerDto(Guid Id, string Name, string Email);
///
/// // Handler
/// public sealed class GetCustomerByIdHandler 
///     : IQueryHandler&lt;GetCustomerByIdQuery, CustomerDto&gt;
/// {
///     public async Task&lt;CustomerDto&gt; HandleAsync(
///         GetCustomerByIdQuery query,
///         CancellationToken ct)
///     {
///         // Simulate data retrieval (e.g., from database)
///         var customer = new CustomerDto(
///             query.CustomerId,
///             "John Doe",
///             "john@example.com");
///
///         return await Task.FromResult(customer);
///     }
/// }
///
/// // Usage
/// var customer = await dispatcher.QueryAsync&lt;GetCustomerByIdQuery, CustomerDto&gt;(
///     new GetCustomerByIdQuery(customerId));
/// </code>
/// </remarks>
public interface IQueryHandler<in TQuery, TResponse> : IHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>;
