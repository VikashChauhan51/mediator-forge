using Microsoft.AspNetCore.Mvc;
using MediatR;
using MediatorApi.Commands;
using MediatorApi.Notifications;
using MediatorApi.Queries;
using MediatorForge.CQRS.Exceptions;
using MediatorForge.CQRS.Notifications;
using ResultifyCore;

namespace MediatorApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ItemsResultController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ItemsResultController> logger;

    public ItemsResultController(IMediator mediator, ILogger<ItemsResultController> logger)
    {
        _mediator = mediator;
        this.logger = logger;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateItem([FromBody] CreateItemResultCommand command)
    {
        var result = await _mediator.Send(command);

        result
        .Do(res => this.logger.LogInformation("Executing common action"))
        .Tap(value => this.logger.LogInformation($"Tap into value: {value}"))
        .Map(value => value)
        .OnSuccess(value => this.logger.LogInformation($"Transformed value: {value}"));


        return await result.Match<Task<IActionResult>>
         (
                async id =>
             {
                 await _mediator.Publish(new EventNotification<ItemCreatedNotification>(new ItemCreatedNotification { ItemId = id }));
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
                     UnauthorizedAccessException => Task.FromResult<IActionResult>(Challenge()),
                     _ => Task.FromResult<IActionResult>(BadRequest(error))
                 };
             }
         );


    }
}
