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
    public ICollection<DistributionList> DistributionLists { get; set; } = [];

    // Convenience property to directly access proctors (not mapped to database)
    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public ICollection<Proctor> Proctors => ProctorAdditionalJobs?.Select(paj => paj.Proctor).ToList() ?? [];

    // Proctors that belong directly to this job (through LabTest)
    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public ICollection<Proctor> DirectProctors => LabTests?.SelectMany(lt => lt.Proctors).ToList() ?? [];

    // Proctors from other jobs that are being reused for this job
    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public ICollection<Proctor> ReusedProctors => ProctorAdditionalJobs?.Select(paj => paj.Proctor).ToList() ?? [];
}