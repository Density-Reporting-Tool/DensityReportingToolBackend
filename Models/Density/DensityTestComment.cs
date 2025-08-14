namespace DensityReportingToolBackend.Models;

public class DensityTestComment
{
    public int Id { get; set; }

    public int CommentId { get; set; }
    public Comment Comment { get; set; } = null!;

    public int DensityTestId { get; set; }
    public DensityTest DensityTest { get; set; } = null!;
}
