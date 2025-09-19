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

    #endregion
}
