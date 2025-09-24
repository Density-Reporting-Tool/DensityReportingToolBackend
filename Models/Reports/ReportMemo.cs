namespace DensityReportingToolBackend.Models;

public class ReportMemo: ModelBaseWithDto<ReportMemo, ReportMemoReadDto>
{
    public int ReportId { get; set; }
    public Report Report { get; set; } = null!;

    public string? Purpose { get; set; }
    public string? CommentsAndObservations { get; set; }
    public string? Conclusion { get; set; }

    public DateTime? CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }

    public ICollection<MemoComment> Comments { get; set; } = [];
}

public class ReportMemoBaseDto
{
    public int ReportId { get; set; }
    public string? Purpose { get; set; }
    public string? CommentsAndObservations { get; set; }
    public string? Conclusion { get; set; }
}

public class ReportMemoCreateDto : ReportMemoBaseDto { }

public class ReportMemoUpdateDto : ReportMemoBaseDto
{
    public int Id { get; set; }
}

public class ReportMemoReadDto : ReportMemoBaseDto
{
    public int Id { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }

    public ReportMemoReadDto(ReportMemo memo, HashSet<(Type, int)> visited)
    {
        Id = memo.Id;
        ReportId = memo.ReportId;
        Purpose = memo.Purpose;
        CommentsAndObservations = memo.CommentsAndObservations;
        Conclusion = memo.Conclusion;
        CreatedDate = memo.CreatedDate;
        UpdatedDate = memo.UpdatedDate;
    }
}