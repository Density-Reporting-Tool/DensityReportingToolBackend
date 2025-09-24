using DensityReportingToolBackend.Data;
using DensityReportingToolBackend.Models;
using DensityReportingToolBackend.Services;
using DensityReportingToolBackend.Validators;
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
                return StatusCode(500, new
                {
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
                    return NotFound(new
                    {
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
                return StatusCode(500, new
                {
                    message = "An error occurred while retrieving the job",
                    jobNumber = jobNumber
                });
            }
        }

        /// <summary>
        /// Create a new job
        /// </summary>
        /// <param name="jobDto">Job creation request with all job details</param>
        /// <returns>Created job information</returns>
        [HttpPost]
        public async Task<ActionResult<object>> CreateJob([FromBody] JobCreateDto jobDto)
        {
            try
            {
                _logger.LogInformation("Creating new job with number: {JobNumber}", jobDto.JobNumber);

                // Validate required fields
                var validation = JobValidator.Validate(jobDto);

                if (!validation.IsValid)
                {
                    return BadRequest(new
                    {
                        errors = validation.Errors
                    });
                }

                // Check if job number already exists
                var existingJob = await _dbContext.Jobs
                    .FirstOrDefaultAsync(j => j.JobNumber == jobDto.JobNumber);

                if (existingJob != null)
                {
                    return BadRequest(new { message = $"Job with number {jobDto.JobNumber} already exists" });
                }

                // Create new job
                var newJob = await _jobService.CreateJob(jobDto);

                // TODO Add job note if provided

                _logger.LogInformation("Successfully created job with ID: {JobId} and number: {JobNumber}",
                    newJob.Id, newJob.JobNumber);

                return CreatedAtAction(nameof(GetJob), new { jobNumber = newJob.JobNumber }, newJob.ToDto());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating job with number: {JobNumber}", jobDto.JobNumber);
                return StatusCode(500, new
                {
                    message = "An error occurred while creating the job"
                });
            }
        }

        [HttpPut("{jobId}")]
        public async Task<ActionResult<object>> UpdateJob(int jobId, [FromBody] JobUpdateDto jobDto)
        {
            try
            {
                var validation = JobValidator.Validate(jobDto);
                if (!validation.IsValid)
                    return BadRequest(new { errors = validation.Errors });

                var updatedJob = await _jobService.UpdateJob(jobId, jobDto);

                if (updatedJob == null)
                    return NotFound(new { message = $"Job with ID {jobId} not found" });

                var visited = new HashSet<(Type, int)>();
                var jobReadDto = new JobReadDto(updatedJob, visited);

                return Ok(jobReadDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating job with number: {JobNumber}", jobDto.JobNumber);
                return StatusCode(500, new
                {
                    message = "An error occurred while updating the job"
                });
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<JobReadDto>>> SearchJobs([FromQuery] string jobNumber, [FromQuery] int limit = 10)
        {
            if (string.IsNullOrWhiteSpace(jobNumber))
                return BadRequest(new { message = "jobNumber query parameter is required" });

            var jobs = await _jobService.SearchJobsByJobNumber(jobNumber, limit);

            if (!jobs.Any())
                return NotFound(new { message = "No jobs found" });

            var visited = new HashSet<(Type, int)>();
            var jobDtos = jobs.Select(j => j.ToDto(visited)).Where(dto => dto != null).ToList();

            return Ok(jobDtos);
        }
    }
}
