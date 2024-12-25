using MediatorForge.Utilities;
using MediatR;

namespace MediatorForge.CQRS.Validators;

/// <summary>
/// Defines a contract for validating a specific request.
/// </summary>
/// <typeparam name="TRequest">The type of the request to validate.</typeparam>
public interface IValidator<in TRequest>
{
    /// <summary>
    /// Asynchronously validates the specified request.
    /// </summary>
    /// <param name="request">The request to validate.</param>
    /// <param name="cancellationToken">The request cancellation token.</param>
    /// <returns>A task that represents the asynchronous validation operation. The task result contains the <see cref="ValidationResult"/>.</returns>
   Task<ValidationResult> ValidateAsync(TRequest request, CancellationToken cancellationToken = default);
}
