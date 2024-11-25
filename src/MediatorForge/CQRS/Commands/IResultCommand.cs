using MediatorForge.Results;
using MediatR;

namespace MediatorForge.CQRS.Commands;
public interface IResultCommand<TResponse> : IRequest<Result<TResponse>>
{
}


public interface IResultPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, Result<TResponse>>
{
}
