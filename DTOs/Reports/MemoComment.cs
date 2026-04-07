namespace DensityReportingToolBackend.DTOs.Reports;

public record MemoCommentReadDto
{
    public int Id { get; init; }
    public int CommentId { get; init; }
    // public CommentReadDto Comment { get; init; } = null!; TODO rethink Comments
    public int MemoId { get; init; }
    public ReportMemoReadDto Memo { get; init; } = null!;
}

public record MemoCommentCreateDto
{
    public int CommentId { get; init; }
    public int MemoId { get; init; }
}