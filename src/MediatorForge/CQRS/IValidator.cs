using MediatorForge.Results;
using MediatR;

namespace MediatorForge.CQRS;

public interface IValidator<in TRequest>
    where TRequest : IRequest
{
    Task<ValidationResult> ValidateAsync(TRequest request);
}
