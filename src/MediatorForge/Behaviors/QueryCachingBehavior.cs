using MediatorForge.Abstraction;
using MediatorForge.Queries;
using MediatR;

namespace MediatorForge.Behaviors;

/// <summary>
/// Represents a caching behavior for MediatR query requests.
/// </summary>
/// <typeparam name="TQuery">The type of the query request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public sealed class QueryCachingBehavior<TQuery, TResponse> : IQueryBehavior<TQuery, TResponse>
   where TQuery : IQuery<TResponse>
    where TResponse : notnull
{
    private readonly ICache _cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="CachingBehavior{TQuery, TResponse}"/> class.
    /// </summary>
    /// <param name="cache">The cache instance to use.</param>
    public QueryCachingBehavior(ICache cache)
    {
        _cache = cache;
    }

    /// <summary>
    /// Handles the request and caches the response.
    /// </summary>
    /// <param name="request">The request instance.</param>
    /// <param name="next">The delegate to invoke the next behavior in the pipeline.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the response.</returns>
    public async Task<TResponse> Handle(TQuery request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var cacheKey = typeof(TQuery).GetHashCode().ToString();
        var cachedResponse = await _cache.GetAsync<TResponse>(cacheKey);
        if (cachedResponse != null)
        {
            return cachedResponse;
        }
        var response = await next();
        await _cache.SetAsync(cacheKey, response);

        return response;
    }
}
