namespace DensityReportingToolBackend.Models;

public class SitePlan
{
    public int Id { get; set; }

    public int JobId { get; set; }
    public Job Job { get; set; } = null!;

    public required string SitePlanUrl { get; set; }

    public ICollection<ShotPlacement> ShotPlacements { get; set; } = [];
}
