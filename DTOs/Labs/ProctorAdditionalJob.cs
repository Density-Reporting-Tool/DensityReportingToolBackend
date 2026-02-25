namespace DensityReportingToolBackend.DTOs.Labs;

using DensityReportingToolBackend.DTOs.Jobs;

public record ProctorAdditionalJobReadDto
{
    public int Id { get; init; }
    public int ProctorId { get; init; }
    public int JobId { get; init; }
    public JobReadDto Job { get; init; } = null!;
}

public record ProctorAdditionalJobCreateDto
{
    public int ProctorId { get; init; }
    public int JobId { get; init; }
}