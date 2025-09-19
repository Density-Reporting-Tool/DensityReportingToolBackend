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
                _logger.LogInformation("Creating report for job ID: {JobId}", request.JobId);

                // 1. Validate and find job
                var job = await FindJobAsync(request.JobId);
                
                // 2. Validate and find employee
                var employee = await FindEmployeeAsync(request.EmployeeId);
                
                // 3. Validate and find reviewer (if provided)
                PersonalInfo? reviewer = null;
                if (request.ReviewerId.HasValue)
                {
                    reviewer = await FindReviewerAsync(request.ReviewerId.Value);
                }
                
                // 4. Get next report number for this job
                var nextReportNumber = await GetNextReportNumberAsync(request.JobId);
                
                // 5. Create new report
                var newReport = await CreateReportEntityAsync(request, nextReportNumber);
                
                // 6. Create memo if provided
                if (request.Memo != null)
                {
                    await CreateReportMemoAsync(newReport.Id, request.Memo);
                }
                
                await transaction.CommitAsync();
                
                _logger.LogInformation("Successfully created report with ID: {ReportId} and number: {ReportNumber}", 
                    newReport.Id, newReport.ReportNumber);

                return MapToCreateResponse(newReport, job, employee, reviewer);
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

        #region Private Helper Methods

        private async Task<Job> FindJobAsync(int jobId)
        {
            var job = await _dbContext.Jobs.FindAsync(jobId);
            
            if (job == null)
            {
                _logger.LogWarning("Job with ID {JobId} does not exist", jobId);
                throw new ArgumentException($"Job with ID {jobId} not found");
            }
            
            _logger.LogInformation("Found existing job with ID: {JobId} and number: {JobNumber}", 
                job.Id, job.JobNumber);
            
            return job;
        }

        private async Task<PersonalInfo> FindEmployeeAsync(int employeeId)
        {
            var employee = await _dbContext.PersonalInfos
                .FirstOrDefaultAsync(e => e.Id == employeeId && e.Company == "GeoPacific");
                
            if (employee == null)
            {
                _logger.LogWarning("GeoPacific employee with ID {EmployeeId} not found", employeeId);
                throw new ArgumentException($"GeoPacific employee with ID {employeeId} not found");
            }
            
            _logger.LogInformation("Found GeoPacific employee: {EmployeeId} - {FirstName} {LastName}", 
                employee.Id, employee.FirstName, employee.LastName);
            
            return employee;
        }

        private async Task<PersonalInfo> FindReviewerAsync(int reviewerId)
        {
            var reviewer = await _dbContext.PersonalInfos
                .FirstOrDefaultAsync(e => e.Id == reviewerId && e.Company == "GeoPacific");
                
            if (reviewer == null)
            {
                _logger.LogWarning("GeoPacific reviewer with ID {ReviewerId} not found", reviewerId);
                throw new ArgumentException($"GeoPacific reviewer with ID {reviewerId} not found");
            }
            
            _logger.LogInformation("Found GeoPacific reviewer: {ReviewerId} - {FirstName} {LastName}", 
                reviewer.Id, reviewer.FirstName, reviewer.LastName);
            
            return reviewer;
        }

        private async Task<int> GetNextReportNumberAsync(int jobId)
        {
            var maxReportNumber = await _dbContext.Reports
                .Where(r => r.JobId == jobId)
                .MaxAsync(r => (int?)r.ReportNumber) ?? 0;
                
            var nextNumber = maxReportNumber + 1;
            _logger.LogInformation("Next report number for job {JobId}: {ReportNumber}", jobId, nextNumber);
            
            return nextNumber;
        }

        private async Task<Report> CreateReportEntityAsync(CreateReportRequest request, int reportNumber)
        {
            var newReport = new Report
            {
                JobId = request.JobId,
                EmployeeId = request.EmployeeId,
                ReviewerId = request.ReviewerId ?? 0, // Will be set when reviewer is assigned
                ReportNumber = reportNumber,
                StartDate = request.StartDate ?? DateTime.UtcNow,
                SubmitDate = request.SubmitDate,
                DistributeDate = request.DistributeDate,
                DistributionListId = request.DistributionListId
            };
            
            _dbContext.Reports.Add(newReport);
            await _dbContext.SaveChangesAsync();
            
            _logger.LogInformation("Created report with ID: {ReportId} and number: {ReportNumber}", 
                newReport.Id, newReport.ReportNumber);
            
            return newReport;
        }

        private async Task CreateReportMemoAsync(int reportId, ReportMemoRequest memoRequest)
        {
            var memo = new ReportMemo
            {
                ReportId = reportId,
                Purpose = memoRequest.Purpose,
                CommentsAndObservations = memoRequest.CommentsAndObservations,
                Conclusion = memoRequest.Conclusion,
                CreatedDate = DateTime.UtcNow
            };

            _dbContext.ReportMemos.Add(memo);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Successfully created memo for report ID: {ReportId}", reportId);
        }

        private static ReportCreateResponse MapToCreateResponse(
            Report report, 
            Job job, 
            PersonalInfo employee, 
            PersonalInfo? reviewer)
        {
            return new ReportCreateResponse
            {
                Id = report.Id.ToString(),
                Message = "Report created successfully",
                Report = new ReportDataResponse
                {
                    Id = report.Id,
                    JobId = report.JobId,
                    ReportNumber = report.ReportNumber,
                    StartDate = report.StartDate,
                    SubmitDate = report.SubmitDate,
                    DistributeDate = report.DistributeDate,
                    Employee = new EmployeeInfo
                    {
                        Id = employee.Id,
                        FirstName = employee.FirstName,
                        LastName = employee.LastName,
                        Email = employee.Email,
                        PhoneNumber = employee.PhoneNumber
                    },
                    Reviewer = reviewer != null ? new EmployeeInfo
                    {
                        Id = reviewer.Id,
                        FirstName = reviewer.FirstName,
                        LastName = reviewer.LastName,
                        Email = reviewer.Email,
                        PhoneNumber = reviewer.PhoneNumber
                    } : new EmployeeInfo(), // Empty reviewer info if none assigned
                    Job = new JobInfo
                    {
                        Id = job.Id,
                        JobNumber = job.JobNumber,
                        ClientName = job.ClientName,
                        ProjectName = job.ProjectName
                    },
                    DistributionListId = report.DistributionListId
                }
            };
        }

        #endregion
    }
}
