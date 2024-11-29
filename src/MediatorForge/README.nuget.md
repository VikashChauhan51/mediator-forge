# MediatorForge

## Overview

`MediatorForge` is a robust library for integrating validation, authorization, logging, and mediator behaviors in .NET applications. It streamlines the implementation of common application patterns, improving readability and maintainability.

## Features

- **Fluent Validation**: Integrate FluentValidation seamlessly with custom validation systems.
- **Authorization**: Handle authorization logic elegantly with custom exception handling.
- **Logging**: Log requests, responses, and performance metrics effectively.
- **Pipeline Behaviors**: Implement pipeline behaviors for handling cross-cutting concerns in a clean and maintainable way.

## Installation

Install the package via NuGet:

```bash
dotnet add package MediatorForge
```

## Usage

### ValidationBehavior

The `ValidationBehavior` class integrates validation logic into your request pipeline.

#### Example:

```csharp
services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
```

### AuthorizationBehavior

The `AuthorizationBehavior` class handles authorization logic, ensuring requests are authorized before proceeding.

#### Example:

```csharp
services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehavior<,>));
```

### LoggingBehavior

The `LoggingBehavior` class logs requests, responses, and performance metrics.

#### Example:

```csharp
services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
```

### FluentValidatorAdapter

The `FluentValidatorAdapter<TRequest>` integrates FluentValidation validators with a custom validation system.

#### Example:

```csharp
services.AddMediatorForgeFluentValidatorAdapter();
```

### Dependency Injection

Add and configure the necessary services in your `Startup.cs` or `Program.cs` file:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddControllers();
    
    // Add MediatR
    services.AddMediatR(typeof(Startup));

    // Add Validators
    services.AddTransient<IValidator<CreateItemCommand>, CreateItemCommandValidator>();

    // Add Pipeline Behaviors
    services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
    services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehavior<,>));
    services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

    // Add FluentValidatorAdapter
    services.AddMediatorForgeFluentValidatorAdapter();

    // Other service registrations
}
```

### Custom Error Handling Middleware

Ensure your application handles exceptions gracefully and returns meaningful error responses:

```csharp
public class ErrorHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlerMiddleware> _logger;
    private readonly IProblemDetailsService _problemDetailsService;

    public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger, IProblemDetailsService problemDetailsService)
    {
        _next = next;
        _logger = logger;
        _problemDetailsService = problemDetailsService;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "An unhandled exception has occurred");

        context.Response.ContentType = "application/problem+json";
        var statusCode = exception switch
        {
            ValidationException => (int)HttpStatusCode.BadRequest,
            AuthorizationException => (int)HttpStatusCode.Forbidden,
            UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
            TooManyRequestsException => (int)HttpStatusCode.TooManyRequests,
            _ => (int)HttpStatusCode.InternalServerError
        };

        context.Response.StatusCode = statusCode;

        var problemDetails = exception switch
        {
            ValidationException validationException => new ValidationProblemDetails(validationException.Errors)
            {
                Status = statusCode,
                Title = "Validation Error",
                Instance = context.Request.Path
            },
            AuthorizationException => _problemDetailsService.CreateProblemDetails(context, statusCode, "Access Denied", exception.Message, exception),
            UnauthorizedAccessException => _problemDetailsService.CreateProblemDetails(context, statusCode, "Unauthorized", exception.Message, exception),
            TooManyRequestsException => _problemDetailsService.CreateProblemDetails(context, statusCode, "Too Many Requests", exception.Message, exception),
            _ => _problemDetailsService.CreateProblemDetails(context, statusCode, "An unexpected error occurred", exception.Message, exception)
        };

        return context.Response.WriteAsJsonAsync(problemDetails);
    }
}
```

### License

This project is licensed under the MIT License.

