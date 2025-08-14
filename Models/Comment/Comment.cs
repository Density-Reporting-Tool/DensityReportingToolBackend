namespace DensityReportingToolBackend.Models;

public class Comment
{
    public int Id { get; set; }

    public int EmployeeId { get; set; }
    public GeoPacificEmployee Employee { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
    public string Details { get; set; } = string.Empty;

    public ICollection<JobNote> JobNotes { get; set; } = [];
    public ICollection<MemoComment> MemoComments { get; set; } = [];
    public ICollection<DensityTestComment> DensityTestComments { get; set; } = [];
}
