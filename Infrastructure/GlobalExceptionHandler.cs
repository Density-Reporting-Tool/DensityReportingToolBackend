namespace DensityReportingToolBackend.Infrastructure;

using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Hosting; // Required for IsDevelopment()

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IWebHostEnvironment _env; // Inject the environment

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IWebHostEnvironment env)
    {
        _logger = logger;
        _env = env;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "An unhandled exception occurred: {Message}", exception.Message);

        var (statusCode, message, errors) = exception switch
        {
            ValidationException fluentEx => (
                StatusCodes.Status400BadRequest,
                "Validation failed",
                fluentEx.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}")
            ),
            KeyNotFoundException => (
                StatusCodes.Status404NotFound,
                "The requested resource was not found.",
                null
            ),
            _ => (
                StatusCodes.Status500InternalServerError,
                "An unexpected error occurred on the server.",
                null
            )
        };

        // Determine if we should show the stack trace
        string? stackTrace = _env.IsDevelopment() ? exception.StackTrace : null;

        var response = new ApiResponse<object>(
            Success: false,
            Message: message,
            Errors: errors,
            StackTrace: stackTrace
        );

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/json";

        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

        return true; 
    }
}