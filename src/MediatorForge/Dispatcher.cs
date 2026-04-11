using Microsoft.Extensions.DependencyInjection;
using MediatorForge.Abstractions;

namespace MediatorForge;

/// <summary>
/// The <see cref="Dispatcher"/> class is responsible for dispatching commands and queries to their respective handlers. It uses the built-in dependency injection container to resolve handlers and pipeline behaviors. The dispatcher supports both command and query handling, allowing for a clean separation of concerns in your application.
/// </summary>
/// <param name="serviceProvider">The service provider used to resolve handlers and pipeline behaviors.</param>
public sealed class Dispatcher(IServiceProvider serviceProvider) : IDispatcher
{
    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">Thrown when the query is null.</exception>
    public async Task<TResponse> QueryAsync<TQuery, TResponse>(TQuery query, CancellationToken ct = default) where TQuery : Abstractions.IQuery<TResponse>
    {
        ArgumentNullException.ThrowIfNull(query);
        ct.ThrowIfCancellationRequested();

        var handler = serviceProvider.GetRequiredService<IHandler<TQuery, TResponse>>();
        var behaviors = serviceProvider.GetServices<IPipelineBehavior<TQuery, TResponse>>();

        var pipeline = BuildPipeline(query, behaviors, handler);

        return await pipeline(ct).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">Thrown when the command is null.</exception>
    public async Task<TResponse> SendAsync<TCommand, TResponse>(TCommand command, CancellationToken ct = default) where TCommand : ICommand<TResponse>
    {
        ArgumentNullException.ThrowIfNull(command);
        ct.ThrowIfCancellationRequested();

        var handler = serviceProvider.GetRequiredService<IHandler<TCommand, TResponse>>();
        var behaviors = serviceProvider.GetServices<IPipelineBehavior<TCommand, TResponse>>();

        var pipeline = BuildPipeline(command, behaviors, handler);

        return await pipeline(ct).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">Thrown when the command is null.</exception>
    public async Task SendAsync<TCommand>(TCommand command, CancellationToken ct = default) where TCommand : Abstractions.ICommand
    {
        ArgumentNullException.ThrowIfNull(command);
        ct.ThrowIfCancellationRequested();

        var handler = serviceProvider.GetRequiredService<IHandler<TCommand, Unit>>();
        var behaviors = serviceProvider.GetServices<IPipelineBehavior<TCommand, Unit>>();

        var pipeline = BuildPipeline(command, behaviors, handler);

        await pipeline(ct).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task PublishAsync<TEvent>(TEvent eventData, CancellationToken ct = default)
      where TEvent : notnull
    {
        ArgumentNullException.ThrowIfNull(eventData);
        ct.ThrowIfCancellationRequested();

        var notification = new EventNotification<TEvent>(eventData);
        var handlers = serviceProvider.GetServices<IEventNotificationHandler<TEvent>>().ToList();

        if (handlers.Count == 0)
        {
            return;
        }

        foreach (var handler in handlers)
        {
            await handler.HandleAsync(notification, ct).ConfigureAwait(false);
        }
    }

    private static RequestHandlerDelegate<TResponse> BuildPipeline<TRequest, TResponse>(
       TRequest request,
       IEnumerable<IPipelineBehavior<TRequest, TResponse>> behaviors,
       IHandler<TRequest, TResponse> handler)
       where TRequest : IRequest<TResponse>
    {
        RequestHandlerDelegate<TResponse> pipeline =
            ct => handler.HandleAsync(request, ct);

        foreach (var behavior in behaviors.Reverse())
        {
            var next = pipeline;
            pipeline = ct => behavior.HandleAsync(request, next, ct);
        }

        return pipeline;
    }

}
