using DensityReportingToolBackend.Models.DTOs;

namespace DensityReportingToolBackend.Services
{
    public interface IReportService
    {
        /// <summary>
        /// Create a new report for a job
        /// </summary>
        Task<ReportCreateResponse> CreateReportAsync(CreateReportRequest request);
        
        /// <summary>
        /// Get all reports for a specific job (newest first)
        /// </summary>
        Task<IEnumerable<ReportListByJobResponse>> GetReportsByJobAsync(string jobNumber);
        
        /// <summary>
        /// Get a specific report by ID with full details
        /// </summary>
        Task<ReportDetailResponse?> GetReportAsync(int reportId);
    }
}
