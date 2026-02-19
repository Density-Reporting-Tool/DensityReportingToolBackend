namespace DensityReportingToolBackend.DTOs.Reports;

using DensityReportingToolBackend.DTOs.Jobs;
using DensityReportingToolBackend.DTOs.People;
using DensityReportingToolBackend.DTOs.Density;

public abstract record ReportBaseDto
{
    public int JobId { get; init; }
    public int EmployeeId { get; init; }
    public int? ReviewerId { get; init; }
    public int ReportNumber { get; init; }
    public DateTime? StartDate { get; init; }
    public DateTime? SubmitDate { get; init; }
    public DateTime? DistributeDate { get; init; }
    public int? DistributionListId { get; init; }
}

public record ReportReadDto : ReportBaseDto
{
    public int Id { get; init; }
    public PersonalInfoReadDto Employee { get; init; } = null!;
    public PersonalInfoReadDto? Reviewer { get; init; }
    public DistributionListReadDto? DistributionList { get; init; }
    
    public ICollection<ReportPhotoReadDto> Photos { get; init; } = [];
    public ICollection<ReportMemoReadDto> Memos { get; init; } = [];
    public ICollection<DensityTestReadDto> DensityTests { get; init; } = [];
}

public record ReportCreateDto : ReportBaseDto;

public record ReportUpdateDto : ReportBaseDto
{
    public int Id { get; init; }
}