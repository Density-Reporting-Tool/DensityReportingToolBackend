namespace DensityReportingToolBackend.DTOs.Labs;

using DensityReportingToolBackend.DTOs.Jobs;

public abstract record LabTestBaseDto
{
    public int JobId { get; init; }
    public string? MaterialType { get; init; }
    public string? ImportLocation { get; init; }
    public DateTime? ReceiveDate { get; init; }
}

public record LabTestReadDto : LabTestBaseDto
{
    public int Id { get; init; }
    public ICollection<SieveReadDto> Sieves { get; init; } = [];
}

public record LabTestCreateDto : LabTestBaseDto;

public record LabTestUpdateDto : LabTestBaseDto
{
    public int Id { get; init; }
}