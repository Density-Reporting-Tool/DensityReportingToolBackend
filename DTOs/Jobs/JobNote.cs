namespace DensityReportingToolBackend.DTOs.Jobs;

public abstract record JobNoteBaseDto
{
    public string Note { get; init; } = string.Empty;
    public DateTime CreatedDate { get; init; } = DateTime.UtcNow;
    public int? CommentId { get; init; }
}

public record JobNoteReadDto : JobNoteBaseDto
{
    public int Id { get; init; }
    public int JobId { get; init; }
}

public record JobNoteCreateDto : JobNoteBaseDto
{
}

public record JobNoteUpdateDto : JobNoteBaseDto
{
    public int Id { get; init; }
}