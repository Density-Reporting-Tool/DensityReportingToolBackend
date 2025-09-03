using DensityReportingToolBackend.Data;
using DensityReportingToolBackend.Models;
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

        public JobsController(ILogger<JobsController> logger, AppDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
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
                
                var job = await _dbContext.Jobs
                    .Include(j => j.Client)
                    .Include(j => j.ProjectManagers)
                        .ThenInclude(jpm => jpm.PersonalInfo)
                    .Include(j => j.SiteContacts)
                        .ThenInclude(jsc => jsc.PersonalInfo)
                    .Include(j => j.DistributionLists)
                        .ThenInclude(dl => dl.DistributionMembers)
                            .ThenInclude(dm => dm.PersonalInfo)
                    .FirstOrDefaultAsync(j => j.JobNumber == jobNumber);

                if (job == null)
                {
                    _logger.LogWarning("Job with number {JobNumber} not found", jobNumber);
                    return NotFound(new { 
                        message = $"Job with number {jobNumber} not found",
                        jobNumber = jobNumber
                    });
                }

                // Return job information
                var result = new
                {
                    Id = job.Id,
                    JobNumber = job.JobNumber,
                    ClientName = job.Client.Name,
                    Project = new
                    {
                        job.ProjectName,
                        job.SiteAddress,
                        job.StartDate,
                        job.EndDate
                    },
                    ProjectManagers = job.ProjectManagers?
                        .Where(pm => pm.IsActive && pm.EndDate == null)
                        .Select(pm => new
                        {
                            pm.PersonalInfo.Id,
                            FirstName = pm.PersonalInfo.FirstName,
                            LastName = pm.PersonalInfo.LastName,
                            Email = pm.PersonalInfo.Email,
                            PhoneNumber = pm.PersonalInfo.PhoneNumber,
                            pm.Notes,
                            pm.StartDate
                        }) ?? Enumerable.Empty<object>(),
                    SiteContacts = job.SiteContacts?
                        .Where(sc => sc.IsActive && sc.EndDate == null)
                        .Select(sc => new
                        {
                            sc.PersonalInfo.Id,
                            FirstName = sc.PersonalInfo.FirstName,
                            LastName = sc.PersonalInfo.LastName,
                            Email = sc.PersonalInfo.Email,
                            PhoneNumber = sc.PersonalInfo.PhoneNumber,
                            sc.Area,
                            sc.Company,
                            sc.Role,
                            sc.IsPrimary,
                            sc.Notes
                        }) ?? Enumerable.Empty<object>(),
                    DistributionLists = job.DistributionLists?
                        .Select(dl => new
                        {
                            dl.Id,
                            dl.Name,
                            dl.Description,
                            Members = dl.DistributionMembers?
                                .Select(dm => new
                                {
                                    dm.PersonalInfo.Id,
                                    FirstName = dm.PersonalInfo.FirstName,
                                    LastName = dm.PersonalInfo.LastName,
                                    Email = dm.PersonalInfo.Email
                                }) ?? Enumerable.Empty<object>()
                        }) ?? Enumerable.Empty<object>()
                };

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
    }
}
