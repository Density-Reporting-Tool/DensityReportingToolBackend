using DensityReportingToolBackend.Models;
using DensityReportingToolBackend.Models.DTOs;

namespace DensityReportingToolBackend.Services
{
    public interface IReportService
    {
        /// <summary>
        /// Get all reports for dashboard/schedule view
        /// </summary>
        Task<IEnumerable<Report>> ListReports();
        
        /// <summary>
        /// Get report by ID
        /// </summary>
        Task<Report?> GetReportById(int reportId);
        
        /// <summary>
        /// Get report by job ID and report number
        /// </summary>
        Task<Report?> GetReportByNumber(int jobId, int reportNumber);
        
        /// <summary>
        /// Search reports by job number
        /// </summary>
        Task<IEnumerable<Report>> SearchReportsByJobNumber(string jobNumber, int limit = 10);
        
        /// <summary>
        /// Create a new report
        /// </summary>
        Task<Report> CreateReport(ReportCreateDto dto);
        
        /// <summary>
        /// Update an existing report
        /// </summary>
        Task<Report> UpdateReport(int reportId, ReportUpdateDto dto);

        // Legacy methods for backward compatibility
        /// <summary>
        /// Create a new report for a job (legacy)
        /// </summary>
        Task<ReportCreateResponse> CreateReportAsync(CreateReportRequest request);
        
        /// <summary>
        /// Get all reports for a specific job (newest first) (legacy)
        /// </summary>
        Task<IEnumerable<ReportListByJobResponse>> GetReportsByJobAsync(string jobNumber);
        
        /// <summary>
        /// Get a specific report by ID with full details (legacy)
        /// </summary>
        Task<ReportDetailResponse?> GetReportAsync(int reportId);
    }
}
