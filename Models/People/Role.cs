namespace DensityReportingToolBackend.Models;

public class Role
{
    public int Id { get; set; }
    public required string RoleTitle { get; set; }

    public ICollection<GeoPacificEmployee> Employees { get; set; } = [];
}
