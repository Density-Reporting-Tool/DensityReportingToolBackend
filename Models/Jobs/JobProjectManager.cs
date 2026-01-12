using DensityReportingToolBackend.DTOs.Jobs;

namespace DensityReportingToolBackend.Models;

public class JobProjectManager: ModelBaseWithDto<JobProjectManager, JobProjectManagerReadDto>
{
    public int JobId { get; set; }
    public Job Job { get; set; } = null!;

    public int PersonalInfoId { get; set; }
    public PersonalInfo PersonalInfo { get; set; } = null!;

    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }  // Null means currently active

    public string? Notes { get; set; }      // e.g., "Primary contact", "Backup contact", "Available 9-5"
    public bool IsActive { get; set; } = true;

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? LastModifiedDate { get; set; }
}

public class JobProjectManagerBaseDto
{
    public int JobId { get; set; }
    public int PersonalInfoId { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public string? Notes { get; set; }
    public bool IsActive { get; set; }
}

public class JobProjectManagerCreateDto : JobProjectManagerBaseDto { }

public class JobProjectManagerUpdateDto : JobProjectManagerBaseDto { }

public class JobProjectManagerReadDto : JobProjectManagerBaseDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }

    public PersonalInfoReadDto? PersonalInfo { get; set; }
    public JobReadDto? Job { get; set; }

    public JobProjectManagerReadDto(JobProjectManager pm, HashSet<(Type, int)> visited)
    {
        Id = pm.Id;
        JobId = pm.JobId;
        PersonalInfoId = pm.PersonalInfoId;

        StartDate = pm.StartDate;
        EndDate = pm.EndDate;
        Notes = pm.Notes;
        IsActive = pm.IsActive;

        FullName = $"{pm.PersonalInfo?.FirstName} {pm.PersonalInfo?.LastName}".Trim();
        CreatedDate = pm.CreatedDate;
        LastModifiedDate = pm.LastModifiedDate;

        PersonalInfo = pm.PersonalInfo?.ToDto(visited);
    }
}