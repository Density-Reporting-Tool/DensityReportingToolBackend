namespace DensityReportingToolBackend.Models;

public class ProctorAdditionalJob
{
    public int Id { get; set; }

    public int ProctorId { get; set; }
    public Proctor Proctor { get; set; } = null!;

    public int JobId { get; set; }
    public Job Job { get; set; } = null!;
}
