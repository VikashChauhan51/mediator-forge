using Microsoft.AspNetCore.Mvc;
using MediatR;
using MediatorApi.Commands;
using MediatorApi.Notifications;
using MediatorApi.Queries;
using MediatorForge.CQRS.Exceptions;
using MediatorForge.CQRS.Notifications;

namespace MediatorApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ItemsOptionController : ControllerBase
{
    private readonly IMediator _mediator;

    public ItemsOptionController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateItem([FromBody] CreateItemOptionCommand command)
    {
        var result = await _mediator.Send(command);

        return await result.Match<Task<IActionResult>>
         (
                async id =>
             {
                 await _mediator.Publish(new EventNotification<ItemCreatedNotification>(new ItemCreatedNotification { ItemId = id }));
                 return Ok(id);
             },
             () =>
             {
                 return Task.FromResult<IActionResult>(BadRequest());
             }
         );


    }
}
