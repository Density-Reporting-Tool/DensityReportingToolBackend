namespace DensityReportingToolBackend.Models;

public class JobNote: ModelBase
{
    public int JobId { get; set; }
    public Job Job { get; set; } = null!;

    public string Note { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    JobNoteReadDto ToDTO()
    {
        return new JobNoteReadDto(this);
    }
}

public class JobNoteBaseDto
{
    public int JobId { get; set; }
    public string Note { get; set; } = string.Empty;
}

public class JobNoteCreateDto : JobNoteBaseDto { }

public class JobNoteUpdateDto : JobNoteBaseDto { }

public class JobNoteReadDto : JobNoteBaseDto
{
    public int Id { get; set; }
    public DateTime CreatedDate { get; set; }

    public JobNoteReadDto(JobNote jobNote)
    {
        Id = jobNote.Id;
        JobId = jobNote.JobId;
        Note = jobNote.Note;
        CreatedDate = jobNote.CreatedDate;
    }
}