namespace DensityReportingToolBackend.DTOs.Jobs;

public abstract record DistributionListBaseDto
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
}

public record DistributionListReadDto : DistributionListBaseDto
{
    public int Id { get; init; }
    public int JobId { get; init; }
    public IEnumerable<DistributionMemberReadDto> DistributionMembers { get; init; } = [];
}

public record DistributionListCreateDto : DistributionListBaseDto
{

}

public record DistributionListUpdateDto : DistributionListBaseDto
{
    public int Id { get; init; }
}