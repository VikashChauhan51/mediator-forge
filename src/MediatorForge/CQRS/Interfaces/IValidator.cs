using MediatorForge.Utilities;
using MediatR;

namespace MediatorForge.CQRS.Interfaces;

/// <summary>
/// Defines a contract for validating a specific request.
/// </summary>
/// <typeparam name="TRequest">The type of the request to validate.</typeparam>
public interface IValidator<in TRequest>
    where TRequest : IRequest
{
    /// <summary>
    /// Asynchronously validates the specified request.
    /// </summary>
    /// <param name="request">The request to validate.</param>
    /// <returns>A task that represents the asynchronous validation operation. The task result contains the <see cref="ValidationResult"/>.</returns>
    Task<ValidationResult> ValidateAsync(TRequest request);
}
