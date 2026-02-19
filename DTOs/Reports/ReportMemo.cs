namespace DensityReportingToolBackend.DTOs.Reports;

public abstract record ReportMemoBaseDto
{
    public int ReportId { get; init; }
    public string? Purpose { get; init; }
    public string? CommentsAndObservations { get; init; }
    public string? Conclusion { get; init; }
    public DateTime? CreatedDate { get; init; }
    public DateTime? UpdatedDate { get; init; }
}

public record ReportMemoReadDto : ReportMemoBaseDto
{
    public int Id { get; init; }
    public ReportReadDto Report { get; init; } = null!;
    public ICollection<MemoCommentReadDto> Comments { get; init; } = [];
}

public record ReportMemoCreateDto : ReportMemoBaseDto;

public record ReportMemoUpdateDto : ReportMemoBaseDto
{
    public int Id { get; init; }
}