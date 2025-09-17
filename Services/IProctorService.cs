itusing DensityReportingToolBackend.Models.DTOs;

namespace DensityReportingToolBackend.Services
{
    public interface IProctorService
    {
        /// <summary>
        /// Create a new proctor (Lab Admin functionality)
        /// </summary>
        Task<ProctorCreateResponse> CreateProctorAsync(CreateProctorRequest request);
        
        // TODO: Add other methods as needed
        // Task<ProctorUpdateResponse?> UpdateProctorAsync(int id, UpdateProctorRequest request);
        // Task<ProctorDataResponse?> GetProctorAsync(int id);
        // Task<IEnumerable<ProctorDataResponse>> GetProctorsForJobAsync(string jobNumber);
    }
}