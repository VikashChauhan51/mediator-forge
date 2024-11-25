using Microsoft.AspNetCore.Mvc;
using MediatR;
using MediatorApi.Commands;
using MediatorApi.Notifications;
using MediatorApi.Queries;
using MediatorForge.CQRS.Exceptions;

namespace MediatorApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ItemsResultController : ControllerBase
{
    private readonly IMediator _mediator;

    public ItemsResultController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateItem([FromBody] CreateItemResultCommand command)
    {
        var result = await _mediator.Send(command);

        return await result.Match<Task<IActionResult>>
         (
                async id =>
             {
                 await _mediator.Publish(new ItemCreatedNotification { ItemId = id });
                 return Ok(id);
             },
             error =>
             {
                 return error switch
                 {
                     ValidationException validationException => Task.FromResult<IActionResult>(ValidationProblem(new ValidationProblemDetails
                     {
                         Title = validationException.Message,
                         Status = 400,
                         Errors = validationException.Errors
                         .GroupBy(x => x.PropertyName).
                         ToDictionary(
                             g => g.Key,
                         g => g.Select(e => e.ErrorMessage).ToArray())
                     })),
                     AuthorizationException => Task.FromResult<IActionResult>(Forbid()),
                     _ => Task.FromResult<IActionResult>(BadRequest(error))
                 };
             }
         );


    }
}
