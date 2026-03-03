namespace DensityReportingToolBackend.Models;

public class JobEvent: ModelBase
{
   

    public int JobId { get; set; }
    public Job Job { get; set; } = null!;

    public int GeoPacificEmployeeId { get; set; }
    public GeoPacificEmployee GeoPacificEmployee { get; set; } = null!;

    public DateTimeOffset StartDateTime { get; set; } 
    public DateTimeOffset EndDateTime { get; set; }
    public string? Description { get; set; }
    public string? Location { get; set; }
    public string? Status { get; set; }
    public int? CreatedById { get; set; }
    public GeoPacificEmployee? CreatedBy { get; set; } = null!;
    public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;


}

