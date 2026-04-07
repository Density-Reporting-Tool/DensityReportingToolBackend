using DensityReportingToolBackend.DTOs.People;

namespace DensityReportingToolBackend.DTOs.Jobs;


public record DistributionMemberReadDto
{
    public int Id { get; init; }
    public int DistributionListId { get; init; }
    public int PersonalInfoId { get; init; }
    public PersonalInfoReadDto PersonalInfo { get; init; } = null!;
}

public record DistributionMemberCreateDto
{
    public int PersonalInfoId { get; init; }
}

public record DistributionMemberUpdateDto
{
    public int Id { get; init; }
    public int PersonalInfoId { get; init; }
}