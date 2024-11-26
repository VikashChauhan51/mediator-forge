using MediatorForge.CQRS.Exceptions;
namespace MediatorApi;

public class ErrorHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlerMiddleware> _logger;

    public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
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

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "An unhandled exception has occurred");

        context.Response.ContentType = "application/problem+json";
        var statusCode = exception switch
        {
            ValidationException => StatusCodes.Status400BadRequest,
            AuthorizationException => StatusCodes.Status403Forbidden,
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            TooManyRequestsException => StatusCodes.Status429TooManyRequests,
            _ => StatusCodes.Status500InternalServerError
        };

        context.Response.StatusCode = statusCode;

        var problemDetails = new HttpValidationProblemDetails
        {
            Status = statusCode,
            Title = statusCode switch
            {
                StatusCodes.Status400BadRequest => "Validation Error",
                StatusCodes.Status403Forbidden => "Access Denied",
                StatusCodes.Status401Unauthorized => "Unauthorized",
                StatusCodes.Status429TooManyRequests => "Too Many Requests",
                _ => "Internal Server Error"
            },
            Detail = exception.Message,
            Instance = context.Request.Path,
            Type = statusCode switch
            {
                StatusCodes.Status400BadRequest => "https://httpstatuses.com/400",
                StatusCodes.Status403Forbidden => "https://httpstatuses.com/403",
                StatusCodes.Status401Unauthorized => "https://httpstatuses.com/401",
                StatusCodes.Status429TooManyRequests => "https://httpstatuses.com/429",
                _ => "https://httpstatuses.com/500"
            }
        };

        if (exception is ValidationException validation)
        {
            Dictionary<string, string[]>? dictionary = validation.Errors?
                            .GroupBy(x => x.PropertyName)
                            .ToDictionary(
                                g => g.Key,
                                g => g.Select(e => e.ErrorMessage).ToArray()
                            );
            if (dictionary != null)
            {
                problemDetails.Errors = dictionary;
            }
        }

        await context.Response.WriteAsJsonAsync(problemDetails);
    }
}



public class TooManyRequestsException : Exception
{
    public TooManyRequestsException(string message) : base(message)
    {
    }
}
