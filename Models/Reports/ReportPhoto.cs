namespace DensityReportingToolBackend.Models;

public class ReportPhoto
{
    public int Id { get; set; }

    public int ReportId { get; set; }
    public Report Report { get; set; } = null!;

    public string? Code { get; set; }
    //url nullable incase that record is stored asynchronously
    public string? Url { get; set; }
    public string? Description { get; set; }

    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public double? GpsAccuracyMeters { get; set; }
}