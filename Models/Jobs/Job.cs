namespace DensityReportingToolBackend.Models;

public class Job
{
    public int Id { get; set; }
    public int ProjectManagerId { get; set; }
    public GeoPacificEmployee ProjectManager { get; set; } = null!;

    public required string ClientName { get; set; }
    public required string ProjectName { get; set; }
    public required string SiteAddress { get; set; }

    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public ICollection<JobContractor> JobContracts { get; set; } = [];
    public ICollection<Report> Reports { get; set; } = [];
    public ICollection<SitePlan> SitePlans { get; set; } = [];
    public ICollection<LabTest> LabTests { get; set; } = [];
    public ICollection<ProctorAdditionalJob> ProctorAdditionalJobs { get; set; } = [];
    public ICollection<JobNote> JobNotes { get; set; } = [];
}