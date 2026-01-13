namespace DensityReportingToolBackend.DTOs.Density;

using DensityReportingToolBackend.DTOs.Jobs; // Assuming SitePlanReadDto is here

public abstract record ShotPlacementBaseDto
{
    public int SitePlanId { get; init; }
    public int? DensityTestId { get; init; }
    public double XCoordinate { get; init; }
    public double YCoordinate { get; init; }
}

public record ShotPlacementReadDto : ShotPlacementBaseDto
{
    public int Id { get; init; }
    public SitePlanReadDto SitePlan { get; init; } = null!;
    public DensityTestReadDto? DensityTest { get; init; }
}

public record ShotPlacementCreateDto : ShotPlacementBaseDto;

public record ShotPlacementUpdateDto : ShotPlacementBaseDto
{
    public int Id { get; init; }
}