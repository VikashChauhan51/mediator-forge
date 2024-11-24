using MediatR;

namespace MediatorForge.CQRS.Subscriptions;

/// <summary>
/// Handle ISubscription.
/// </summary>
/// <typeparam name="TSubscription"></typeparam>
public interface ISubscriptionHandler<in TSubscription>
    : ISubscriptionHandler<TSubscription, Unit>
    where TSubscription : ISubscription<Unit>
{
}

/// <summary>
/// Handle ISubscription.
/// </summary>
/// <typeparam name="TSubscription"></typeparam>
/// <typeparam name="TResponse"></typeparam>
public interface ISubscriptionHandler<in TSubscription, TResponse>
    : IRequestHandler<TSubscription, TResponse>
    where TSubscription : ISubscription<TResponse>
    where TResponse : notnull
{
}