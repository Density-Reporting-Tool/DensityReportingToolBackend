namespace DensityReportingToolBackend.Models;

public class JobContractor
{
    public int JobId { get; set; }
    public Job Job { get; set; } = null!;

    public int ContractorId { get; set; }
    public Contractor Contractor { get; set; } = null!;
}