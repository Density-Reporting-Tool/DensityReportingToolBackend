namespace DensityReportingToolBackend.DTOs.Jobs;

using DensityReportingToolBackend.DTOs.People;

public abstract record JobProjectManagerBaseDto
{
    public DateTime StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public string? Notes { get; init; }
    public bool IsActive { get; init; } = true;
}

public record JobProjectManagerReadDto : JobProjectManagerBaseDto
{
    public int Id { get; init; }
    public int JobId { get; init; }
    public int PersonalInfoId { get; init; }
    public PersonalInfoReadDto PersonalInfo { get; init; } = null!;
    public DateTime CreatedDate { get; init; }
    public DateTime? LastModifiedDate { get; init; }
}

public record JobProjectManagerCreateDto : JobProjectManagerBaseDto
{
    public int PersonalInfoId { get; init; }
}

public record JobProjectManagerUpdateDto : JobProjectManagerBaseDto
{
    public int Id { get; init; }
}