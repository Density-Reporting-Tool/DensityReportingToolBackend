namespace DensityReportingToolBackend.Models;

public enum ProbeDepthUnit { Cm = 0, Inch = 1 }
public enum CompactionSpecificationUnit { SPDD = 0, MPDD = 1 }
public enum ElevationUnit { Meters = 0, Feet = 1 }
public enum ElevationReference
{
    Unknown = 0,            // fallback if nothing specified
    AboveSubgrade,          // Above the prepared subgrade
    BelowSubgrade,          // Below the prepared subgrade
    AboveFinalGrade,        // Above the finished surface grade
    BelowFinalGrade,        // Below the finished surface grade
    Geodetic,               // Absolute elevation (sea level datum)
    DesignElevation,        // As per design plans/drawings
    ExistingGround,         // Natural ground before construction
    TopOfPavement,          // Finished asphalt/concrete surface
    TopOfBaseCourse,        // Top of aggregate base layer
    TopOfSubbase,           // Top of subbase material
    TopOfLift,              // Top of a compacted lift
    BottomOfLift            // Bottom of a compacted lift
}

public class DensityTest
{
    public int Id { get; set; }

    public int ProctorId { get; set; }
    public Proctor Proctor { get; set; } = null!;

    public int ReportId { get; set; }
    public Report Report { get; set; } = null!;

    public string? TestArea { get; set; }
    public string? Location { get; set; }

    public ElevationReference? ElevationReference { get; set; }
    public double? ElevationValue { get; set; }   // e.g +0.30 m above subgrade or geodetic m
    public ElevationUnit? ElevationUnit { get; set; }  // m or ft

    public float? CorrectedOversizePercentage { get; set; }
    public int? ProbeDepth { get; set; } //cm or inch
    public ProbeDepthUnit? ProbeDepthUnit { get; set; }

    public int? CompactionSpecification { get; set; } // e.g 95 %
    public CompactionSpecificationUnit? CompactionSpecificationUnit { get; set; } // MPDD or SPDD

    public int? DensityValue { get; set; }
    public int? MoistureValue { get; set; }

    public int? ShotPlacementId { get; set; }
    public ShotPlacement? ShotPlacement { get; set; }

    public DateTime? CreatedDate { get; set; }
    public DateTime? LastEditDate { get; set; }

    public ICollection<DensityTestComment> Comments { get; set; } = [];
}
