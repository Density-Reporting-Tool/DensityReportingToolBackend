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
            var jobDto = await jobService.GetJobByNumberAsync(jobNumber);

            if (jobDto == null)
            {
                logger.LogWarning("Job {JobNumber} was requested but not found", jobNumber);
                return Failure<JobReadDto>($"Job {jobNumber} not found", 404);
            }

            return Success(jobDto);
        }

        [HttpPut("{jobId}")]
        public async Task<ActionResult<ApiResponse<JobReadDto>>> UpdateJob(int jobId, [FromBody] JobUpdateDto jobDto)
        {
            var result = await jobService.UpdateJobAsync(jobId, jobDto);

            if (result == null)
            {
                return Failure<JobReadDto>($"Job with ID {jobId} not found", 404);
            }

            return Success(result, "Job updated successfully");
        }

        [HttpGet("search/{jobNumber}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<JobReadDto>>>> SearchJobs(
            string jobNumber, 
            [FromQuery] int limit = 10)
        {
            if (string.IsNullOrWhiteSpace(jobNumber))
            {
                return Failure<IEnumerable<JobReadDto>>("Job number search term is required", 400);
            }

            var jobs = await jobService.SearchJobsByJobNumberAsync(jobNumber, limit);

            if (jobs == null || !jobs.Any())
            {
                return Success(Enumerable.Empty<JobReadDto>(), "No jobs matched your search criteria");
            }

            return Success(jobs, $"Found {jobs.Count()} jobs matching '{jobNumber}'");
        }
    }
}
