namespace DensityReportingToolBackend.Models;

public class JobNote
{
    public int Id { get; set; }

    public int CommentId { get; set; }
    public Comment Comment { get; set; } = null!;

    public int JobId { get; set; }
    public Job Job { get; set; } = null!;
}