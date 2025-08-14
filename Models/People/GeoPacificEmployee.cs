namespace DensityReportingToolBackend.Models;

public class GeoPacificEmployee : PersonalInfo
{
    public int RoleId { get; set; }
    public Role Role { get; set; } = null!;
}
