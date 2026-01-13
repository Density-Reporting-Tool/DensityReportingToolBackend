using DensityReportingToolBackend.DTOs.Jobs;

namespace DensityReportingToolBackend.Models;

public class JobProjectManager: ModelBase
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