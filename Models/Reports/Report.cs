namespace DensityReportingToolBackend.Models;

public class Report
{
    public int Id { get; set; }
    public int JobId { get; set; }
    public Job Job { get; set; } = null!;
    public int EmployeeId { get; set; }
    public GeoPacificEmployee Employee { get; set; } = null!;
    public int ReviewerId { get; set; }
    public GeoPacificEmployee Reviewer { get; set; } = null!;

    public int ReportNumber { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? SubmitDate { get; set; }
    public DateTime? DistributeDate { get; set; }

    public ICollection<ReportPhoto> Photos { get; set; } = [];
    public ICollection<ReportMemo> Memos { get; set; } = [];
    public ICollection<DensityTest> DensityTests { get; set; } = [];

    // Optional reference to a specific distribution list from the job
    public int? DistributionListId { get; set; }
    public DistributionList? DistributionList { get; set; }
}