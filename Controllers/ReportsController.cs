using DensityReportingToolBackend.Data;
using DensityReportingToolBackend.Models;
using DensityReportingToolBackend.Models.DTOs;
using DensityReportingToolBackend.Services;
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

                var report = await _dbContext.Reports
                    .Include(r => r.Job)
                    .Include(r => r.Employee)
                        .ThenInclude(e => e.PersonalInfo)
                    .Include(r => r.Reviewer)
                        .ThenInclude(r => r.PersonalInfo)
                    .Include(r => r.DensityTests)
                    .Include(r => r.Photos)
                    .Include(r => r.Memos)
                    .FirstOrDefaultAsync(r => r.Id == reportId);

                if (report == null)
                {
                    _logger.LogWarning("Report with ID {ReportId} not found", reportId);
                    return NotFound(new { 
                        message = $"Report with ID {reportId} not found",
                        reportId = reportId
                    });
                }

                var result = new
                {
                    Id = report.Id,
                    JobId = report.JobId,
                    Job = new
                    {
                        report.Job.Id,
                        report.Job.JobNumber,
                        report.Job.ClientName,
                        report.Job.ProjectName
                    },
                    ReportNumber = report.ReportNumber,
                    StartDate = report.StartDate,
                    SubmitDate = report.SubmitDate,
                    DistributeDate = report.DistributeDate,
                    Employee = new
                    {
                        report.Employee.Id,
                        FirstName = report.Employee.PersonalInfo.FirstName,
                        LastName = report.Employee.PersonalInfo.LastName,
                        Email = report.Employee.PersonalInfo.Email,
                        PhoneNumber = report.Employee.PersonalInfo.PhoneNumber
                    },
                    Reviewer = new
                    {
                        report.Reviewer.Id,
                        FirstName = report.Reviewer.PersonalInfo.FirstName,
                        LastName = report.Reviewer.PersonalInfo.LastName,
                        Email = report.Reviewer.PersonalInfo.Email,
                        PhoneNumber = report.Reviewer.PersonalInfo.PhoneNumber
                    },
                    DensityTests = report.DensityTests.Select(dt => new
                    {
                        dt.Id,
                        TestArea = dt.TestArea,
                        Location = dt.Location,
                        ElevationReference = dt.ElevationReference,
                        ElevationValue = dt.ElevationValue,
                        ElevationUnit = dt.ElevationUnit,
                        CompactionSpecification = dt.CompactionSpecification,
                        CompactionSpecificationUnit = dt.CompactionSpecificationUnit,
                        DensityValue = dt.DensityValue,
                        MoistureValue = dt.MoistureValue,
                        CreatedDate = dt.CreatedDate
                    }),
                    Photos = report.Photos.Select(p => new
                    {
                        p.Id,
                        Code = p.Code,
                        Url = p.Url,
                        Description = p.Description,
                        Latitude = p.Latitude,
                        Longitude = p.Longitude,
                        GpsAccuracyMeters = p.GpsAccuracyMeters
                    }),
                    Memos = report.Memos.Select(m => new
                    {
                        m.Id,
                        Purpose = m.Purpose,
                        CommentsAndObservations = m.CommentsAndObservations,
                        Conclusion = m.Conclusion,
                        CreatedDate = m.CreatedDate,
                        UpdatedDate = m.UpdatedDate
                    }),
                    DistributionListId = report.DistributionListId
                };

                _logger.LogInformation("Successfully retrieved report {ReportId}", reportId);
                return Ok(result);
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
