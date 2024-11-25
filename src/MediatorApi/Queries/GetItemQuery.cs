using MediatorForge.CQRS.Queries;
using MediatorForge.Results;

namespace MediatorApi.Queries;

public class GetItemQuery : IQuery<Result<ItemDto>>
{
    public Guid Id { get; set; }
}

public class GetItemQueryHandler : IQueryHandler<GetItemQuery, Result<ItemDto>>
{
    public async Task<Result<ItemDto>> Handle(GetItemQuery request, CancellationToken cancellationToken)
    {
        // Business logic to get an item by ID
        var item = new ItemDto
        {
            Id = request.Id,
            Name = "Sample Item",
            Description = "This is a sample item."
        };
        return await Task.FromResult(Result<ItemDto>.Succ(item));
    }
}

public class ItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}

