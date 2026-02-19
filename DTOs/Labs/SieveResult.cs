using DensityReportingToolBackend.Models;

namespace DensityReportingToolBackend.DTOs.Labs;

public record SieveResultReadDto
{
    public int Id { get; init; }
    public int SieveId { get; init; }
    public SieveSize SieveSize { get; init; }
    public double GramsRetained { get; init; }
    public int OrderIndex { get; init; }
}

public record SieveResultCreateDto
{
    public SieveSize SieveSize { get; init; }
    public double GramsRetained { get; init; }
    public int OrderIndex { get; init; }
}

// Mirroring the SieveCalcs Row logic for the API response
public record SieveRowDto(
    SieveResultReadDto Result,
    double PercentRetained,
    double CumulativePercentRetained,
    double PercentPassing
);