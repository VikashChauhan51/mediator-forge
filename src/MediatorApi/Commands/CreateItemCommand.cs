using Akka.Actor;
using Akka.DependencyInjection;
using MediatorForge.CQRS.Commands;
using MediatorForge.CQRS.Validators;
using ResultifyCore;
using MediatorForge.Utilities;
using MediatR;

namespace MediatorApi.Commands;

public class CreateItemCommand : ICommand<Guid>
{
    public string Name { get; set; }
    public string Description { get; set; }
}


public class CreateItemCommandValidator : IValidator<CreateItemCommand>
{
    public Task<ValidationResult> ValidateAsync(CreateItemCommand request, CancellationToken cancellationToken = default)
    {

        return Task.FromResult(ValidationResult.Success);
    }
}


public class CreateItemCommandHandler : IRequestHandler<CreateItemCommand, Guid>
{
    private readonly IActorRef _itemActor;

    public CreateItemCommandHandler(ActorSystem actorSystem)
    {
        var props = DependencyResolver.For(actorSystem).Props<ItemActor>();
        _itemActor = actorSystem.ActorOf(props, "itemActor");
    }

    public async Task<Guid> Handle(CreateItemCommand request, CancellationToken cancellationToken)
    {
        var itemId = await _itemActor.Ask<Guid>(request, cancellationToken);
        return itemId;
    }
}



public class ItemActor : ReceiveActor
{
    public ItemActor()
    {
        ReceiveAsync<CreateItemCommand>(async command =>
        {
            var itemId = Guid.NewGuid();
            Sender.Tell(itemId);
        });
    }
}

public class CreateItemOptionCommand : ICommand<Option<Guid>>
{
    public string Name { get; set; }
    public string Description { get; set; }
}


public class CreateItemOptionCommandValidator : IValidator<CreateItemOptionCommand>
{
    public Task<ValidationResult> ValidateAsync(CreateItemOptionCommand request, CancellationToken cancellationToken = default)
    {

        return Task.FromResult(ValidationResult.Failure( new List<ValidationError>()
        {
            new ValidationError("test","required","")
        }));
    }
}
public class CreateItemOptionCommandHandler : ICommandHandler<CreateItemOptionCommand, Option<Guid>>
{
    public async Task<Option<Guid>> Handle(CreateItemOptionCommand request, CancellationToken cancellationToken)
    {
        // Business logic to create an item and return its ID
        var itemId = Guid.NewGuid();
        return Option<Guid>.Some(itemId);
    }
}

// result pattern:
public class CreateItemResultCommand : ICommand<Result<Guid>>
{
    public string Name { get; set; }
    public string Description { get; set; }
}


public class CreateItemResultCommandValidator : IValidator<CreateItemResultCommand>
{
    public Task<ValidationResult> ValidateAsync(CreateItemResultCommand request, CancellationToken cancellationToken = default)
    {

        return Task.FromResult(ValidationResult.Failure(new List<ValidationError>()
        {
            new ValidationError("test","required","")
        }));
    }
}
public class CreateItemResultCommandHandler : ICommandHandler<CreateItemResultCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateItemResultCommand request, CancellationToken cancellationToken)
    {
        // Business logic to create an item and return its ID
        var itemId = Guid.NewGuid();
        return Result<Guid>.Succ(itemId);
    }
}
