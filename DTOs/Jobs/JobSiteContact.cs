namespace DensityReportingToolBackend.DTOs.Jobs;

using DensityReportingToolBackend.DTOs.People;

public abstract record JobSiteContactBaseDto
{
    public string? Area { get; init; }
    public string? Company { get; init; }
    public string? Role { get; init; }
    public bool IsPrimary { get; init; }
    public string? Notes { get; init; }
    public bool IsActive { get; init; } = true;
}

public record JobSiteContactReadDto : JobSiteContactBaseDto
{
    public int Id { get; init; }
    public int JobId { get; init; }
    public int PersonalInfoId { get; init; }
    public PersonalInfoReadDto PersonalInfo { get; init; } = null!;
    public DateTime CreatedDate { get; init; }
    public DateTime? LastModifiedDate { get; init; }
}

public record JobSiteContactCreateDto : JobSiteContactBaseDto
{
    public int PersonalInfoId { get; init; }
}

public record JobSiteContactUpdateDto : JobSiteContactBaseDto
{
    public int Id { get; init; }
}