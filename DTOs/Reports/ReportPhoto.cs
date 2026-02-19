namespace DensityReportingToolBackend.DTOs.Reports;

public abstract record ReportPhotoBaseDto
{
    public int ReportId { get; init; }
    public string? Code { get; init; }
    public string? Url { get; init; }
    public string? Description { get; init; }
    public double? Latitude { get; init; }
    public double? Longitude { get; init; }
    public double? GpsAccuracyMeters { get; init; }
}

public record ReportPhotoReadDto : ReportPhotoBaseDto
{
    public int Id { get; init; }
    public ReportReadDto Report { get; init; } = null!;
}

public record ReportPhotoCreateDto : ReportPhotoBaseDto;

public record ReportPhotoUpdateDto : ReportPhotoBaseDto
{
    public int Id { get; init; }
}