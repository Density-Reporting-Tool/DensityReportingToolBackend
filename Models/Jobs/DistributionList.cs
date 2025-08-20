namespace DensityReportingToolBackend.Models;

public class DistributionList
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    
    public ICollection<DistributionMember> DistributionMembers { get; set; } = [];
}
