using DensityReportingToolBackend.DTOs.Jobs;
using DensityReportingToolBackend.Infrastructure;
using DensityReportingToolBackend.Services;
using Microsoft.AspNetCore.Mvc;

namespace DensityReportingToolBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobsController(IJobService jobService, ILogger<JobsController> logger) : BaseApiController
    {

        /// <summary>
        /// Get all jobs for dashboard/schedule view
        /// </summary>
        /// <returns>List of all jobs with basic information</returns>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<JobReadDto>>>> GetAllJobs()
        {
            logger.LogInformation("Retrieving all jobs for dashboard");
            var result = await jobService.ListJobsAsync();
            return Success(result, "Jobs retrieved successfully");
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<JobReadDto>>> CreateJob(JobCreateDto dto)
        {
            var result = await jobService.CreateJobAsync(dto);
            var response = ApiResponse<JobReadDto>.SuccessResponse(result, "Job created successfully");
            return Created(nameof(GetJob), new { jobNumber = result.Id }, result);
        }

        /// <summary>
        /// Get job information by job number
        /// </summary>
        /// <param name="jobNumber">The job number to search for (can be numeric like "25482" or alphanumeric like "15827-A")</param>
        /// <returns>Job information including client, project details, and key relationships</returns>
        [HttpGet("{jobNumber}")]
        public async Task<ActionResult<ApiResponse<JobReadDto>>> GetJob(string jobNumber)
        {
            // 1. The service now returns the DTO directly.
            // Logic for "null means 404" is handled either here or in a Global Filter.
            var jobDto = await jobService.GetJobByNumberAsync(jobNumber);

            if (jobDto == null)
            {
                logger.LogWarning("Job {JobNumber} was requested but not found", jobNumber);
                return Failure<JobReadDto>($"Job {jobNumber} not found", 404);
            }

            return Success(jobDto);
        }

        // [HttpPut("{jobId}")]
        // public async Task<ActionResult<object>> UpdateJob(int jobId, [FromBody] JobUpdateDto jobDto)
        // {
        //     try
        //     {
        //         var validation = JobValidator.Validate(jobDto);
        //         if (!validation.IsValid)
        //             return BadRequest(new { errors = validation.Errors });

        //         var updatedJob = await _jobService.UpdateJob(jobId, jobDto);

        //         if (updatedJob == null)
        //             return NotFound(new { message = $"Job with ID {jobId} not found" });

        //         var visited = new HashSet<(Type, int)>();
        //         var jobReadDto = new JobReadDto(updatedJob, visited);

        //         return Ok(jobReadDto);
        //     }
        //     catch (Exception ex)
        //     {
        //         _logger.LogError(ex, "Error updating job with number: {JobNumber}", jobDto.JobNumber);
        //         return StatusCode(500, new
        //         {
        //             message = "An error occurred while updating the job"
        //         });
        //     }
        // }

        // [HttpGet("search")]
        // public async Task<ActionResult<IEnumerable<JobReadDto>>> SearchJobs([FromQuery] string jobNumber, [FromQuery] int limit = 10)
        // {
        //     if (string.IsNullOrWhiteSpace(jobNumber))
        //         return BadRequest(new { message = "jobNumber query parameter is required" });

        //     var jobs = await _jobService.SearchJobsByJobNumber(jobNumber, limit);

        //     if (!jobs.Any())
        //         return NotFound(new { message = "No jobs found" });

        //     var visited = new HashSet<(Type, int)>();
        //     var jobDtos = jobs.Select(j => j.ToDto(visited)).Where(dto => dto != null).ToList();

        //     return Ok(jobDtos);
        // }
    }
}
