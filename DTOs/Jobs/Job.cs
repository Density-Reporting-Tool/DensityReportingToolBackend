using DensityReportingToolBackend.DTOs.Labs;
using DensityReportingToolBackend.DTOs.Reports;

namespace DensityReportingToolBackend.DTOs.Jobs;

// Base properties for shared logic
public abstract record JobBaseDto
{
    public string JobNumber { get; init; } = string.Empty;
    public string ClientName { get; init; } = string.Empty;
    public string ProjectName { get; init; } = string.Empty;
    public string SiteAddress { get; init; } = string.Empty;
    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }
}

public record JobReadDto : JobBaseDto
{
    public int Id { get; init; }

    // Navigation Collections mirrored from the Model
    public ICollection<ReportReadDto> Reports { get; init; } = [];
    public ICollection<SitePlanReadDto> SitePlans { get; init; } = [];
    public ICollection<LabTestReadDto> LabTests { get; init; } = [];
    public ICollection<ProctorAdditionalJobReadDto> ProctorAdditionalJobs { get; init; } = [];
    public ICollection<JobNoteReadDto> JobNotes { get; init; } = [];
    public ICollection<DistributionListReadDto> DistributionLists { get; init; } = [];
    public ICollection<JobProjectManagerReadDto> ProjectManagers { get; init; } = [];
    public ICollection<JobSiteContactReadDto> SiteContacts { get; init; } = [];
}

public record JobCreateDto : JobBaseDto
{
    // Usually, collections aren't passed during a simple Job creation, 
    // but they can be added here if you want to support bulk-create.
}

public record JobUpdateDto : JobBaseDto
{
    public int Id { get; init; }
}

public record JobRefDto
{
    public int Id { get; init; }
    public string JobNumber { get; init; } = string.Empty;
}