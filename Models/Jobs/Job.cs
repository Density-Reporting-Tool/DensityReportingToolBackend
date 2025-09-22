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