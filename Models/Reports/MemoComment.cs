namespace DensityReportingToolBackend.Models;

public class MemoComment
{
    public int Id { get; set; }

    public int CommentId { get; set; }
    public Comment Comment { get; set; } = null!;

    public int MemoId { get; set; }
    public ReportMemo Memo { get; set; } = null!;
}
