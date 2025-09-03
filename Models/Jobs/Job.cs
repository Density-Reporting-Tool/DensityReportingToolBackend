namespace DensityReportingToolBackend.Models;

public class Job
{
    public int Id { get; set; }
    
    // Job number - can be numeric (e.g., "25482") or alphanumeric (e.g., "15827-A")
    public required string JobNumber { get; set; }
    
    // Client name as a simple string
    public required string ClientName { get; set; }
    
    public required string ProjectName { get; set; }
    public required string SiteAddress { get; set; }

    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }


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
    public ICollection<PersonalInfo> ActiveProjectManagers => 
        ProjectManagers?
            .Where(pm => pm.IsActive && pm.EndDate == null)
            .Select(pm => pm.PersonalInfo)
            .ToList() ?? [];

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public PersonalInfo? ActiveProjectManager => 
        ActiveProjectManagers.FirstOrDefault();

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public ICollection<PersonalInfo> AllProjectManagers => 
        ProjectManagers?
            .Select(pm => pm.PersonalInfo)
            .ToList() ?? [];

    // Computed properties for easy access to site contacts
    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public ICollection<PersonalInfo> ActiveSiteContact => 
        SiteContacts?
            .Where(sc => sc.IsActive && sc.EndDate == null)
            .Select(sc => sc.PersonalInfo)
            .ToList() ?? [];

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public ICollection<PersonalInfo> AllSiteContacts => 
        SiteContacts?
            .Select(sc => sc.PersonalInfo)
            .ToList() ?? [];

    // Proctors that belong directly to this job (through LabTest)
    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public ICollection<Proctor> DirectProctors => LabTests?.SelectMany(lt => lt.Proctors).ToList() ?? [];

    // Proctors from other jobs that are being reused for this job
    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public ICollection<Proctor> ReusedProctors => ProctorAdditionalJobs?.Select(paj => paj.Proctor).ToList() ?? [];
}