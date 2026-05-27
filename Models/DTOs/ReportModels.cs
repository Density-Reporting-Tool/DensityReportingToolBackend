using System.ComponentModel.DataAnnotations;

namespace DensityReportingToolBackend.Models.DTOs
{
    #region Request Models

    /// <summary>
    /// Request model for creating a new report
    /// </summary>
    public class CreateReportRequest
    {
        [Required(ErrorMessage = "Job ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Valid Job ID is required")]
        public required int JobId { get; set; }

        [Required(ErrorMessage = "Employee ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Valid Employee ID is required")]
        public required int EmployeeId { get; set; }

        public int? ReviewerId { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? SubmitDate { get; set; }
        public DateTime? DistributeDate { get; set; }
        public int? DistributionListId { get; set; }
        public ReportMemoRequest? Memo { get; set; }
    }

    /// <summary>
    /// Request model for report memo
    /// </summary>
    public class ReportMemoRequest
    {
        [StringLength(2000, ErrorMessage = "Purpose cannot exceed 2000 characters")]
        public string? Purpose { get; set; }

        [StringLength(2000, ErrorMessage = "Comments and observations cannot exceed 2000 characters")]
        public string? CommentsAndObservations { get; set; }

        [StringLength(2000, ErrorMessage = "Conclusion cannot exceed 2000 characters")]
        public string? Conclusion { get; set; }
    }

    /// <summary>
    /// Request model for updating (or creating) a report memo via PATCH /api/reports/{reportId}/memo.
    /// Upserts the most recent memo for the report.
    /// </summary>
    public class UpdateMemoRequest
    {
        [StringLength(2000, ErrorMessage = "Purpose cannot exceed 2000 characters")]
        public string? Purpose { get; set; }

        [StringLength(2000, ErrorMessage = "Comments and observations cannot exceed 2000 characters")]
        public string? CommentsAndObservations { get; set; }

        [StringLength(2000, ErrorMessage = "Conclusion cannot exceed 2000 characters")]
        public string? Conclusion { get; set; }
    }

    /// <summary>
    /// Request model for creating a density test
    /// </summary>
    public class CreateDensityTestRequest
    {
        [Required(ErrorMessage = "Proctor ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Valid Proctor ID is required")]
        public required int ProctorId { get; set; }

        [StringLength(200, ErrorMessage = "Test area cannot exceed 200 characters")]
        public string? TestArea { get; set; }

        [StringLength(200, ErrorMessage = "Location cannot exceed 200 characters")]
        public string? Location { get; set; }

        public string ElevationReference { get; set; } = "AboveSubgrade";
        public double ElevationValue { get; set; }
        public string ElevationUnit { get; set; } = "Meters";
        public double CorrectedOversizePercentage { get; set; }
        public double ProbeDepth { get; set; }
        public string ProbeDepthUnit { get; set; } = "Cm";
        public double CompactionSpecification { get; set; }
        public string CompactionSpecificationUnit { get; set; } = "SPDD";
        public double DensityValue { get; set; }
        public double MoistureValue { get; set; }
    }

    #endregion

    #region Response Models

    /// <summary>
    /// Response model for report creation
    /// </summary>
    public class ReportCreateResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public ReportDataResponse Report { get; set; } = new();
    }

    /// <summary>
    /// Response model for individual report in job list
    /// </summary>
    public class ReportListByJobResponse
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public JobInfo Job { get; set; } = new();
        public int ReportNumber { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? SubmitDate { get; set; }
        public DateTime? DistributeDate { get; set; }

        public EmployeeInfo Employee { get; set; } = new();
        public EmployeeInfo Reviewer { get; set; } = new();

        public int DensityTestsCount { get; set; }
        public int PhotosCount { get; set; }
        public int MemosCount { get; set; }
        public int? DistributionListId { get; set; }
    }

    /// <summary>
    /// Detailed report data response
    /// </summary>
    public class ReportDataResponse
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public int ReportNumber { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? SubmitDate { get; set; }
        public DateTime? DistributeDate { get; set; }
        
        public EmployeeInfo Employee { get; set; } = new();
        public EmployeeInfo Reviewer { get; set; } = new();
        public JobInfo Job { get; set; } = new();
        
        public int? DistributionListId { get; set; }
    }

    /// <summary>
    /// Detailed report response with all related data
    /// </summary>
    public class ReportDetailResponse
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public JobInfo Job { get; set; } = new();
        public int ReportNumber { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? SubmitDate { get; set; }
        public DateTime? DistributeDate { get; set; }
        
        public EmployeeInfo Employee { get; set; } = new();
        public EmployeeInfo? Reviewer { get; set; }
        
        public IEnumerable<DensityTestInfo> DensityTests { get; set; } = new List<DensityTestInfo>();
        public IEnumerable<PhotoInfo> Photos { get; set; } = new List<PhotoInfo>();
        public IEnumerable<MemoInfo> Memos { get; set; } = new List<MemoInfo>();
        
        public int? DistributionListId { get; set; }
    }

    /// <summary>
    /// Density test information for reports
    /// </summary>
    public class DensityTestInfo
    {
        public int Id { get; set; }
        public int TestNumber { get; set; }
        public string? TestArea { get; set; }
        public string? Location { get; set; }
        public string? ElevationReference { get; set; }
        public double ElevationValue { get; set; }
        public string? ElevationUnit { get; set; }
        public double CompactionSpecification { get; set; }
        public string? CompactionSpecificationUnit { get; set; }
        public double DensityValue { get; set; }
        public double MoistureValue { get; set; }
        public DateTime CreatedDate { get; set; }
        public double CompactionPercentage { get; set; }
        public bool Passed { get; set; }
    }

    /// <summary>
    /// Photo information for reports
    /// </summary>
    public class PhotoInfo
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public string? Url { get; set; }
        public string? Description { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public double? GpsAccuracyMeters { get; set; }
    }

    /// <summary>
    /// Memo information for reports
    /// </summary>
    public class MemoInfo
    {
        public int Id { get; set; }
        public string? Purpose { get; set; }
        public string? CommentsAndObservations { get; set; }
        public string? Conclusion { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }

    /// <summary>
    /// Employee information for reports
    /// </summary>
    public class EmployeeInfo
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
    }

    /// <summary>
    /// Job information for reports
    /// </summary>
    public class JobInfo
    {
        public int Id { get; set; }
        public string JobNumber { get; set; } = string.Empty;
        public string ClientName { get; set; } = string.Empty;
        public string ProjectName { get; set; } = string.Empty;
    }

    /// <summary>
    /// Proctor information for reports and field testing
    /// </summary>
    public class ReportProctorDataResponse
    {
        public int Id { get; set; }
        public string ProctorID { get; set; } = string.Empty;
        public double? MaxDensity { get; set; }
        public double? CorrectedDensity { get; set; }
        public double? OptimumMoistureContent { get; set; }
        public double? SpecificGravity { get; set; }
        public string? ProctorType { get; set; }
        public string? MaterialType { get; set; }
    }

    /// <summary>
    /// Density test creation response
    /// </summary>
    public class DensityTestCreateResponse
    {
        public int Id { get; set; }
        public string Message { get; set; } = "Density test created successfully";
    }

    #endregion
}
