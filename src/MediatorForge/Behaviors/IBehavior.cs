using MediatR;

namespace MediatorForge.Behaviors;

/// <summary>
/// Interface for Pipeline Behaviors.
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
public interface IBehavior<in TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : notnull
{
}
