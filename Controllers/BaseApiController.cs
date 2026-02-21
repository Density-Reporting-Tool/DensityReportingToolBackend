using DensityReportingToolBackend.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace DensityReportingToolBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseApiController : ControllerBase
{
    // Standard Success (200 OK)
    protected ActionResult<ApiResponse<T>> Success<T>(T data, string message = "Success")
    {
        return Ok(ApiResponse<T>.SuccessResponse(data, message));
    }

    // Standard Created (201 Created)
    protected ActionResult<ApiResponse<T>> Created<T>(string actionName, object routeValues, T data)
    {
        return CreatedAtAction(actionName, routeValues, ApiResponse<T>.SuccessResponse(data, "Resource created successfully"));
    }

    // Standard Failure (Default 400 Bad Request)
    protected ActionResult<ApiResponse<T>> Failure<T>(string message, int statusCode = 400)
    {
        return StatusCode(statusCode, ApiResponse<T>.FailureResponse(message));
    }
}