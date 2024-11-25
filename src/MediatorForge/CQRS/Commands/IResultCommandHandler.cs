using MediatorForge.Results;
using MediatR;

namespace MediatorForge.CQRS.Commands;

/// <summary>
/// Handle IResultCommand Request.
/// </summary>
/// <typeparam name="TCommand">The type of the command.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public interface IResultCommandHandler<in TCommand, TResponse>
    : IRequestHandler<TCommand, Result<TResponse>>
    where TCommand : IResultCommand<TResponse>
{
}
