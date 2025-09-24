namespace DensityReportingToolBackend.Models;

public class Report: ModelBase
{
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

    public ReportReadDto ToDTO()
    {
        return new ReportReadDto(this);
    }
}

public class ReportBaseDto
{
    public int JobId { get; set; }
    public int EmployeeId { get; set; }
    public int ReviewerId { get; set; }

    public int ReportNumber { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? SubmitDate { get; set; }
    public DateTime? DistributeDate { get; set; }

    public int? DistributionListId { get; set; }
}

public class ReportCreateDto : ReportBaseDto { }

public class ReportUpdateDto : ReportBaseDto { }

public class ReportReadDto : ReportBaseDto
{
    public int Id { get; set; }

    public string EmployeeName { get; set; } = string.Empty;
    public string ReviewerName { get; set; } = string.Empty;

    public IEnumerable<ReportPhoto> Photos { get; set; } = [];
    public IEnumerable<ReportMemo> Memos { get; set; } = [];
    public IEnumerable<DensityTest> DensityTests { get; set; } = [];

    public ReportReadDto(Report report)
    {
        Id = report.Id;
        JobId = report.JobId;
        EmployeeId = report.EmployeeId;
        ReviewerId = report.ReviewerId;

        ReportNumber = report.ReportNumber;
        StartDate = report.StartDate;
        SubmitDate = report.SubmitDate;
        DistributeDate = report.DistributeDate;
        DistributionListId = report.DistributionListId;

        EmployeeName = report.Employee?.PersonalInfo?.FirstName ?? string.Empty;
        ReviewerName = report.Reviewer?.PersonalInfo?.FirstName ?? string.Empty;
    }
}