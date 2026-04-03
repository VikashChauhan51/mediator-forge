namespace MediatorForge.Abstractions;

/// <summary>
/// Dispatcher for queries (read operations).
/// </summary>
public interface IQueryDispatcher
{
    /// <summary>
    /// Sends a query and returns a response. The query is processed by the appropriate query handler, which returns a response of any type. Queries are used to retrieve data or perform read-only operations, while commands are used to modify state or perform actions. The QueryAsync method is used for queries, while the SendAsync method is used for commands.
    /// </summary>
    /// <typeparam name="TQuery">The type of the query.</typeparam>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <param name="query">The query to send.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The response from the query handler.</returns>
    /// <returns></returns>
    Task<TResponse> QueryAsync<TQuery, TResponse>(
        TQuery query,
        CancellationToken ct = default)
        where TQuery : IQuery<TResponse>;
}
