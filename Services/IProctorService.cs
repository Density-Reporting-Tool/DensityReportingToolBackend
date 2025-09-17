using DensityReportingToolBackend.Models.DTOs;

namespace DensityReportingToolBackend.Services
{
    public interface IProctorService
    {
        /// <summary>
        /// Create a new proctor (Lab Admin functionality)
        /// </summary>
        Task<ProctorCreateResponse> CreateProctorAsync(CreateProctorRequest request);
        
        /// <summary>
        /// Update an existing proctor (Lab Admin functionality)
        /// </summary>
        Task<ProctorUpdateResponse?> UpdateProctorAsync(int id, UpdateProctorRequest request);
        
        /// <summary>
        /// Get a specific proctor by ID
        /// </summary>
        Task<ProctorDataResponse?> GetProctorAsync(int id);
        
        /// <summary>
        /// Get all proctors for a specific job
        /// </summary>
        Task<IEnumerable<ProctorDataResponse>> GetProctorsForJobAsync(string jobNumber);
        
        /// <summary>
        /// Get all proctors for lab admin management (paginated)
        /// </summary>
        Task<ProctorListResponse> GetProctorsForLabAdminAsync(int page, int limit, string? jobNumber = null);
        
        /// <summary>
        /// Get density requirements for field tech
        /// </summary>
        Task<DensityRequirementsResponse?> GetDensityRequirementsAsync(int id);
    }
}