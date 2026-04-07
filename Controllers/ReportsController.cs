using DensityReportingToolBackend.Data;
using DensityReportingToolBackend.Models;
using DensityReportingToolBackend.Models.DTOs;
using DensityReportingToolBackend.Services;
using Microsoft.AspNetCore.Mvc;

namespace DensityReportingToolBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly ILogger<ReportsController> _logger;
        private readonly AppDbContext _dbContext;
        private readonly ReportService _reportService;

        public ReportsController(ILogger<ReportsController> logger, AppDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
            _reportService = new ReportService(dbContext);
        }

        /// <summary>
        /// Get all reports for dashboard view
        /// </summary>
        /// <returns>List of all reports with basic information</returns>
        [HttpGet]
        public async Task<ActionResult<object>> GetAllReports()
        {
            try
            {
                _logger.LogInformation("Retrieving all reports for dashboard");
                var reports = await _reportService.GetAllReportsAsync();
                _logger.LogInformation("Successfully retrieved {ReportCount} reports", reports.Count());
                return Ok(reports);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all reports");
                return StatusCode(500, new
                {
                    message = "An error occurred while retrieving reports"
                });
            }
        }

        /// <summary>
        /// Get a specific report by ID
        /// </summary>
        /// <param name="reportId">The report ID</param>
        /// <returns>Report details</returns>
        [HttpGet("{reportId}")]
        public async Task<ActionResult<object>> GetReport(int reportId)
        {
            try
            {
                _logger.LogInformation("Retrieving report with ID: {ReportId}", reportId);

                var report = await _reportService.GetReportAsync(reportId);

                if (report == null)
                {
                    _logger.LogWarning("Report with ID {ReportId} not found", reportId);
                    return NotFound(new
                    {
                        message = $"Report with ID {reportId} not found",
                        reportId = reportId
                    });
                }

                _logger.LogInformation("Successfully retrieved report {ReportId}", reportId);
                return Ok(report);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving report {ReportId}", reportId);
                return StatusCode(500, new
                {
                    message = "An error occurred while retrieving the report"
                });
            }
        }

        /// <summary>
        /// Create a new report for a job
        /// </summary>
        /// <param name="reportRequest">Report creation request</param>
        /// <returns>Created report information</returns>
        [HttpPost]
        public async Task<ActionResult<object>> CreateReport([FromBody] CreateReportRequest reportRequest)
        {
            try
            {
                _logger.LogInformation("Creating new report for job ID: {JobId}", reportRequest.JobId);

                // Basic validation
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Use the service to create the report
                var result = await _reportService.CreateReportAsync(reportRequest);

                return CreatedAtAction(nameof(GetReport), new { reportId = int.Parse(result.Id) }, result);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error creating report for job {JobId}", reportRequest.JobId);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating report for job {JobId}", reportRequest.JobId);
                return StatusCode(500, new
                {
                    message = "An error occurred while creating the report"
                });
            }
        }

        /// <summary>
        /// Search reports by job number
        /// </summary>
        /// <param name="jobNumber">Job number to search for</param>
        /// <param name="limit">Maximum number of results to return</param>
        /// <returns>List of matching reports</returns>
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<ReportListByJobResponse>>> SearchReports([FromQuery] string jobNumber, [FromQuery] int limit = 10)
        {
            if (string.IsNullOrWhiteSpace(jobNumber))
                return BadRequest(new { message = "jobNumber query parameter is required" });

            var reports = await _reportService.SearchReportsByJobNumberAsync(jobNumber, limit);

            if (!reports.Any())
                return NotFound(new { message = "No reports found" });

            return Ok(reports);
        }

        /// <summary>
        /// Get all reports for a specific job
        /// </summary>
        /// <param name="jobNumber">The job number to get reports for</param>
        /// <returns>List of reports for the job</returns>
        [HttpGet("job/{jobNumber}")]
        public async Task<ActionResult<object>> GetReportsByJob(string jobNumber)
        {
            try
            {
                _logger.LogInformation("Retrieving reports for job number: {JobNumber}", jobNumber);

                var reports = await _reportService.GetReportsByJobAsync(jobNumber);

                _logger.LogInformation("Successfully retrieved {ReportCount} reports for job {JobNumber}", reports.Count(), jobNumber);
                return Ok(reports);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving reports for job {JobNumber}", jobNumber);
                return StatusCode(500, new
                {
                    message = "An error occurred while retrieving reports"
                });
            }
        }

        /// <summary>
        /// Get proctors available for a specific job
        /// </summary>
        /// <param name="jobId">The job ID</param>
        /// <returns>List of proctors for the job</returns>
        [HttpGet("proctors/job/{jobId}")]
        public async Task<ActionResult<object>> GetProctorsForJob(int jobId)
        {
            try
            {
                _logger.LogInformation("Getting proctors for job {JobId}", jobId);

                var proctors = await _reportService.GetProctorsForJobAsync(jobId);

                _logger.LogInformation("Found {Count} proctors for job {JobId}", proctors.Count(), jobId);
                return Ok(proctors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting proctors for job {JobId}", jobId);
                return StatusCode(500, new { message = "An error occurred while getting proctors" });
            }
        }

        /// <summary>
        /// Create a new density test for a report
        /// </summary>
        /// <param name="reportId">The report ID</param>
        /// <param name="request">Density test creation request</param>
        /// <returns>Created density test information</returns>
        [HttpPost("{reportId}/density-test")]
        public async Task<ActionResult<object>> CreateDensityTest(int reportId, [FromBody] CreateDensityTestRequest request)
        {
            try
            {
                _logger.LogInformation("Creating density test for report {ReportId}", reportId);

                var result = await _reportService.CreateDensityTestAsync(reportId, request);

                _logger.LogInformation("Density test created with ID {DensityTestId} for report {ReportId}", result.Id, reportId);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error creating density test for report {ReportId}", reportId);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating density test for report {ReportId}", reportId);
                return StatusCode(500, new
                {
                    message = "An error occurred while creating the density test"
                });
            }
        }
    }
}
