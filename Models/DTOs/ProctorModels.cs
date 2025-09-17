using System.ComponentModel.DataAnnotations;

namespace DensityReportingToolBackend.Models.DTOs
{
    #region Request Models

    /// <summary>
    /// Request model for creating a new proctor (Lab Admin)
    /// Maps directly to frontend ProctorData interface
    /// </summary>
    public class CreateProctorRequest
    {
        [Required(ErrorMessage = "Job number is required")]
        [StringLength(50, ErrorMessage = "Job number cannot exceed 50 characters")]
        public required string JobNumber { get; set; }

        [Required(ErrorMessage = "Proctor test number is required")]
        [StringLength(50, ErrorMessage = "Proctor test number cannot exceed 50 characters")]
        public required string ProctorTestNumber { get; set; }

        [Required(ErrorMessage = "Material type is required")]
        [StringLength(100, ErrorMessage = "Material type cannot exceed 100 characters")]
        public required string MaterialType { get; set; }

        [Required(ErrorMessage = "Date sampled is required")]
        public required string DateSampled { get; set; }

        [Required(ErrorMessage = "Proctor type is required")]
        [RegularExpression("^(MPDD|Standard|Modified)$", ErrorMessage = "Proctor type must be MPDD, Standard, or Modified")]
        public required string ProctorType { get; set; }

        [Required(ErrorMessage = "Max dry density is required")]
        public required string MaxDryDensity { get; set; }

        [Required(ErrorMessage = "Corrected density is required")]
        public required string CorrectedDensity { get; set; }

        [Required(ErrorMessage = "Lab location is required")]
        [StringLength(100, ErrorMessage = "Lab location cannot exceed 100 characters")]
        public required string LabLocation { get; set; }

        [Required(ErrorMessage = "Proctor ID is required")]
        [StringLength(50, ErrorMessage = "Proctor ID cannot exceed 50 characters")]
        public required string ProctorId { get; set; }

        [Required(ErrorMessage = "Date tested is required")]
        public required string DateTested { get; set; }

        [Range(0, 100, ErrorMessage = "Oversize percentage must be between 0 and 100")]
        public required double OversizePercentage { get; set; }

        [Range(0, 100, ErrorMessage = "Optimum moisture must be between 0 and 100")]
        public required double OptimumMoisture { get; set; }

        [StringLength(20, ErrorMessage = "Specific gravity cannot exceed 20 characters")]
        public string? SpecificGravity { get; set; }
    }

    /// <summary>
    /// Request model for updating an existing proctor (Lab Admin)
    /// </summary>
    public class UpdateProctorRequest
    {
        [Required(ErrorMessage = "Proctor test number is required")]
        [StringLength(50, ErrorMessage = "Proctor test number cannot exceed 50 characters")]
        public required string ProctorTestNumber { get; set; }

        [Required(ErrorMessage = "Material type is required")]
        [StringLength(100, ErrorMessage = "Material type cannot exceed 100 characters")]
        public required string MaterialType { get; set; }

        [Required(ErrorMessage = "Date sampled is required")]
        public required string DateSampled { get; set; }

        [Required(ErrorMessage = "Proctor type is required")]
        [RegularExpression("^(MPDD|Standard|Modified)$", ErrorMessage = "Proctor type must be MPDD, Standard, or Modified")]
        public required string ProctorType { get; set; }

        [Required(ErrorMessage = "Max dry density is required")]
        public required string MaxDryDensity { get; set; }

        [Required(ErrorMessage = "Corrected density is required")]
        public required string CorrectedDensity { get; set; }

        [Required(ErrorMessage = "Lab location is required")]
        [StringLength(100, ErrorMessage = "Lab location cannot exceed 100 characters")]
        public required string LabLocation { get; set; }

        [Required(ErrorMessage = "Date tested is required")]
        public required string DateTested { get; set; }

        [Range(0, 100, ErrorMessage = "Oversize percentage must be between 0 and 100")]
        public required double OversizePercentage { get; set; }

        [Range(0, 100, ErrorMessage = "Optimum moisture must be between 0 and 100")]
        public required double OptimumMoisture { get; set; }

        [StringLength(20, ErrorMessage = "Specific gravity cannot exceed 20 characters")]
        public string? SpecificGravity { get; set; }
    }

    #endregion

    #region Response Models

    /// <summary>
    /// Response model for proctor creation
    /// Maps to frontend ProctorCreateResponse interface
    /// </summary>
    public class ProctorCreateResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public ProctorDataResponse Proctor { get; set; } = new();
    }

    /// <summary>
    /// Response model for proctor updates
    /// </summary>
    public class ProctorUpdateResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public ProctorDataResponse Proctor { get; set; } = new();
    }

    /// <summary>
    /// Standard proctor data response
    /// Maps to frontend ProctorData interface
    /// </summary>
    public class ProctorDataResponse
    {
        public string JobNumber { get; set; } = string.Empty;
        public string ProctorTestNumber { get; set; } = string.Empty;
        public string MaterialType { get; set; } = string.Empty;
        public string DateSampled { get; set; } = string.Empty;
        public string ProctorType { get; set; } = string.Empty;
        public string MaxDryDensity { get; set; } = string.Empty;
        public string CorrectedDensity { get; set; } = string.Empty;
        public string LabLocation { get; set; } = string.Empty;
        public string ProctorId { get; set; } = string.Empty;
        public string DateTested { get; set; } = string.Empty;
        public double OversizePercentage { get; set; }
        public double OptimumMoisture { get; set; }
        public string SpecificGravity { get; set; } = string.Empty;
    }

    /// <summary>
    /// Paginated list response for lab admin proctor management
    /// Maps to frontend ProctorListResponse interface
    /// </summary>
    public class ProctorListResponse
    {
        public IEnumerable<ProctorDataResponse> Proctors { get; set; } = new List<ProctorDataResponse>();
        public int Total { get; set; }
        public int Page { get; set; }
        public int Limit { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)Total / Limit);
        public bool HasNextPage => Page < TotalPages;
        public bool HasPreviousPage => Page > 1;
    }

    /// <summary>
    /// Density requirements response for field techs
    /// Contains only the data needed for density testing
    /// </summary>
    public class DensityRequirementsResponse
    {
        public int ProctorId { get; set; }
        public string ProctorTestNumber { get; set; } = string.Empty;
        public double MaxDryDensity { get; set; }
        public double CorrectedDensity { get; set; }
        public double OptimumMoisture { get; set; }
        public string CompactionRequirement { get; set; } = "95% of maximum dry density";
        
        // Pre-calculated target densities for field tech convenience
        public double TargetDensity95 { get; set; }  // 95% of MaxDryDensity
        public double TargetDensity90 { get; set; }  // 90% of MaxDryDensity
        public double TargetDensity98 { get; set; }  // 98% of MaxDryDensity (for critical areas)
        
        // Additional context for field techs
        public string MaterialType { get; set; } = string.Empty;
        public string TestMethod { get; set; } = string.Empty;
        public string ProctorType { get; set; } = string.Empty;
        public double? SpecificGravity { get; set; }
        public double OversizePercentage { get; set; }
        
        // Moisture guidance
        public string MoistureGuidance { get; set; } = string.Empty;
    }

    /// <summary>
    /// Field tech specific proctor response
    /// Simplified view for field dashboard
    /// </summary>
    public class FieldTechProctorResponse
    {
        public int Id { get; set; }
        public string ProctorId { get; set; } = string.Empty;
        public string MaterialType { get; set; } = string.Empty;
        public string ProctorType { get; set; } = string.Empty;
        public double MaxDryDensity { get; set; }
        public double OptimumMoisture { get; set; }
        public double TargetDensity95 { get; set; }
        public string TestMethod { get; set; } = string.Empty;
        public DateTime? DateTested { get; set; }
        public bool IsActive { get; set; } = true;
    }

    #endregion

    #region Validation Models

    /// <summary>
    /// Validation result model
    /// </summary>
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public IList<string> Errors { get; set; } = new List<string>();
        public IList<string> Warnings { get; set; } = new List<string>();
    }

    /// <summary>
    /// Proctor validation response
    /// </summary>
    public class ProctorValidationResponse
    {
        public bool IsValid { get; set; }
        public IList<ValidationError> Errors { get; set; } = new List<ValidationError>();
        public IList<ValidationWarning> Warnings { get; set; } = new List<ValidationWarning>();
    }

    public class ValidationError
    {
        public string Field { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
    }

    public class ValidationWarning
    {
        public string Field { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
    }

    #endregion

    #region Query Models

    /// <summary>
    /// Query parameters for proctor searches
    /// </summary>
    public class ProctorQueryParameters
    {
        public string? JobNumber { get; set; }
        public string? MaterialType { get; set; }
        public string? ProctorType { get; set; }
        public DateTime? DateTestedFrom { get; set; }
        public DateTime? DateTestedTo { get; set; }
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 50;
        public string? SortBy { get; set; } = "DateTested";
        public string? SortDirection { get; set; } = "desc";
    }

    #endregion
}
