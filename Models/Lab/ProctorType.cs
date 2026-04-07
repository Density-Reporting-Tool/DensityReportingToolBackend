namespace DensityReportingToolBackend.Models;

public class ProctorType
{
    public int Id { get; set; }
    public required string Type { get; set; }   // e.g., "SPDD", "MPDD"
    public ICollection<Proctor> Proctors { get; set; } = [];
}
