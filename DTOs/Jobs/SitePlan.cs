using DensityReportingToolBackend.DTOs.Density;

namespace DensityReportingToolBackend.DTOs.Jobs;

public abstract record SitePlanBaseDto
{
    public string SitePlanUrl { get; init; } = string.Empty;
}

public record SitePlanReadDto : SitePlanBaseDto
{
    public int Id { get; init; }
    public int JobId { get; init; }
    public IEnumerable<ShotPlacementReadDto> ShotPlacements { get; init; } = [];
}

public record SitePlanCreateDto : SitePlanBaseDto;

public record SitePlanUpdateDto : SitePlanBaseDto
{
    public int Id { get; init; }
}