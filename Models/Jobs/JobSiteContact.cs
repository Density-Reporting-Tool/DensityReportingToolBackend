namespace DensityReportingToolBackend.Models;

public class JobSiteContact: ModelBaseWithDto<JobSiteContact, JobSiteContactReadDto>
{
    public int JobId { get; set; }
    public Job Job { get; set; } = null!;

    public int PersonalInfoId { get; set; }
    public PersonalInfo PersonalInfo { get; set; } = null!;

    public string? Area { get; set; }           // e.g., "North Site", "Excavation Area", "Paving Section"
    public string? Company { get; set; }        // e.g., "ABC Excavation", "XYZ Paving"
    public string? Role { get; set; }           // e.g., "Site Supervisor", "Safety Officer", "Foreman"
    public bool IsPrimary { get; set; } = false; // Primary site contact for the job


    public string? Notes { get; set; }          // e.g., "Available 6AM-6PM", "Weekend emergency contact"
    public bool IsActive { get; set; } = true;

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? LastModifiedDate { get; set; }
}


public class JobSiteContactBaseDto
{
    public int JobId { get; set; }
    public int PersonalInfoId { get; set; }

    public string? Area { get; set; }
    public string? Company { get; set; }
    public string? Role { get; set; }
    public bool IsPrimary { get; set; }
    public string? Notes { get; set; }
    public bool IsActive { get; set; }
}

public class JobSiteContactCreateDto : JobSiteContactBaseDto { }

public class JobSiteContactUpdateDto : JobSiteContactBaseDto { }

public class JobSiteContactReadDto : JobSiteContactBaseDto
{
    public int Id { get; set; }
    public string? ContactName { get; set; } = string.Empty; // from PersonalInfo
    public DateTime CreatedDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }

    public PersonalInfoReadDto? PersonalInfo { get; set; }

    public JobSiteContactReadDto(JobSiteContact contact, HashSet<(Type, int)> visited)
    {
        Id = contact.Id;
        JobId = contact.JobId;
        PersonalInfoId = contact.PersonalInfoId;

        Area = contact.Area;
        Company = contact.Company;
        Role = contact.Role;
        IsPrimary = contact.IsPrimary;
        Notes = contact.Notes;
        IsActive = contact.IsActive;

        ContactName = contact.PersonalInfo?.FirstName + " " + contact.PersonalInfo?.LastName;
        CreatedDate = contact.CreatedDate;
        LastModifiedDate = contact.LastModifiedDate;

        PersonalInfo = contact.PersonalInfo?.ToDto(visited);
    }
}