using MediatorForge.CQRS.Commands;
using MediatorForge.CQRS.Interfaces;
using MediatorForge.Results;
using MediatorForge.Utilities;

namespace MediatorApi.Commands;

public class CreateItemCommand : ICommand<Option<Guid>>
{
    public string Name { get; set; }
    public string Description { get; set; }
}


public class CreateItemCommandValidator : IValidator<CreateItemCommand>
{
    public Task<ValidationResult> ValidateAsync(CreateItemCommand request, CancellationToken cancellationToken = default)
    {

        return Task.FromResult(ValidationResult.Failure( new List<ValidationError>()
        {
            new ValidationError("test","required","")
        }));
    }
}
public class CreateItemCommandHandler : ICommandHandler<CreateItemCommand, Option<Guid>>
{
    public async Task<Option<Guid>> Handle(CreateItemCommand request, CancellationToken cancellationToken)
    {
        // Business logic to create an item and return its ID
        var itemId = Guid.NewGuid();
        return Option<Guid>.Some(itemId);
    }
}

