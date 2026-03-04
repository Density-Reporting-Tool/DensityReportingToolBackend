namespace DensityReportingToolBackend.Models;

public class JobEvent: ModelBase
{
   

    public int JobId { get; set; }
    public Job Job { get; set; } = null!;

    public int PersonalInfoId { get; set; }
    public PersonalInfo PersonalInfo { get; set; } = null!;

    public DateTimeOffset StartDateTime { get; set; } 
    public DateTimeOffset EndDateTime { get; set; }
    public string? Description { get; set; }
    public string? Status { get; set; }
    public int? CreatedById { get; set; }
    public GeoPacificEmployee? CreatedBy { get; set; } = null!;
    public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;


}

