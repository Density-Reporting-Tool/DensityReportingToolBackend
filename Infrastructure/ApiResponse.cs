namespace DensityReportingToolBackend.Infrastructure;

public record ApiResponse<T>(
    bool Success, 
    string Message, 
    T? Data = default, 
    IEnumerable<string>? Errors = null,
    string? StackTrace = null)
{
    // Quick Success helper
    public static ApiResponse<T> SuccessResponse(T data, string message = "Success") 
        => new(true, message, data);

    // Quick Failure helper for single messages
    public static ApiResponse<T> FailureResponse(string message, IEnumerable<string>? errors = null) 
        => new(false, message, default, errors);

    // Helper for FluentValidation error lists
    public static ApiResponse<T> ValidationError(IEnumerable<string> errors) 
        => new(false, "Validation failed", default, errors);
}