namespace DensityReportingToolBackend.DTOs.Labs;

using DensityReportingToolBackend.DTOs.Density;

public abstract record ProctorBaseDto
{
    public string? ProctorID { get; init; }
    public string? ProctorTestNumber { get; init; }
    public int? LabTestId { get; init; }
    public int? SieveId { get; init; }
    public int ProctorTypeId { get; init; }
    public string? MaterialType { get; init; }
    public string? LabLocation { get; init; }
    public DateTime? DateSampled { get; init; }
    public DateTime? DateTested { get; init; }
    public double? MaxDensity { get; init; }
    public double? CorrectedDensity { get; init; }
    public double? OptimumMoistureContent { get; init; }
    public double? SpecificGravity { get; init; }
    public double? OversizePercentage { get; init; }
}

public record ProctorReadDto : ProctorBaseDto
{
    public int Id { get; init; }
    public ProctorTypeReadDto ProctorType { get; init; } = null!;
    public LabTestReadDto LabTest { get; init; } =null!;
    public SieveReadDto? Sieve { get; init; }
    public ICollection<ProctorAdditionalJobReadDto> AdditionalJobs { get; init; } = [];
    public ICollection<DensityTestReadDto> DensityTests { get; init; } = [];
}

public record ProctorSummaryDto
{
    public int Id { get; init; }
    public string? ProctorID { get; init; }
    public string? ProctorTestNumber { get; init; }
    public string? ProctorType { get; init; }      
    public string? MaterialType { get; init; }
    public string? LabLocation { get; init; }
    public DateTime? DateTested { get; init; }
    public double? MaxDensity { get; init; }
    public double? CorrectedDensity { get; init; }
    public double? OptimumMoistureContent { get; init; }
    public string? JobNumber { get; init; }         
    public bool IsReused { get; init; }            //true if borrowed via ProctorAdditionalJob
}

public record ProctorCreateDto : ProctorBaseDto
{
    public int? JobId { get; init; }
    public string? JobNumber { get; init; }
}

public record ProctorUpdateDto : ProctorBaseDto
{
    public int Id { get; init; }
}