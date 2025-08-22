namespace DensityReportingToolBackend.Models;

public class GeoPacificEmployee
{
    public int Id { get; set; }
    
    // Link to PersonalInfo
    public int PersonalInfoId { get; set; }
    public PersonalInfo PersonalInfo { get; set; } = null!;
    
    // Employee-specific fields
    public int RoleId { get; set; }
    public Role Role { get; set; } = null!;
    
    public string Password { get; set; } = string.Empty;
    
    // Navigation properties for jobs
    public ICollection<JobProjectManager> ManagedJobs { get; set; } = [];
}
