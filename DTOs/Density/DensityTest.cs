using DensityReportingToolBackend.DTOs.Labs;
using DensityReportingToolBackend.DTOs.Reports;
using DensityReportingToolBackend.Models;

namespace DensityReportingToolBackend.DTOs.Density;

public abstract record DensityTestBaseDto
{
    public int ProctorId { get; init; }
    public int ReportId { get; init; }
    public string? TestArea { get; init; }
    public string? Location { get; init; }
    public ElevationReference? ElevationReference { get; init; }
    public double? ElevationValue { get; init; }
    public ElevationUnit? ElevationUnit { get; init; }
    public float? CorrectedOversizePercentage { get; init; }
    public int? ProbeDepth { get; init; }
    public ProbeDepthUnit? ProbeDepthUnit { get; init; }
    public double? CompactionSpecification { get; init; }
    public CompactionSpecificationUnit? CompactionSpecificationUnit { get; init; }
    public double? DensityValue { get; init; }
    public double? MoistureValue { get; init; }
    public DateTime? CreatedDate { get; init; }
    public DateTime? LastEditDate { get; init; }
}

public record DensityTestReadDto : DensityTestBaseDto
{
    public int Id { get; init; }
    public ProctorSummaryDto Proctor { get; init; } = null!;
    public ReportReadDto Report { get; init; } = null!;
    public ShotPlacementReadDto? ShotPlacement { get; init; }
    public ICollection<DensityTestCommentReadDto> Comments { get; init; } = [];
}

public record DensityTestCreateDto : DensityTestBaseDto;

public record DensityTestUpdateDto : DensityTestBaseDto
{
    public int Id { get; init; }
}