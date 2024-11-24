using MediatR;

namespace MediatorForge.CQRS;

public interface IAuthorizer<in TRequest>
    where TRequest : IRequest
{
    Task AuthorizeAsync(TRequest request);
}

