namespace DensityReportingToolBackend.Models;

public class Client
{
    public int Id { get; set; }
    public required string Name { get; set; }
    
    // Optional: Additional company information
    public string? Address { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    
    // Relationships
    public ICollection<Contractor> Contractors { get; set; } = [];
    public ICollection<Job> Jobs { get; set; } = [];
}
