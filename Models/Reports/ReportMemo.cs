namespace DensityReportingToolBackend.Models;

public class ReportMemo
{
    public int Id { get; set; }

    public int ReportId { get; set; }
    public Report Report { get; set; } = null!;

    public string? Purpose { get; set; }
    public string? CommentsAndObservations { get; set; }
    public string? Conclusion { get; set; }

    public DateTime? CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }

    public ICollection<MemoComment> Comments { get; set; } = [];
}