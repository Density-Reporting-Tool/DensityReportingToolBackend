namespace DensityReportingToolBackend.Models;

public class ReportPhoto: ModelBaseWithDto<ReportPhoto, ReportPhotoReadDto>
{
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

public class ReportPhotoBaseDto
{
    public int ReportId { get; set; }
    public string? Code { get; set; }
    public string? Url { get; set; }
    public string? Description { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public double? GpsAccuracyMeters { get; set; }
}

public class ReportPhotoCreateDto : ReportPhotoBaseDto { }

public class ReportPhotoUpdateDto : ReportPhotoBaseDto
{
    public int Id { get; set; }
}

public class ReportPhotoReadDto : ReportPhotoBaseDto
{
    public int Id { get; set; }

    public ReportPhotoReadDto(ReportPhoto photo, HashSet<(Type, int)> visited)
    {
        Id = photo.Id;
        ReportId = photo.ReportId;
        Code = photo.Code;
        Url = photo.Url;
        Description = photo.Description;
        Latitude = photo.Latitude;
        Longitude = photo.Longitude;
        GpsAccuracyMeters = photo.GpsAccuracyMeters;
    }
}