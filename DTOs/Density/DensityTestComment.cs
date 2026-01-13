namespace DensityReportingToolBackend.DTOs.Density;

public record DensityTestCommentReadDto
{
    public int Id { get; init; }
    public int CommentId { get; init; }
    // public CommentReadDto Comment { get; init; } = null!; TODO Rethink Comments
    public int DensityTestId { get; init; }
}

public record DensityTestCommentCreateDto
{
    public int CommentId { get; init; }
    public int DensityTestId { get; init; }
}