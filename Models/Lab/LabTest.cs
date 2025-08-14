namespace DensityReportingToolBackend.Models;

public class LabTest
{
    public int Id { get; set; }

    public int JobId { get; set; }
    public Job Job { get; set; } = null!;

    public string? MaterialType { get; set; }
    public string? ImportLocation { get; set; }
    public DateTime? ReceiveDate { get; set; }

    public ICollection<Sieve> Sieves { get; set; } = [];
    public ICollection<Proctor> Proctors { get; set; } = [];
}
