namespace DensityReportingToolBackend.Models;

public class Report
{
    public int Id { get; set; }
    public int JobId { get; set; }
    public Job Job { get; set; } = null!;
    
    // References to PersonalInfo instead of GeoPacificEmployee
    public int EmployeeId { get; set; }
    public PersonalInfo Employee { get; set; } = null!;
    
    // Nullable ReviewerId since reports can be created without a reviewer initially
    public int? ReviewerId { get; set; }
    public PersonalInfo? Reviewer { get; set; }

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