namespace DensityReportingToolBackend.DTOs.Labs;

public abstract record SieveBaseDto
{
    public int LabTestId { get; init; }
    public double? TotalDryMassGrams { get; init; }
    public double? MoistureContentBefore { get; init; }
    public double? MoistureContentAfter { get; init; }
}

public record SieveReadDto : SieveBaseDto
{
    public int Id { get; init; }
    public LabTestReadDto LabTest { get; init; } = null!;
    public ICollection<SieveResultReadDto> Results { get; init; } = [];
    public ICollection<ProctorReadDto> Proctors { get; init; } = [];

    // Computed properties for the UI
    public double SumRetainedGrams { get; init; }
    public double MassClosureErrorGrams { get; init; }
    public double MassClosureErrorPercent { get; init; }
    
    // This allows the frontend to get the computed percentages immediately
    public IEnumerable<SieveRowDto> ComputedRows { get; init; } = [];
}

public record SieveCreateDto : SieveBaseDto;

public record SieveUpdateDto : SieveBaseDto
{
    public int Id { get; init; }
}