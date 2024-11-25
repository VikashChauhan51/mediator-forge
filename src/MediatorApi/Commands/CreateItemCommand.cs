using MediatorForge.CQRS.Commands;
using MediatorForge.CQRS.Interfaces;
using MediatorForge.Results;
using MediatorForge.Utilities;

namespace MediatorApi.Commands;

public class CreateItemCommand : IResultCommand<Guid>
{
    public string Name { get; set; }
    public string Description { get; set; }
}


public class CreateItemCommandValidator : IValidator<CreateItemCommand>
{
    public Task<ValidationResult> ValidateAsync(CreateItemCommand request)
    {

        return Task.FromResult(ValidationResult.Success);
    }
}
public class CreateItemCommandHandler : IResultCommandHandler<CreateItemCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateItemCommand request, CancellationToken cancellationToken)
    {
        // Business logic to create an item and return its ID
        var itemId = Guid.NewGuid();
        return Result<Guid>.Succ(itemId);
    }
}

