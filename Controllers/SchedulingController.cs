using DensityReportingToolBackend.DTOs.Calendar;
using DensityReportingToolBackend.Infrastructure;
using DensityReportingToolBackend.Services;
using Microsoft.AspNetCore.Mvc;

namespace DensityReportingToolBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SchedulingController(ISchedulingService schedulingService, ILogger<SchedulingController> logger)
    : BaseApiController
{
    [HttpPost("events")]
    public async Task<ActionResult<ApiResponse<ScheduleJobReadDto>>> CreateEvent(
        [FromBody] ScheduleJobCreateDto dto)
    {
        var result = await schedulingService.CreateJobEventAsync(dto);
        return Created(nameof(GetEventById), new { id = result.Id }, result);
    }

    [HttpGet("events/{id:int}")]
    public async Task<ActionResult<ApiResponse<ScheduleJobReadDto>>> GetEventById(int id)
    {
        var result = await schedulingService.GetJobEventByIdAsync(id);
        return Success(result);
    }

    [HttpPut("events/{id:int}")]
    public async Task<ActionResult<ApiResponse<ScheduleJobReadDto>>> UpdateEvent(
        int id,
        [FromBody] ScheduleJobUpdateDto dto)
    {
        var result = await schedulingService.UpdateJobEventAsync(id, dto);
        return Success(result, "Event updated successfully");
    }

    [HttpDelete("events/{id:int}")]
    public async Task<ActionResult<ApiResponse<ScheduleJobReadDto>>> DeleteEvent(int id)
    {
        var result = await schedulingService.DeleteJobEventAsync(id);
        return Success(result, "Event deleted successfully");
    }

    [HttpGet("events")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ScheduleJobReadDto>>>> GetEventsInRange(
    [FromQuery] DateTimeOffset start,
    [FromQuery] DateTimeOffset end)
    {
        if (start >= end)
            return Failure<IEnumerable<ScheduleJobReadDto>>("start must be before end", 400);

        var events = await schedulingService.GetEventsInRangeAsync(start, end);
        return Success(events);
    }
}