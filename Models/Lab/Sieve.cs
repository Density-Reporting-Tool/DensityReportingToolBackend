namespace DensityReportingToolBackend.Models;

public class Sieve
{
    public int Id { get; set; }

    public int LabTestId { get; set; }
    public LabTest LabTest { get; set; } = null!;

    // Total dry mass used in this gradation (g). Set once for the whole stack run.
    public double? TotalDryMassGrams { get; set; }

    // Optional moisture bookkeeping if you capture it
    public double? MoistureContentBefore { get; set; }  // %
    public double? MoistureContentAfter  { get; set; }  // %

    // Results ordered coarse→fine (include a Pan row)
    public ICollection<SieveResult> Results { get; set; } = [];

    // Convenience checks (not mapped)
    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public double SumRetainedGrams => Results?.Sum(r => r.GramsRetained) ?? 0.0;

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public double MassClosureErrorGrams => (TotalDryMassGrams ?? 0) - SumRetainedGrams;

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public double MassClosureErrorPercent =>
        (TotalDryMassGrams is > 0)
            ? (MassClosureErrorGrams / TotalDryMassGrams!.Value) * 100.0
            : 0.0;
}
