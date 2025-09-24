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

public class DensityTest: ModelBaseWithDto<DensityTest, DensityTestReadDto>
{
    public int ProctorId { get; set; }
    public Proctor Proctor { get; set; } = null!;

    public int ReportId { get; set; }
    public Report Report { get; set; } = null!;

    public string? TestArea { get; set; }
    public string? Location { get; set; }

    public ElevationReference? ElevationReference { get; set; }
    public double? ElevationValue { get; set; }   // e.g +0.30 above subgrade or geodetic m
    public ElevationUnit? ElevationUnit { get; set; }  // m or ft

    public float? CorrectedOversizePercentage { get; set; }
    public int? ProbeDepth { get; set; } //cm or inch
    public ProbeDepthUnit? ProbeDepthUnit { get; set; }

    [System.ComponentModel.DataAnnotations.Range(0, 110, ErrorMessage = "Compaction specification must be between 0 and 110%")]
    public double? CompactionSpecification { get; set; } // e.g 95 %
    public CompactionSpecificationUnit? CompactionSpecificationUnit { get; set; } // MPDD or SPDD

    public double? DensityValue { get; set; }
    [System.ComponentModel.DataAnnotations.Range(0, 100, ErrorMessage = "Moisture value must be between 0 and 100%")]
    public double? MoistureValue { get; set; }

    public ShotPlacement? ShotPlacement { get; set; }

    public DateTime? CreatedDate { get; set; }
    public DateTime? LastEditDate { get; set; }

    public ICollection<DensityTestComment> Comments { get; set; } = [];
}

public class DensityTestBaseDto
{
    public int ProctorId { get; set; }
    public int ReportId { get; set; }
    public string? TestArea { get; set; }
    public string? Location { get; set; }
    public ElevationReference? ElevationReference { get; set; }
    public double? ElevationValue { get; set; }
    public ElevationUnit? ElevationUnit { get; set; }
    public float? CorrectedOversizePercentage { get; set; }
    public int? ProbeDepth { get; set; }
    public ProbeDepthUnit? ProbeDepthUnit { get; set; }
    public double? CompactionSpecification { get; set; }
    public CompactionSpecificationUnit? CompactionSpecificationUnit { get; set; }
    public double? DensityValue { get; set; }
    public double? MoistureValue { get; set; }
}

public class DensityTestCreateDto : DensityTestBaseDto { }

public class DensityTestUpdateDto : DensityTestBaseDto
{
    public int Id { get; set; }
}

public class DensityTestReadDto : DensityTestBaseDto
{
    public int Id { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? LastEditDate { get; set; }

    public DensityTestReadDto(DensityTest densityTest, HashSet<(Type, int)> visited)
    {
        Id = densityTest.Id;
        ProctorId = densityTest.ProctorId;
        ReportId = densityTest.ReportId;
        TestArea = densityTest.TestArea;
        Location = densityTest.Location;
        ElevationReference = densityTest.ElevationReference;
        ElevationValue = densityTest.ElevationValue;
        ElevationUnit = densityTest.ElevationUnit;
        CorrectedOversizePercentage = densityTest.CorrectedOversizePercentage;
        ProbeDepth = densityTest.ProbeDepth;
        ProbeDepthUnit = densityTest.ProbeDepthUnit;
        CompactionSpecification = densityTest.CompactionSpecification;
        CompactionSpecificationUnit = densityTest.CompactionSpecificationUnit;
        DensityValue = densityTest.DensityValue;
        MoistureValue = densityTest.MoistureValue;
        CreatedDate = densityTest.CreatedDate;
        LastEditDate = densityTest.LastEditDate;
    }
}
