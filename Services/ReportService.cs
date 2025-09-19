using DensityReportingToolBackend.Data;
using DensityReportingToolBackend.Models;
using DensityReportingToolBackend.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace DensityReportingToolBackend.Services
{
    public class ReportService : IReportService
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<ReportService> _logger;

        public ReportService(AppDbContext dbContext, ILogger<ReportService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<ReportCreateResponse> CreateReportAsync(CreateReportRequest request)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            
            try
            {
                _logger.LogInformation("Creating report for job: {JobId}", request.JobId);

                // TODO: Implement create report logic
                throw new NotImplementedException("CreateReportAsync not yet implemented");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Failed to create report for job: {JobId}", request.JobId);
                throw;
            }
        }

        public async Task<IEnumerable<ReportListByJobResponse>> GetReportsByJobAsync(string jobNumber)
        {
            try
            {
                _logger.LogInformation("Getting reports for job: {JobNumber}", jobNumber);

                // TODO: Implement get reports by job logic
                throw new NotImplementedException("GetReportsByJobAsync not yet implemented");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting reports for job: {JobNumber}", jobNumber);
                throw;
            }
        }
    }
}
