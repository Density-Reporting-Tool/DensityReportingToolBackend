namespace DensityReportingToolBackend.Models;

public class ShotPlacement
{
    public int Id { get; set; }

    public int SitePlanId { get; set; }
    public SitePlan SitePlan { get; set; } = null!;

    public int DensityTestId { get; set; }
    public DensityTest DensityTest { get; set; } = null!;

    public double XCoordinate { get; set; }
    public double YCoordinate { get; set; }
}
