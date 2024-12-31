using MediatorForge.Queries;

namespace MediatorForge.Behaviors;

/// <summary>
/// Interface for Query Pipeline Behaviors.
/// </summary>
/// <typeparam name="TQuery"></typeparam>
/// <typeparam name="TResponse"></typeparam>
public interface IQueryBehavior<in TQuery, TResponse> : IBehavior<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
    where TResponse : notnull
{
}
