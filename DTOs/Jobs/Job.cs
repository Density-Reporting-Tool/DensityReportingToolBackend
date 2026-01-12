namespace DensityReportingToolBackend.DTOs.Jobs;

public abstract record JobBaseDto
{
    public string JobNumber { get; init; } = string.Empty;
    public string ClientName { get; init; } = string.Empty;
    public string ProjectName { get; init; } = string.Empty;
    public string SiteAddress { get; init; } = string.Empty;
    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }
}

public record JobReadDto : JobBaseDto
{
    public int Id { get; init; }
}

public record JobCreateDto : JobBaseDto
{
    
}

public record JobUpdateDto : JobBaseDto
{
    public int Id { get; init; }
}