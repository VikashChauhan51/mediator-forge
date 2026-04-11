namespace MediatorForge.Abstractions;

/// <summary>
/// Defines a pipeline behavior for handling requests and responses in a mediator pattern.
/// Pipeline behaviors allow you to execute logic before and/or after the main request handler,
/// such as validation, logging, caching, or transaction management.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
/// <remarks>
/// <para>
/// Pipeline behaviors are executed in a chain, where each behavior can perform logic
/// before and after invoking the next delegate in the pipeline.
/// </para>
/// <para>
/// The execution order of pipeline behaviors is determined by their registration order
/// or an explicit ordering mechanism such as a <c>PipelineOrder</c> attribute.
/// </para>
/// <para><b>Example:</b></para>
/// <code>
/// // Example pipeline behavior for logging
/// [PipelineOrder(2)]
/// public sealed class LoggingBehavior&lt;TRequest, TResponse&gt; 
///     : IPipelineBehavior&lt;TRequest, TResponse&gt;
///     where TRequest : notnull
/// {
///     public async Task&lt;TResponse&gt; HandleAsync(
///         TRequest request,
///         RequestHandlerDelegate&lt;TResponse&gt; next,
///         CancellationToken ct = default)
///     {
///         Console.WriteLine($"Handling {typeof(TRequest).Name}");
///
///         var response = await next(ct);
///
///         Console.WriteLine($"Handled {typeof(TRequest).Name}");
///
///         return response;
///     }
/// }
///
/// // Example validation behavior
/// [PipelineOrder(1)]
/// public sealed class ValidationBehavior&lt;TRequest, TResponse&gt; 
///     : IPipelineBehavior&lt;TRequest, TResponse&gt;
///     where TRequest : notnull
/// {
///     public async Task&lt;TResponse&gt; HandleAsync(
///         TRequest request,
///         RequestHandlerDelegate&lt;TResponse&gt; next,
///         CancellationToken ct = default)
///     {
///         // Perform validation here
///         // Throw exception if invalid
///
///         return await next(ct);
///     }
/// }
///
/// // Execution order:
/// // ValidationBehavior → LoggingBehavior → Handler
/// </code>
/// </remarks>
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
