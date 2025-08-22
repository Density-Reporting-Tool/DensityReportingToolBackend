namespace DensityReportingToolBackend.Models;

public class JobSiteContact
{
    public int Id { get; set; }
    
    public int JobId { get; set; }
    public Job Job { get; set; } = null!;
    
    public int PersonalInfoId { get; set; }
    public PersonalInfo PersonalInfo { get; set; } = null!;
    
    public string? Area { get; set; }           // e.g., "North Site", "Excavation Area", "Paving Section"
    public string? Company { get; set; }        // e.g., "ABC Excavation", "XYZ Paving"
    public string? Role { get; set; }           // e.g., "Site Supervisor", "Safety Officer", "Foreman"
    public bool IsPrimary { get; set; } = false; // Primary site contact for the job
    
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }      // Null means currently active
    
    public string? Notes { get; set; }          // e.g., "Available 6AM-6PM", "Weekend emergency contact"
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? LastModifiedDate { get; set; }
}
