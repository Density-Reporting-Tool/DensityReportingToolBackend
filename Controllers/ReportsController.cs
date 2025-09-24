using DensityReportingToolBackend.Data;
using DensityReportingToolBackend.Models;
using DensityReportingToolBackend.Models.DTOs;
using DensityReportingToolBackend.Services;
using DensityReportingToolBackend.Validators;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DensityReportingToolBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly ILogger<ReportsController> _logger;
        private readonly AppDbContext _dbContext;
        private readonly IReportService _reportService;

        public ReportsController(ILogger<ReportsController> logger, AppDbContext dbContext, IReportService reportService)
        {
            _logger = logger;
            _dbContext = dbContext;
            _reportService = reportService;
        }

        /// <summary>
        /// Get all reports for dashboard/schedule view
        /// </summary>
        /// <returns>List of all reports with basic information</returns>
        [HttpGet]
        public async Task<ActionResult<object>> GetAllReports()
        {
            try
            {
                _logger.LogInformation("Retrieving all reports for dashboard");
                var reports = await _reportService.ListReports();
                var result = reports.Select(report => report.ToDto()).ToList();
                _logger.LogInformation("Successfully retrieved {ReportCount} reports", result.Count);
                return Ok(result);
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
        /// Get report information by report ID
        /// </summary>
        /// <param name="reportId">The report ID to search for</param>
        /// <returns>Report information including job, employee details, and key relationships</returns>
        [HttpGet("{reportId}")]
        public async Task<ActionResult<object>> GetReport(int reportId)
        {
            try
            {
                _logger.LogInformation("Retrieving report with ID: {ReportId}", reportId);

                var report = await _reportService.GetReportById(reportId);

                if (report == null)
                {
                    _logger.LogWarning("Report with ID {ReportId} not found", reportId);
                    return NotFound(new
                    {
                        message = $"Report with ID {reportId} not found",
                        reportId = reportId
                    });
                }

                var result = report.ToDto();
                _logger.LogInformation("Successfully retrieved report {ReportId}", reportId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving report {ReportId}", reportId);
                return StatusCode(500, new
                {
                    message = "An error occurred while retrieving the report",
                    reportId = reportId
                });
            }
        }

        /// <summary>
        /// Create a new report
        /// </summary>
        /// <param name="reportDto">Report creation request with all report details</param>
        /// <returns>Created report information</returns>
        [HttpPost]
        public async Task<ActionResult<object>> CreateReport([FromBody] ReportCreateDto reportDto)
        {
            try
            {
                _logger.LogInformation("Creating new report for job ID: {JobId}", reportDto.JobId);

                // Validate required fields
                var validation = ReportValidator.Validate(reportDto);

                if (!validation.IsValid)
                {
                    return BadRequest(new
                    {
                        errors = validation.Errors
                    });
                }

                // Create new report
                var newReport = await _reportService.CreateReport(reportDto);

                _logger.LogInformation("Successfully created report with ID: {ReportId} and number: {ReportNumber}",
                    newReport.Id, newReport.ReportNumber);

                return CreatedAtAction(nameof(GetReport), new { reportId = newReport.Id }, newReport.ToDto());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating report for job ID: {JobId}", reportDto.JobId);
                return StatusCode(500, new
                {
                    message = "An error occurred while creating the report"
                });
            }
        }

        /// <summary>
        /// Update an existing report
        /// </summary>
        /// <param name="reportId">The report ID to update</param>
        /// <param name="reportDto">Report update request</param>
        /// <returns>Updated report information</returns>
        [HttpPut("{reportId}")]
        public async Task<ActionResult<object>> UpdateReport(int reportId, [FromBody] ReportUpdateDto reportDto)
        {
            try
            {
                _logger.LogInformation("Updating report with ID: {ReportId}", reportId);

                var validation = ReportValidator.Validate(reportDto);
                if (!validation.IsValid)
                    return BadRequest(new { errors = validation.Errors });

                var updatedReport = await _reportService.UpdateReport(reportId, reportDto);

                if (updatedReport == null)
                    return NotFound(new { message = $"Report with ID {reportId} not found" });

                var visited = new HashSet<(Type, int)>();
                var reportReadDto = new ReportReadDto(updatedReport, visited);

                _logger.LogInformation("Successfully updated report with ID: {ReportId}", reportId);
                return Ok(reportReadDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating report with ID: {ReportId}", reportId);
                return StatusCode(500, new
                {
                    message = "An error occurred while updating the report"
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
        public async Task<ActionResult<IEnumerable<ReportReadDto>>> SearchReports([FromQuery] string jobNumber, [FromQuery] int limit = 10)
        {
            if (string.IsNullOrWhiteSpace(jobNumber))
                return BadRequest(new { message = "jobNumber query parameter is required" });

            var reports = await _reportService.SearchReportsByJobNumber(jobNumber, limit);

            if (!reports.Any())
                return NotFound(new { message = "No reports found" });

            var visited = new HashSet<(Type, int)>();
            var reportDtos = reports.Select(r => r.ToDto(visited)).Where(dto => dto != null).ToList();

            return Ok(reportDtos);
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

                // Use the service to get reports by job number directly
                var reports = await _reportService.GetReportsByJobAsync(jobNumber);

                _logger.LogInformation("Successfully retrieved {ReportCount} reports for job {JobNumber}", reports.Count(), jobNumber);
                return Ok(reports);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving reports for job {JobNumber}", jobNumber);
                return StatusCode(500, new { 
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
                    return NotFound(new { 
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
                return StatusCode(500, new { 
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
                return StatusCode(500, new { 
                    message = "An error occurred while creating the report"
                });
            }
        }

        /// <summary>
        /// Get proctors available for a specific job
        /// </summary>
        [HttpGet("proctors/job/{jobId}")]
        public async Task<ActionResult<object>> GetProctorsForJob(int jobId)
        {
            try
            {
                _logger.LogInformation("Getting proctors for job {JobId}", jobId);

                var proctors = await _dbContext.Proctors
                    .Include(p => p.ProctorType)
                    .Include(p => p.LabTest)
                    .Where(p => p.LabTest.JobId == jobId)
                    .Select(p => new
                    {
                        p.Id,
                        p.ProctorID,
                        p.MaxDensity,
                        p.CorrectedDensity,
                        p.OptimumMoistureContent,
                        p.SpecificGravity,
                        ProctorType = p.ProctorType.Type,
                        MaterialType = p.LabTest.MaterialType
                    })
                    .ToListAsync();

                _logger.LogInformation("Found {Count} proctors for job {JobId}", proctors.Count, jobId);
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
        [HttpPost("{reportId}/density-test")]
        public async Task<ActionResult<object>> CreateDensityTest(int reportId, [FromBody] CreateDensityTestRequest request)
        {
            try
            {
                _logger.LogInformation("Creating density test for report {ReportId}", reportId);

                // Verify report exists
                var report = await _dbContext.Reports.FindAsync(reportId);
                if (report == null)
                {
                    return NotFound(new { message = "Report not found" });
                }

                // Create density test
                var densityTest = new DensityTest
                {
                    ReportId = reportId,
                    ProctorId = request.ProctorId,
                    TestArea = request.TestArea,
                    Location = request.Location,
                    ElevationReference = Enum.Parse<ElevationReference>(request.ElevationReference),
                    ElevationValue = request.ElevationValue,
                    ElevationUnit = Enum.Parse<ElevationUnit>(request.ElevationUnit),
                    CorrectedOversizePercentage = (float)request.CorrectedOversizePercentage,
                    ProbeDepth = (int)request.ProbeDepth,
                    ProbeDepthUnit = Enum.Parse<ProbeDepthUnit>(request.ProbeDepthUnit),
                    CompactionSpecification = request.CompactionSpecification,
                    CompactionSpecificationUnit = Enum.Parse<CompactionSpecificationUnit>(request.CompactionSpecificationUnit),
                    DensityValue = request.DensityValue,
                    MoistureValue = request.MoistureValue,
                    CreatedDate = DateTime.UtcNow
                };

                _dbContext.DensityTests.Add(densityTest);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Density test created with ID {DensityTestId}", densityTest.Id);

                return Ok(new { 
                    id = densityTest.Id,
                    message = "Density test created successfully" 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating density test for report {ReportId}", reportId);
                return StatusCode(500, new { 
                    message = "An error occurred while creating the density test" 
                });
            }
        }
    }


    public class CreateDensityTestRequest
    {
        public required int ProctorId { get; set; }
        public string? TestArea { get; set; }
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
}
