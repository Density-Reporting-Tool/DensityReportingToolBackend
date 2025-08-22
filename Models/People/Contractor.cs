namespace DensityReportingToolBackend.Models;

public class Contractor
{
    public int Id { get; set; }

    // Link to PersonalInfo
    public int PersonalInfoId { get; set; }
    public PersonalInfo PersonalInfo { get; set; } = null!;

    // Company information
    public string CompanyName { get; set; } = string.Empty;
    
    // Optional: Link to Client if the company is a registered client
    public int? ClientId { get; set; }
    public Client? Client { get; set; }

    // Job relationships
    public ICollection<JobContractor> JobContracts { get; set; } = [];
}

