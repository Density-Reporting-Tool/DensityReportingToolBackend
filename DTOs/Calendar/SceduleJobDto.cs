using DensityReportingToolBackend.DTOs.Jobs;
using DensityReportingToolBackend.DTOs.People;

namespace DensityReportingToolBackend.DTOs.Calendar;

public abstract record ScheduleJobBaseDto
{
    public int JobId { get; init; }
    public int PersonalInfoId { get; init; }
    public DateTimeOffset StartDateTime { get; init; }
    public DateTimeOffset EndDateTime { get; init; }
    public string? Description { get; init; } = string.Empty;
    public string? Status { get; set; }  // e.g. "Scheduled", "Cancelled"

    public int? CreatedById { get; set; }
    public GeoPacificEmployeeReadDto? CreatedBy { get; set; }

    public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;
}   

public record ScheduleJobReadDto : ScheduleJobBaseDto
{
    public int Id { get; init; }
    public JobReadDto Job { get; init; } = null!;
    public PersonalInfoReadDto PersonalInfo { get; init; } = null!;
}

public record ScheduleJobCreateDto : ScheduleJobBaseDto{}

public record ScheduleJobUpdateDto : ScheduleJobBaseDto
{
    public int Id { get; init; }
}
