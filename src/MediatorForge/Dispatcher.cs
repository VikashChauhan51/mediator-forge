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
    public Task<TResponse> QueryAsync<TQuery, TResponse>(TQuery query, CancellationToken ct = default) where TQuery : Abstractions.IQuery<TResponse>
    {
        ArgumentNullException.ThrowIfNull(query);
        var handler = serviceProvider.GetRequiredService<IHandler<TQuery, TResponse>>();

        var behaviors = serviceProvider.GetServices<IPipelineBehavior<TQuery, TResponse>>().ToList();

        RequestHandlerDelegate<TResponse> pipeline = ct => handler.HandleAsync(query, ct);

        for (var i = behaviors.Count - 1; i >= 0; i--)
        {
            var behavior = behaviors[i];
            var next = pipeline;
            pipeline = ct => behavior.HandleAsync(query, next, ct);
        }

        return pipeline(ct);
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">Thrown when the command is null.</exception>
    public Task<TResponse> SendAsync<TCommand, TResponse>(TCommand command, CancellationToken ct = default) where TCommand : ICommand<TResponse>
    {
        ArgumentNullException.ThrowIfNull(command);
        var handler = serviceProvider.GetRequiredService<IHandler<TCommand, TResponse>>();

        var behaviors = serviceProvider.GetServices<IPipelineBehavior<TCommand, TResponse>>().ToList();

        RequestHandlerDelegate<TResponse> pipeline = ct => handler.HandleAsync(command, ct);

        for (var i = behaviors.Count - 1; i >= 0; i--)
        {
            var behavior = behaviors[i];
            var next = pipeline;
            pipeline = ct => behavior.HandleAsync(command, next, ct);
        }
        return pipeline(ct);
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">Thrown when the command is null.</exception>
    public Task SendAsync<TCommand>(TCommand command, CancellationToken ct = default) where TCommand : Abstractions.ICommand
    {
        ArgumentNullException.ThrowIfNull(command);
        var handler = serviceProvider.GetRequiredService<IHandler<TCommand, Unit>>();

        var behaviors = serviceProvider.GetServices<IPipelineBehavior<TCommand, Unit>>().ToList();

        RequestHandlerDelegate<Unit> pipeline = ct => handler.HandleAsync(command, ct);
        for (var i = behaviors.Count - 1; i >= 0; i--)
        {
            var behavior = behaviors[i];
            var next = pipeline;
            pipeline = ct => behavior.HandleAsync(command, next, ct);
        }
        return pipeline(ct);
    }

    /// <inheritdoc/>
    public async Task PublishAsync<TEvent>(TEvent eventData, CancellationToken ct = default)
      where TEvent : notnull
    {
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

    /// <inheritdoc/>
    public async Task PublishAllAsync<TEvent>(TEvent eventData, CancellationToken ct = default)
    {
        var notification = new EventNotification<TEvent>(eventData);
        var handlers = serviceProvider.GetServices<IEventNotificationHandler<TEvent>>().ToList();
        var tasks = handlers.Select(h => h.HandleAsync(notification, ct));
        await Task.WhenAll(tasks);
    }

}
