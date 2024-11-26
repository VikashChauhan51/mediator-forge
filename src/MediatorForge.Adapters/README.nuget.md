# MediatorForge.Adapters

## Overview

The `FluentValidatorAdapter<TRequest>` is an adapter class that integrates `FluentValidation` validators with a custom validation system. This adapter allows you to use `FluentValidation` within your validation pipeline seamlessly.

## Installation

To install the `FluentValidatorAdapter`, run the following command in the NuGet Package Manager Console:

```bash
dotnet add package MediatorForge.Adapters
```

## Usage

### Adding to Service Collection

Add and configure the `FluentValidatorAdapter` to your `IServiceCollection` in your `Startup.cs` or `Program.cs` file:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    // Add MediatR
    services.AddMediatR(typeof(Startup));

    // Add FluentValidation
    services.AddTransient<IValidator<CreateItemCommand>, CreateItemCommandValidator>();

    // Add FluentValidatorAdapter
    services.AddMediatorForgeFluentValidatorAdapter();

    // Other service registrations
}
```

### Defining Your Request and Command

Define your request and create a corresponding command:

```csharp
public class CreateItemCommand : IRequest<Guid>
{
    public string Name { get; set; }
    public string Description { get; set; }
}
```

### Creating a Validator

Create a validator for your request using FluentValidation:

```csharp
using FluentValidation;

public class CreateItemCommandValidator : AbstractValidator<CreateItemCommand>
{
    public CreateItemCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
        RuleFor(x => x.Description).NotEmpty().WithMessage("Description is required.");
    }
}
```

### Implementing Command Handler

Create a handler for your command:

```csharp
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

public class CreateItemCommandHandler : IRequestHandler<CreateItemCommand, Guid>
{
    public Task<Guid> Handle(CreateItemCommand request, CancellationToken cancellationToken)
    {
        // Business logic to handle the command
        var itemId = Guid.NewGuid();
        return Task.FromResult(itemId);
    }
}
```

### Creating a Controller

Create a controller to handle the HTTP requests:

```csharp
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class ItemsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ItemsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateItem([FromBody] CreateItemCommand command)
    {
        var itemId = await _mediator.Send(command);
        return Ok(itemId);
    }
}
```

### Example Request and Response

#### Example Request

```json
POST /api/items
{
    "name": "NewItem",
    "description": "Description of the new item"
}
```

#### Expected Response

```json
{
    "itemId": "generated-item-id"
}
```

## FluentValidatorAdapter<TRequest> Class

```csharp
/// <summary>
/// Adapter class to integrate FluentValidation validators with the custom validation system.
/// </summary>
/// <typeparam name="TRequest">The type of the request to be validated.</typeparam>
public class FluentValidatorAdapter<TRequest>(FluentValidation.IValidator<TRequest> fluentValidator) : IValidator<TRequest>
    where TRequest : IRequest
{
    /// <inheritdoc/>
    /// <returns>A task that represents the asynchronous validation operation. The task result contains the <see cref="ValidationResult"/>.</returns>
    public async Task<ValidationResult> ValidateAsync(TRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = await fluentValidator.ValidateAsync(request);

        if (validationResult.IsValid)
        {
            return ValidationResult.Success;
        }

        var errors = validationResult.Errors.Select(e => new ValidationError(e.PropertyName, e.ErrorMessage, e.AttemptedValue));
        return ValidationResult.Failure(errors);
    }
}
```

## License

This project is licensed under the MIT License.

---
