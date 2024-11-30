# mediator-forge

MediatorForge is a robust library designed to streamline the implementation of common patterns in .NET applications. It provides integration for validation, authorization, logging, and mediator behaviors, enhancing code readability and maintainability.

## Features

- **Fluent Validation**: Seamless integration with FluentValidation for request validation.
- **Authorization Logic**: Secure access control with customizable authorization behaviors.
- **Logging**: Monitor and record request processing and performance metrics.
- **Command and Query Handling**: Simplified handling using the Mediator pattern.
- **Result and Option Types**: Effective handling of operation outcomes.

## Installation

Install the package via NuGet:

```bash
dotnet add package MediatorForge
```

## Usage

### Register Services

Add and configure the necessary services in your `Startup.cs` or `Program.cs` file:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddControllers();

    // Add MediatR
    services.AddMediatR(typeof(Startup).Assembly);

    // Add Validators
    services.AddTransient<IValidator<CreateItemCommand>, CreateItemCommandValidator>();

    // Add Pipeline Behaviors
    services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
    services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehavior<,>));
    services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

    // Register default authorizer and validator
    services.AddTransient(typeof(IAuthorizer<>), typeof(DefaultAuthorizer<>));
    services.AddTransient(typeof(IValidator<>), typeof(DefaultValidator<>));

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

---

# MediatorForge.Adapters

MediatorForge.Adapters provides a seamless integration of MediatorForge validators with custom validation systems in .NET applications. This adapter ensures consistent validation logic across different application layers, simplifying the integration process and enhancing code maintainability.

## Features

- **Seamless Integration**: Easily connect MediatorForge validators with your custom validation systems.
- **Enhanced Code Maintainability**: Simplifies the integration of validation logic.
- **Consistent Validation**: Ensures consistent validation logic across different layers.
- **Extensible**: Allows customization of validation logic as needed.

## Installation

Install the package via NuGet:

```bash
dotnet add package MediatorForge.Adapters
```

## Usage

After installing the package, register the adapter in your `Startup.cs` or `Program.cs`:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    // Register MediatorForge and other necessary services
    services.AddMediatorForge(typeof(Startup).Assembly);

    // Register FluentValidatorAdapter
    services.AddMediatorForgeFluentValidatorAdapter();

    // Other service registrations
}
```

## License

This project is licensed under the MIT License.

