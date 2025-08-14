namespace DensityReportingToolBackend.Models;

public class Proctor
{
    public int Id { get; set; }

    public string? ProctorID { get; set; }

    public int LabTestId { get; set; }
    public LabTest LabTest { get; set; } = null!;

    public int? SieveId { get; set; }
    public Sieve? Sieve { get; set; }

    public int ProctorTypeId { get; set; }
    public ProctorType ProctorType { get; set; } = null!;

    public double? MaxDensity { get; set; }
    public double? CorrectedDensity { get; set; }
    public double? OptimumMoistureContent { get; set; }
    public double? SpecificGravity { get; set; }

    public ICollection<ProctorAdditionalJob> AdditionalJobs { get; set; } = [];
    public ICollection<DensityTest> DensityTests { get; set; } = [];
}
