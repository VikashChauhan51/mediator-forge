﻿using Microsoft.AspNetCore.Mvc;
using MediatR;
using MediatorApi.Commands;
using MediatorApi.Notifications;
using MediatorApi.Queries;
using MediatorForge.CQRS.Exceptions;

namespace MediatorApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ItemsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ItemsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateItem([FromBody] CreateItemCommand command)
    {
        var result = await _mediator.Send(command);

        return await result.Match<Task<IActionResult>>
 (
        async id =>
     {
         await _mediator.Publish(new ItemCreatedNotification { ItemId = id });
         return Ok(id);
     },
     () =>
     {

         return Task.FromResult<IActionResult>(BadRequest());
     }
 );


    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetItem(Guid id)
    {
        var query = new GetItemQuery { Id = id };
        var result = await _mediator.Send(query);

        return result.Match<IActionResult>
            (
              onSuccess: item =>
              {
                  return Ok(item);
              },
              onFailure: error =>
              {
                  return NotFound();
              }
            );
    }
}
