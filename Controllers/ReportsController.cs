using DensityReportingToolBackend.Data;
using DensityReportingToolBackend.Models;
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

        public ReportsController(ILogger<ReportsController> logger, AppDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        /// <summary>
        /// Get all reports for a specific job
        /// </summary>
        /// <param name="jobId">The job ID to get reports for</param>
        /// <returns>List of reports for the job</returns>
        [HttpGet("job/{jobId}")]
        public async Task<ActionResult<object>> GetReportsByJob(int jobId)
        {
            try
            {
                _logger.LogInformation("Retrieving reports for job ID: {JobId}", jobId);

                var reports = await _dbContext.Reports
                    .Include(r => r.Employee)
                        .ThenInclude(e => e.PersonalInfo)
                    .Include(r => r.Reviewer)
                        .ThenInclude(r => r.PersonalInfo)
                    .Include(r => r.DensityTests)
                    .Include(r => r.Photos)
                    .Include(r => r.Memos)
                    .Where(r => r.JobId == jobId)
                    .OrderByDescending(r => r.ReportNumber)
                    .ToListAsync();

                var result = reports.Select(report => new
                {
                    Id = report.Id,
                    JobId = report.JobId,
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
                    DensityTestsCount = report.DensityTests.Count,
                    PhotosCount = report.Photos.Count,
                    MemosCount = report.Memos.Count,
                    DistributionListId = report.DistributionListId
                }).ToList();

                _logger.LogInformation("Successfully retrieved {ReportCount} reports for job {JobId}", result.Count, jobId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving reports for job {JobId}", jobId);
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

                // Validate required fields
                if (reportRequest.JobId <= 0)
                {
                    return BadRequest(new { message = "Valid Job ID is required" });
                }

                if (reportRequest.EmployeeId <= 0)
                {
                    return BadRequest(new { message = "Valid Employee ID is required" });
                }

                if (reportRequest.ReviewerId <= 0)
                {
                    return BadRequest(new { message = "Valid Reviewer ID is required" });
                }

                // Check if job exists
                var job = await _dbContext.Jobs.FindAsync(reportRequest.JobId);
                if (job == null)
                {
                    return BadRequest(new { message = $"Job with ID {reportRequest.JobId} not found" });
                }

                // Check if employee exists
                var employee = await _dbContext.GeoPacificEmployees
                    .Include(e => e.PersonalInfo)
                    .FirstOrDefaultAsync(e => e.Id == reportRequest.EmployeeId);
                if (employee == null)
                {
                    return BadRequest(new { message = $"Employee with ID {reportRequest.EmployeeId} not found" });
                }

                // Check if reviewer exists
                var reviewer = await _dbContext.GeoPacificEmployees
                    .Include(e => e.PersonalInfo)
                    .FirstOrDefaultAsync(e => e.Id == reportRequest.ReviewerId);
                if (reviewer == null)
                {
                    return BadRequest(new { message = $"Reviewer with ID {reportRequest.ReviewerId} not found" });
                }

                // Get the next report number for this job
                var maxReportNumber = await _dbContext.Reports
                    .Where(r => r.JobId == reportRequest.JobId)
                    .MaxAsync(r => (int?)r.ReportNumber) ?? 0;

                // Create new report
                var newReport = new Report
                {
                    JobId = reportRequest.JobId,
                    EmployeeId = reportRequest.EmployeeId,
                    ReviewerId = reportRequest.ReviewerId,
                    ReportNumber = maxReportNumber + 1,
                    StartDate = reportRequest.StartDate ?? DateTime.UtcNow,
                    SubmitDate = reportRequest.SubmitDate,
                    DistributeDate = reportRequest.DistributeDate,
                    DistributionListId = reportRequest.DistributionListId
                };

                _dbContext.Reports.Add(newReport);
                await _dbContext.SaveChangesAsync();

                // Create memo if provided
                if (reportRequest.Memo != null)
                {
                    var memo = new ReportMemo
                    {
                        ReportId = newReport.Id,
                        Purpose = reportRequest.Memo.Purpose,
                        CommentsAndObservations = reportRequest.Memo.CommentsAndObservations,
                        Conclusion = reportRequest.Memo.Conclusion,
                        CreatedDate = DateTime.UtcNow
                    };

                    _dbContext.ReportMemos.Add(memo);
                    await _dbContext.SaveChangesAsync();

                    _logger.LogInformation("Successfully created memo for report ID: {ReportId}", newReport.Id);
                }

                _logger.LogInformation("Successfully created report with ID: {ReportId} and number: {ReportNumber}", 
                    newReport.Id, newReport.ReportNumber);

                return CreatedAtAction(nameof(GetReport), new { reportId = newReport.Id }, new
                {
                    Id = newReport.Id,
                    JobId = newReport.JobId,
                    ReportNumber = newReport.ReportNumber,
                    StartDate = newReport.StartDate,
                    SubmitDate = newReport.SubmitDate,
                    DistributeDate = newReport.DistributeDate,
                    Employee = new
                    {
                        employee.Id,
                        FirstName = employee.PersonalInfo.FirstName,
                        LastName = employee.PersonalInfo.LastName,
                        Email = employee.PersonalInfo.Email,
                        PhoneNumber = employee.PersonalInfo.PhoneNumber
                    },
                    Reviewer = new
                    {
                        reviewer.Id,
                        FirstName = reviewer.PersonalInfo.FirstName,
                        LastName = reviewer.PersonalInfo.LastName,
                        Email = reviewer.PersonalInfo.Email,
                        PhoneNumber = reviewer.PersonalInfo.PhoneNumber
                    },
                    Message = "Report created successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating report for job {JobId}", reportRequest.JobId);
                return StatusCode(500, new { 
                    message = "An error occurred while creating the report"
                });
            }
        }
    }

    /// <summary>
    /// Request model for creating a new report
    /// </summary>
    public class CreateReportRequest
    {
        public required int JobId { get; set; }
        public required int EmployeeId { get; set; }
        public required int ReviewerId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? SubmitDate { get; set; }
        public DateTime? DistributeDate { get; set; }
        public int? DistributionListId { get; set; }
        public ReportMemoRequest? Memo { get; set; }
    }

    public class ReportMemoRequest
    {
        public string? Purpose { get; set; }
        public string? CommentsAndObservations { get; set; }
        public string? Conclusion { get; set; }
    }
}
