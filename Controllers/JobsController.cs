using DensityReportingToolBackend.Data;
using DensityReportingToolBackend.Models;
using DensityReportingToolBackend.Services;
using DensityReportingToolBackend.Validators;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

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

        [HttpPost("{jobNumber}/notes")]
        public async Task<ActionResult<object>> AddJobNote(string jobNumber, [FromBody] CreateJobNoteRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Note))
            {
                return BadRequest(new { message = "Note content is required." });
            }

            try
            {
                _logger.LogInformation("Adding note to job {JobNumber}", jobNumber);

                var job = await _dbContext.Jobs.FirstOrDefaultAsync(j => j.JobNumber == jobNumber);
                if (job == null)
                {
                    return NotFound(new { message = $"Job with number {jobNumber} not found" });
                }

                var jobNote = new JobNote
                {
                    JobId = job.Id,
                    Note = request.Note.Trim(),
                    CreatedDate = DateTime.UtcNow
                };

                _dbContext.JobNotes.Add(jobNote);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Successfully added note {JobNoteId} to job {JobNumber}", jobNote.Id, jobNumber);

                return Ok(new JobNoteReadDto(jobNote));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding note to job {JobNumber}", jobNumber);
                return StatusCode(500, new { message = "An error occurred while adding the job note" });
            }
        }

        [HttpDelete("{jobNumber}/notes/{noteId:int}")]
        public async Task<ActionResult<object>> DeleteJobNote(string jobNumber, int noteId)
        {
            try
            {
                _logger.LogInformation("Deleting note {JobNoteId} from job {JobNumber}", noteId, jobNumber);

                var job = await _dbContext.Jobs.FirstOrDefaultAsync(j => j.JobNumber == jobNumber);
                if (job == null)
                {
                    return NotFound(new { message = $"Job with number {jobNumber} not found" });
                }

                var jobNote = await _dbContext.JobNotes.FirstOrDefaultAsync(jn => jn.Id == noteId && jn.JobId == job.Id);
                if (jobNote == null)
                {
                    return NotFound(new { message = $"Job note {noteId} not found for job {jobNumber}" });
                }

                _dbContext.JobNotes.Remove(jobNote);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Successfully deleted note {JobNoteId} from job {JobNumber}", noteId, jobNumber);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting note {JobNoteId} from job {JobNumber}", noteId, jobNumber);
                return StatusCode(500, new { message = "An error occurred while deleting the job note" });
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

        // ==================== PROJECT MANAGER ENDPOINTS ====================

        /// <summary>
        /// Assign a project manager to a job (deactivates existing active primary PM if assigning a new primary)
        /// </summary>
        /// <param name="jobNumber">The job number to assign the PM to</param>
        /// <param name="dto">Project manager assignment details</param>
        /// <returns>Created project manager assignment</returns>
        [HttpPost("{jobNumber}/project-manager")]
        public async Task<ActionResult<object>> AssignProjectManager(
            string jobNumber,
            [FromBody] AssignProjectManagerDto dto)
        {
            try
            {
                _logger.LogInformation("Assigning PM {PersonalInfoId} to job {JobNumber}", dto.PersonalInfoId, jobNumber);

                // Get the job
                var job = await _dbContext.Jobs
                    .FirstOrDefaultAsync(j => j.JobNumber == jobNumber);

                if (job == null)
                {
                    return NotFound(new { message = $"Job {jobNumber} not found" });
                }

                // Verify the person exists
                var person = await _dbContext.PersonalInfos
                    .FirstOrDefaultAsync(p => p.Id == dto.PersonalInfoId);

                if (person == null)
                {
                    return NotFound(new { message = $"Person with ID {dto.PersonalInfoId} not found" });
                }

                // If assigning a primary PM, deactivate any existing active primary PMs for this job
                if (dto.IsPrimary)
                {
                    var existingPrimaryPMs = await _dbContext.JobProjectManagers
                        .Where(pm => pm.JobId == job.Id && pm.IsPrimary && pm.IsActive)
                        .ToListAsync();

                    foreach (var pm in existingPrimaryPMs)
                    {
                        pm.IsActive = false;
                    }
                }

                // Create new project manager assignment
                var newPM = new JobProjectManager
                {
                    JobId = job.Id,
                    PersonalInfoId = dto.PersonalInfoId,
                    StartDate = DateTime.UtcNow,
                    IsPrimary = dto.IsPrimary,
                    IsActive = true
                };

                _dbContext.JobProjectManagers.Add(newPM);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Successfully assigned PM {PersonalInfoId} to job {JobNumber}", dto.PersonalInfoId, jobNumber);

                return Ok(new
                {
                    message = "Project manager assigned successfully",
                    projectManager = new
                    {
                        id = newPM.Id,
                        personalInfoId = newPM.PersonalInfoId,
                        fullName = $"{person.FirstName} {person.LastName}",
                        isPrimary = newPM.IsPrimary,
                        isActive = newPM.IsActive
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning PM to job {JobNumber}", jobNumber);
                return StatusCode(500, new { message = "An error occurred while assigning the project manager" });
            }
        }

        /// <summary>
        /// Remove/deactivate a project manager from a job
        /// </summary>
        /// <param name="jobNumber">The job number</param>
        /// <param name="projectManagerId">The ID of the project manager to remove</param>
        /// <returns>Success message</returns>
        [HttpDelete("{jobNumber}/project-manager/remove/{projectManagerId:int}")]
        public async Task<ActionResult<object>> RemoveProjectManager(
            string jobNumber,
            int projectManagerId)
        {
            try
            {
                _logger.LogInformation("Removing PM {ProjectManagerId} from job {JobNumber}", projectManagerId, jobNumber);

                var job = await _dbContext.Jobs
                    .FirstOrDefaultAsync(j => j.JobNumber == jobNumber);

                if (job == null)
                {
                    return NotFound(new { message = $"Job {jobNumber} not found" });
                }

                var projectManager = await _dbContext.JobProjectManagers
                    .FirstOrDefaultAsync(pm => pm.Id == projectManagerId && pm.JobId == job.Id);

                if (projectManager == null)
                {
                    return NotFound(new { message = $"Project manager {projectManagerId} not found for job {jobNumber}" });
                }

                projectManager.IsActive = false;
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Successfully removed PM {ProjectManagerId} from job {JobNumber}", projectManagerId, jobNumber);

                return Ok(new { message = "Project manager removed successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing PM {ProjectManagerId} from job {JobNumber}", projectManagerId, jobNumber);
                return StatusCode(500, new { message = "An error occurred while removing the project manager" });
            }
        }

        // ==================== SITE CONTACT ENDPOINTS ====================

        /// <summary>
        /// Assign a site contact to a job (deactivates existing active primary contact if assigning a new primary)
        /// </summary>
        /// <param name="jobNumber">The job number to assign the site contact to</param>
        /// <param name="dto">Site contact assignment details</param>
        /// <returns>Created site contact assignment</returns>
        [HttpPost("{jobNumber}/site-contact")]
        public async Task<ActionResult<object>> AssignSiteContact(
            string jobNumber,
            [FromBody] AssignSiteContactDto dto)
        {
            try
            {
                _logger.LogInformation("Assigning site contact {PersonalInfoId} to job {JobNumber}", dto.PersonalInfoId, jobNumber);

                // Get the job
                var job = await _dbContext.Jobs
                    .FirstOrDefaultAsync(j => j.JobNumber == jobNumber);

                if (job == null)
                {
                    return NotFound(new { message = $"Job {jobNumber} not found" });
                }

                // Verify the person exists
                var person = await _dbContext.PersonalInfos
                    .FirstOrDefaultAsync(p => p.Id == dto.PersonalInfoId);

                if (person == null)
                {
                    return NotFound(new { message = $"Person with ID {dto.PersonalInfoId} not found" });
                }

                // If assigning a primary site contact, deactivate any existing active primary contacts for this job
                if (dto.IsPrimary)
                {
                    var existingPrimaryContacts = await _dbContext.JobSiteContacts
                        .Where(sc => sc.JobId == job.Id && sc.IsPrimary && sc.IsActive)
                        .ToListAsync();

                    foreach (var contact in existingPrimaryContacts)
                    {
                        contact.IsActive = false;
                    }
                }

                // Create new site contact assignment
                var newSiteContact = new JobSiteContact
                {
                    JobId = job.Id,
                    PersonalInfoId = dto.PersonalInfoId,
                    IsPrimary = dto.IsPrimary,
                    IsActive = true
                };

                _dbContext.JobSiteContacts.Add(newSiteContact);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Successfully assigned site contact {PersonalInfoId} to job {JobNumber}", dto.PersonalInfoId, jobNumber);

                return Ok(new
                {
                    message = "Site contact assigned successfully",
                    siteContact = new
                    {
                        id = newSiteContact.Id,
                        personalInfoId = newSiteContact.PersonalInfoId,
                        fullName = $"{person.FirstName} {person.LastName}",
                        isPrimary = newSiteContact.IsPrimary,
                        isActive = newSiteContact.IsActive
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning site contact to job {JobNumber}", jobNumber);
                return StatusCode(500, new { message = "An error occurred while assigning the site contact" });
            }
        }

        /// <summary>
        /// Remove/deactivate a site contact from a job
        /// </summary>
        /// <param name="jobNumber">The job number</param>
        /// <param name="siteContactId">The ID of the site contact to remove</param>
        /// <returns>Success message</returns>
        [HttpDelete("{jobNumber}/site-contact/remove/{siteContactId:int}")]
        public async Task<ActionResult<object>> RemoveSiteContact(
            string jobNumber,
            int siteContactId)
        {
            try
            {
                _logger.LogInformation("Removing site contact {SiteContactId} from job {JobNumber}", siteContactId, jobNumber);

                var job = await _dbContext.Jobs
                    .FirstOrDefaultAsync(j => j.JobNumber == jobNumber);

                if (job == null)
                {
                    return NotFound(new { message = $"Job {jobNumber} not found" });
                }

                var siteContact = await _dbContext.JobSiteContacts
                    .FirstOrDefaultAsync(sc => sc.Id == siteContactId && sc.JobId == job.Id);

                if (siteContact == null)
                {
                    return NotFound(new { message = $"Site contact {siteContactId} not found for job {jobNumber}" });
                }

                siteContact.IsActive = false;
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Successfully removed site contact {SiteContactId} from job {JobNumber}", siteContactId, jobNumber);

                return Ok(new { message = "Site contact removed successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing site contact {SiteContactId} from job {JobNumber}", siteContactId, jobNumber);
                return StatusCode(500, new { message = "An error occurred while removing the site contact" });
            }
        }

        public class CreateJobNoteRequest
        {
            [Required]
            public string Note { get; set; } = string.Empty;
        }
    }
}
