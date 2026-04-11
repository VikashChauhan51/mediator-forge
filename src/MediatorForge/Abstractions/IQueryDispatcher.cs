namespace MediatorForge.Abstractions;

/// <summary>
/// Dispatcher for queries (read operations).
/// Responsible for sending queries to their respective handlers and returning results.
/// </summary>
/// <remarks>
/// <para>
/// Queries represent read-only operations and should not modify application state.
/// Each query must have exactly one handler that returns the requested data.
/// </para>
/// <para>
/// The dispatcher resolves the appropriate query handler and executes any registered
/// pipeline behaviors before invoking the handler.
/// </para>
/// <para><b>Example:</b></para>
/// <code>
/// // Query
/// public sealed record GetProductByIdQuery(Guid ProductId) : IQuery&lt;ProductDto&gt;;
///
/// // Response DTO
/// public sealed record ProductDto(Guid Id, string Name, decimal Price);
///
/// // Handler
/// public sealed class GetProductByIdHandler 
///     : IHandler&lt;GetProductByIdQuery, ProductDto&gt;
/// {
///     public async Task&lt;ProductDto&gt; HandleAsync(
///         GetProductByIdQuery query,
///         CancellationToken ct)
///     {
///         // Simulate data retrieval (e.g., from database)
///         var product = new ProductDto(
///             query.ProductId,
///             "Laptop",
///             75000m);
///
///         return await Task.FromResult(product);
///     }
/// }
///
/// // Usage
/// var product = await queryDispatcher.QueryAsync&lt;GetProductByIdQuery, ProductDto&gt;(
///     new GetProductByIdQuery(productId));
/// </code>
/// </remarks>
public interface IQueryDispatcher
{
    /// <summary>
    /// Sends a query and returns a response.
    /// </summary>
    /// <typeparam name="TQuery">The type of the query.</typeparam>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <param name="query">The query to send.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The response from the query handler.</returns>
    Task<TResponse> QueryAsync<TQuery, TResponse>(
        TQuery query,
        CancellationToken ct = default)
        where TQuery : IQuery<TResponse>;
}
