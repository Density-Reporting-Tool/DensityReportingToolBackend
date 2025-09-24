namespace DensityReportingToolBackend.Models;

public class Report: ModelBaseWithDto<Report, ReportReadDto>
{
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

public class ReportBaseDto
{
    public int JobId { get; set; }
    public int EmployeeId { get; set; }
    public int? ReviewerId { get; set; }

    public int ReportNumber { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? SubmitDate { get; set; }
    public DateTime? DistributeDate { get; set; }

    public int? DistributionListId { get; set; }
}

public class ReportCreateDto : ReportBaseDto { }

public class ReportUpdateDto : ReportBaseDto
{
    public int Id { get; set; }
}

public class ReportReadDto : ReportBaseDto
{
    public int Id { get; set; }

    public string EmployeeName { get; set; } = string.Empty;
    public string ReviewerName { get; set; } = string.Empty;

    public IEnumerable<ReportPhotoReadDto?> Photos { get; set; } = [];
    public IEnumerable<ReportMemoReadDto?> Memos { get; set; } = [];
    public IEnumerable<DensityTestReadDto?> DensityTests { get; set; } = [];

    public JobReadDto? Job { get; set; }
    public PersonalInfoReadDto? Employee { get; set; }
    public PersonalInfoReadDto? Reviewer { get; set; }

    public ReportReadDto(Report report, HashSet<(Type, int)> visited)
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

        EmployeeName = $"{report.Employee?.FirstName} {report.Employee?.LastName}".Trim();
        ReviewerName = $"{report.Reviewer?.FirstName} {report.Reviewer?.LastName}".Trim();

        // Null-safe mapping
        Photos = report.Photos?.Select(p => p?.ToDto(visited)) ?? [];
        Memos = report.Memos?.Select(m => m?.ToDto(visited)) ?? [];
        DensityTests = report.DensityTests?.Select(dt => dt?.ToDto(visited)) ?? [];
        Job = report.Job?.ToDto(visited);
        Employee = report.Employee?.ToDto(visited);
        Reviewer = report.Reviewer?.ToDto(visited);
    }
}