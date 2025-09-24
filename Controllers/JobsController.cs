using DensityReportingToolBackend.Data;
using DensityReportingToolBackend.Models;
using DensityReportingToolBackend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DensityReportingToolBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobsController : ControllerBase
    {
        private readonly ILogger<JobsController> _logger;
        private readonly AppDbContext _dbContext;

        private readonly JobService _jobService;

        public JobsController(ILogger<JobsController> logger, AppDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
            _jobService = new JobService(dbContext);
        }

        /// <summary>
        /// Get all jobs for dashboard/schedule view
        /// </summary>
        /// <returns>List of all jobs with basic information</returns>
        [HttpGet]
        public async Task<ActionResult<object>> GetAllJobs()
        {
            try
            {
                _logger.LogInformation("Retrieving all jobs for dashboard");
                var jobs = await _jobService.ListJobs();
                var result = jobs.Select(job => job.ToDto()).ToList();
                _logger.LogInformation("Successfully retrieved {JobCount} jobs", result.Count);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all jobs");
                return StatusCode(500, new { 
                    message = "An error occurred while retrieving jobs"
                });
            }
        }

        /// <summary>
        /// Get job information by job number
        /// </summary>
        /// <param name="jobNumber">The job number to search for (can be numeric like "25482" or alphanumeric like "15827-A")</param>
        /// <returns>Job information including client, project details, and key relationships</returns>
        [HttpGet("{jobNumber}")]
        public async Task<ActionResult<object>> GetJob(string jobNumber)
        {
            try
            {
                _logger.LogInformation("Retrieving job with number: {JobNumber}", jobNumber);

                var job = await _jobService.GetJobByNumber(jobNumber);

                if (job == null)
                {
                    _logger.LogWarning("Job with number {JobNumber} not found", jobNumber);
                    return NotFound(new { 
                        message = $"Job with number {jobNumber} not found",
                        jobNumber = jobNumber
                    });
                }

                var result = job.ToDto();
                _logger.LogInformation("Successfully retrieved job {JobNumber}", jobNumber);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving job {JobNumber}", jobNumber);
                return StatusCode(500, new { 
                    message = "An error occurred while retrieving the job",
                    jobNumber = jobNumber
                });
            }
        }

        /// <summary>
        /// Create a new job
        /// </summary>
        /// <param name="jobRequest">Job creation request with all job details</param>
        /// <returns>Created job information</returns>
        [HttpPost]
        public async Task<ActionResult<object>> CreateJob([FromBody] CreateJobRequest jobRequest)
        {
            try
            {
                _logger.LogInformation("Creating new job with number: {JobNumber}", jobRequest.JobNumber);

                // Validate required fields
                if (string.IsNullOrWhiteSpace(jobRequest.JobNumber))
                {
                    return BadRequest(new { message = "Job Number is required" });
                }

                if (string.IsNullOrWhiteSpace(jobRequest.ProjectName))
                {
                    return BadRequest(new { message = "Project Name is required" });
                }

                if (string.IsNullOrWhiteSpace(jobRequest.SiteAddress))
                {
                    return BadRequest(new { message = "Site Address is required" });
                }

                if (string.IsNullOrWhiteSpace(jobRequest.ClientName))
                {
                    return BadRequest(new { message = "Client Name is required" });
                }

                // Check if job number already exists
                var existingJob = await _dbContext.Jobs
                    .FirstOrDefaultAsync(j => j.JobNumber == jobRequest.JobNumber);
                
                if (existingJob != null)
                {
                    return BadRequest(new { message = $"Job with number {jobRequest.JobNumber} already exists" });
                }

                // Create new job
                var newJob = new Job
                {
                    JobNumber = jobRequest.JobNumber,
                    ClientName = jobRequest.ClientName,
                    ProjectName = jobRequest.ProjectName,
                    SiteAddress = jobRequest.SiteAddress,
                    StartDate = jobRequest.StartDate,
                    EndDate = jobRequest.EndDate
                };

                _dbContext.Jobs.Add(newJob);
                await _dbContext.SaveChangesAsync();

                // Add job note if provided
                if (!string.IsNullOrWhiteSpace(jobRequest.JobNotes))
                {
                    var jobNote = new JobNote
                    {
                        JobId = newJob.Id,
                        Note = jobRequest.JobNotes
                    };
                    _dbContext.JobNotes.Add(jobNote);
                await _dbContext.SaveChangesAsync();
                }

                _logger.LogInformation("Successfully created job with ID: {JobId} and number: {JobNumber}", 
                    newJob.Id, newJob.JobNumber);

                return CreatedAtAction(nameof(GetJob), new { jobNumber = newJob.JobNumber }, new
                {
                    Id = newJob.Id,
                    JobNumber = newJob.JobNumber,
                    ClientName = newJob.ClientName,
                    Project = new
                    {
                        newJob.ProjectName,
                        newJob.SiteAddress,
                        newJob.StartDate,
                        newJob.EndDate
                    },
                    Message = "Job created successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating job with number: {JobNumber}", jobRequest.JobNumber);
                return StatusCode(500, new { 
                    message = "An error occurred while creating the job"
                });
            }
        }
    }

    /// <summary>
    /// Request model for creating a new job
    /// </summary>
    public class CreateJobRequest
    {
        public required string JobNumber { get; set; }
        public required string ClientName { get; set; }
        public required string ProjectName { get; set; }
        public required string SiteAddress { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? JobNotes { get; set; }
    }
}
