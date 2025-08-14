namespace DensityReportingToolBackend.Models;

public class Contractor
{
    public int Id { get; set; }

    public int DetailsId { get; set; }
    public PersonalInfo Details { get; set; } = null!;

    public ICollection<JobContractor> JobContracts { get; set; } = [];
}

