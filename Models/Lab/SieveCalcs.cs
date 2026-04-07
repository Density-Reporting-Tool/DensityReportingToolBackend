namespace DensityReportingToolBackend.Models;

public static class SieveCalcs
{
    public sealed record Row(
        SieveResult Result,
        double PercentRetained,       // this sieve
        double CumulativePercentRetained,
        double PercentPassing         // 100 - cumulative retained
    );

    public static IReadOnlyList<Row> ComputePercentages(this Sieve sieve)
    {
        var total = sieve.TotalDryMassGrams.GetValueOrDefault();
        if (total <= 0 || sieve.Results is null || sieve.Results.Count == 0)
            return Array.Empty<Row>();

        var ordered = sieve.Results.OrderBy(r => r.OrderIndex).ToList();

        double cumRetainedPct = 0.0;
        var rows = new List<Row>(ordered.Count);

        foreach (var r in ordered)
        {
            var pctRetained = (r.GramsRetained / total) * 100.0;
            cumRetainedPct += pctRetained;
            var pctPassing = 100.0 - cumRetainedPct;

            rows.Add(new Row(
                r,
                PercentRetained: pctRetained,
                CumulativePercentRetained: cumRetainedPct,
                PercentPassing: pctPassing
            ));
        }

        return rows;
    }
}
