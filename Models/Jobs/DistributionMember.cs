namespace DensityReportingToolBackend.Models;

public class DistributionMember
{
    public int Id { get; set; }
    public int DistributionListId { get; set; }
    public DistributionList DistributionList { get; set; } = null!;

    public int PersonalInfoId { get; set; }
    public PersonalInfo PersonalInfo { get; set; } = null!;
}