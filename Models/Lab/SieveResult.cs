namespace DensityReportingToolBackend.Models;

public enum SieveSize
{
    // Coarse
    Sieve100mm,    // 4 in
    Sieve75mm,     // 3 in
    Sieve63mm,     // 2 1/2 in
    Sieve50mm,     // 2 in
    Sieve37_5mm,   // 1 1/2 in
    Sieve25mm,     // 1 in
    Sieve19mm,     // 3/4 in
    Sieve12_5mm,   // 1/2 in
    Sieve9_5mm,    // 3/8 in

    // Fine
    Sieve4_75mm,   // No. 4
    Sieve2_36mm,   // No. 8
    Sieve1_18mm,   // No. 16
    Sieve600um,    // No. 30
    Sieve300um,    // No. 50
    Sieve150um,    // No. 100
    Sieve75um,     // No. 200

    // Pan (fines passing last sieve)
    Pan
}

public class SieveResult
{
    public int Id { get; set; }

    public int SieveId { get; set; }
    public Sieve Sieve { get; set; } = null!;

    public SieveSize SieveSize { get; set; }

    public double GramsRetained { get; set; }
    public int OrderIndex { get; set; }
}
