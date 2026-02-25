namespace DensityReportingToolBackend.DTOs.Labs;

public abstract record ProctorTypeBaseDto
{
    public string Type { get; init; } = string.Empty;
}

public record ProctorTypeReadDto : ProctorTypeBaseDto
{
    public int Id { get; init; }
}

public record ProctorTypeCreateDto : ProctorTypeBaseDto;