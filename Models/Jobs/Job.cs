namespace DensityReportingToolBackend.Models;

public class Job
{
    public int Id { get; set; }
    
    // Client relationship
    public int ClientId { get; set; }
    public Client Client { get; set; } = null!;
    
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
    public ICollection<JobProjectManager> ProjectManagers { get; set; } = [];
    public ICollection<JobSiteContact> SiteContacts { get; set; } = [];

    // Computed properties for easy access to project managers
    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public ICollection<GeoPacificEmployee> ActiveProjectManagers => 
        ProjectManagers?
            .Where(pm => pm.IsActive && pm.EndDate == null)
            .Select(pm => pm.Employee)
            .ToList() ?? [];

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public GeoPacificEmployee? PrimaryContact => 
        ActiveProjectManagers.FirstOrDefault();

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public ICollection<GeoPacificEmployee> AllProjectManagers => 
        ProjectManagers?
            .Select(pm => pm.Employee)
            .ToList() ?? [];

    // Computed properties for easy access to site contacts
    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public ICollection<PersonalInfo> ActiveSiteContacts => 
        SiteContacts?
            .Where(sc => sc.IsActive && sc.EndDate == null)
            .Select(sc => sc.PersonalInfo)
            .ToList() ?? [];

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public PersonalInfo? PrimarySiteContact => 
        SiteContacts?
            .Where(sc => sc.IsPrimary && sc.IsActive && sc.EndDate == null)
            .Select(sc => sc.PersonalInfo)
            .FirstOrDefault();

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public ICollection<PersonalInfo> AllSiteContacts => 
        SiteContacts?
            .Select(sc => sc.PersonalInfo)
            .ToList() ?? [];

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