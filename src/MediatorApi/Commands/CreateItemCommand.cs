using MediatorForge.CQRS.Commands;
using MediatorForge.Results;

namespace MediatorApi.Commands;

public class CreateItemCommand : ICommand<Result<Guid>>
{
    public string Name { get; set; }
    public string Description { get; set; }
}

public class CreateItemCommandHandler : ICommandHandler<CreateItemCommand, Result<Guid>>
{

    public async Task<Result<Guid>> Handle(CreateItemCommand request, CancellationToken cancellationToken)
    {

        // Business logic to create an item and return its ID
        var itemId = Guid.NewGuid();
        return Result<Guid>.Succ(itemId);
    }
}

