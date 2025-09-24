namespace DensityReportingToolBackend.Models;

public class Job: ModelBaseWithDto<Job, JobReadDto>
{
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
}

public static class JobExtensions
{
    public static IEnumerable<PersonalInfo> GetActiveProjectManagers(this Job job) =>
        job.ProjectManagers?
            .Where(pm => pm.IsActive && pm.EndDate == null)
            .Select(pm => pm.PersonalInfo)
        ?? [];

    public static PersonalInfo? GetActiveProjectManager(this Job job) =>
        job.GetActiveProjectManagers().FirstOrDefault();

    public static IEnumerable<PersonalInfo> GetAllProjectManagers(this Job job) =>
        job.ProjectManagers?.Select(pm => pm.PersonalInfo)
        ?? [];

    public static IEnumerable<PersonalInfo> GetActiveSiteContacts(this Job job) =>
        job.SiteContacts?.Where(sc => sc.IsActive)
            .Select(sc => sc.PersonalInfo)
        ?? [];

    public static IEnumerable<PersonalInfo> GetAllSiteContacts(this Job job) =>
        job.SiteContacts?.Select(sc => sc.PersonalInfo)
        ?? [];

    public static IEnumerable<Proctor> GetDirectProctors(this Job job) =>
        job.LabTests?.SelectMany(lt => lt.Proctors)
        ?? [];

    public static IEnumerable<Proctor> GetReusedProctors(this Job job) =>
        job.ProctorAdditionalJobs?.Select(paj => paj.Proctor)
        ?? [];
}

public class JobBaseDto
{
    public string JobNumber { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public string ProjectName { get; set; } = string.Empty;
    public string SiteAddress { get; set; } = string.Empty;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

public class JobCreateDto : JobBaseDto
{ 
    public IEnumerable<JobNoteReadDto?> JobNotes { get; set; } = [];
}
public class JobUpdateDto : JobBaseDto { }
public class JobReadDto : JobBaseDto
{
    public int Id { get; set; }
    public IEnumerable<JobNoteReadDto?> JobNotes { get; set; } = [];
    public IEnumerable<ReportReadDto?> Reports { get; set; } = [];
    public IEnumerable<JobProjectManagerReadDto?> ProjectManagers { get; set; } = [];
    public IEnumerable<JobSiteContactReadDto?> SiteContacts { get; set; } = [];

    public JobReadDto(Job job, HashSet<(Type, int)> visited)
    {
        Id = job.Id;
        JobNumber = job.JobNumber;
        ClientName = job.ClientName;
        ProjectName = job.ProjectName;
        SiteAddress = job.SiteAddress;
        StartDate = job.StartDate;
        EndDate = job.EndDate;

        // Null-safe mapping
        Reports = job.Reports?.Select(r => r?.ToDTO()) ?? [];
        ProjectManagers = job.ProjectManagers?.Select(pm => pm?.ToDto(visited)) ?? [];
        SiteContacts = job.SiteContacts?.Select(sc => sc?.ToDto(visited)) ?? [];
    }
}