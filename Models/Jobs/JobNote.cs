namespace DensityReportingToolBackend.Models;

public class JobNote: ModelBase
{
    public int JobId { get; set; }
    public Job Job { get; set; } = null!;

    public string Note { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}