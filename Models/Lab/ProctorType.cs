namespace DensityReportingToolBackend.Models;

public class ProctorType
{
    public int Id { get; set; }
    public required string Type { get; set; }   // e.g., "Standard", "Modified"
    public ICollection<Proctor> Proctors { get; set; } = [];
}
