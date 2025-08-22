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

        // GET: api/jobs/test
        [HttpGet("test")]
        public ActionResult<object> TestEndpoint()
        {
            return Ok(new { message = "JobsController is working!", timestamp = DateTime.UtcNow });
        }

        // GET: api/jobs/db-test
        [HttpGet("db-test")]
        public async Task<ActionResult<object>> TestDatabaseConnection()
        {
            try
            {
                // Test if we can connect to the database
                var canConnect = await _dbContext.Database.CanConnectAsync();
                var connectionString = _dbContext.Database.GetConnectionString();
                
                return Ok(new { 
                    message = "Database connection test", 
                    canConnect = canConnect,
                    connectionString = connectionString?.Substring(0, Math.Min(50, connectionString?.Length ?? 0)) + "...",
                    timestamp = DateTime.UtcNow 
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    error = "Database connection failed", 
                    message = ex.Message,
                    timestamp = DateTime.UtcNow 
                });
            }
        }

        // GET: api/jobs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetJobs()
        {
            try
            {
                _logger.LogInformation("Retrieving all jobs");
                
                // Start with a simple query to get basic job data
                var jobs = await _dbContext.Jobs
                    .Select(j => new
                    {
                        j.Id,
                        ClientName = j.Client.Name,
                        j.ProjectName,
                        j.SiteAddress,
                        j.StartDate,
                        j.EndDate
                    })
                    .ToListAsync();

                return Ok(jobs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving jobs");
                return StatusCode(500, "Internal server error occurred while retrieving jobs");
            }
        }

        // GET: api/jobs/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetJob(int id)
        {
            try
            {
                _logger.LogInformation("Retrieving job with ID: {JobId}", id);
                
                var job = await _dbContext.Jobs
                    .Include(j => j.Client)
                    .Include(j => j.ProjectManagers)
                        .ThenInclude(jpm => jpm.Employee)
                            .ThenInclude(e => e.PersonalInfo)
                    .Include(j => j.SiteContacts)
                        .ThenInclude(jsc => jsc.PersonalInfo)
                    .Include(j => j.DistributionLists)
                        .ThenInclude(dl => dl.DistributionMembers)
                            .ThenInclude(dm => dm.PersonalInfo)
                    .Include(j => j.JobContracts)
                        .ThenInclude(jc => jc.Contractor)
                            .ThenInclude(c => c.PersonalInfo)
                    .Include(j => j.LabTests)
                        .ThenInclude(lt => lt.Proctors)
                            .ThenInclude(p => p.ProctorType)
                    .Include(j => j.ProctorAdditionalJobs)
                        .ThenInclude(paj => paj.Proctor)
                            .ThenInclude(p => p.ProctorType)
                    .Include(j => j.Reports)
                    .Include(j => j.SitePlans)
                    .AsSplitQuery()
                    .FirstOrDefaultAsync(j => j.Id == id);

                if (job == null)
                {
                    _logger.LogWarning("Job with ID {JobId} not found", id);
                    return NotFound($"Job with ID {id} not found");
                }

                // Return a clean object to avoid circular references
                var result = new
                {
                    job.Id,
                    ClientName = job.Client?.Name,
                    job.ProjectName,
                    job.SiteAddress,
                    job.StartDate,
                    job.EndDate,
                    ProjectManagers = job.ProjectManagers?
                        .Where(pm => pm.IsActive && pm.EndDate == null && pm.Employee?.PersonalInfo != null)
                        .Select(pm => new
                        {
                            pm.Employee.Id,
                            pm.Employee.PersonalInfo.FirstName,
                            pm.Employee.PersonalInfo.LastName,
                            pm.Employee.PersonalInfo.Email,
                            pm.Employee.PersonalInfo.PhoneNumber,
                            pm.Notes,
                            pm.StartDate
                        }) ?? Enumerable.Empty<object>(),
                    SiteContacts = job.SiteContacts?
                        .Where(sc => sc.IsActive && sc.EndDate == null && sc.PersonalInfo != null)
                        .Select(sc => new
                        {
                            sc.PersonalInfo.Id,
                            sc.PersonalInfo.FirstName,
                            sc.PersonalInfo.LastName,
                            sc.PersonalInfo.Email,
                            sc.PersonalInfo.PhoneNumber,
                            sc.Area,
                            sc.Company,
                            sc.Role,
                            sc.IsPrimary,
                            sc.Notes,
                            sc.StartDate
                        }) ?? Enumerable.Empty<object>(),
                    DistributionLists = job.DistributionLists?
                        .Where(dl => dl.DistributionMembers != null)
                        .Select(dl => new
                        {
                            dl.Id,
                            dl.Name,
                            dl.Description,
                            Members = dl.DistributionMembers
                                .Where(dm => dm.PersonalInfo != null)
                                .Select(dm => new
                                {
                                    dm.PersonalInfo.Id,
                                    dm.PersonalInfo.FirstName,
                                    dm.PersonalInfo.LastName,
                                    dm.PersonalInfo.Email
                                })
                        }) ?? Enumerable.Empty<object>(),
                    Contractors = job.JobContracts?
                        .Where(jc => jc.Contractor?.PersonalInfo != null)
                        .Select(jc => new
                        {
                            jc.Contractor.Id,
                            jc.Contractor.PersonalInfo.FirstName,
                            jc.Contractor.PersonalInfo.LastName,
                            jc.Contractor.PersonalInfo.Email
                        }) ?? Enumerable.Empty<object>(),
                    Reports = job.Reports?
                        .Select(r => new
                        {
                            r.Id,
                            r.ReportNumber,
                            r.StartDate,
                            r.SubmitDate,
                            r.DistributeDate
                        }) ?? Enumerable.Empty<object>(),
                    SitePlans = job.SitePlans?
                        .Select(sp => new
                        {
                            sp.Id,
                            sp.SitePlanUrl
                        }) ?? Enumerable.Empty<object>(),
                    DirectProctors = job.LabTests?
                        .Where(lt => lt.Proctors != null)
                        .SelectMany(lt => lt.Proctors)
                        .Where(p => p.ProctorType != null)
                        .Select(p => new
                        {
                            p.Id,
                            p.ProctorID,
                            p.MaxDensity,
                            p.CorrectedDensity,
                            p.OptimumMoistureContent,
                            p.SpecificGravity,
                            ProctorType = new
                            {
                                p.ProctorType.Id,
                                p.ProctorType.Type
                            },
                            Source = "Direct",
                            LabTest = new
                            {
                                p.LabTest.Id,
                                p.LabTest.MaterialType,
                                p.LabTest.ReceiveDate
                            }
                        }) ?? Enumerable.Empty<object>(),
                    ReusedProctors = job.ProctorAdditionalJobs?
                        .Where(paj => paj.Proctor?.ProctorType != null)
                        .Select(paj => new
                        {
                            paj.Proctor.Id,
                            paj.Proctor.ProctorID,
                            paj.Proctor.MaxDensity,
                            paj.Proctor.CorrectedDensity,
                            paj.Proctor.OptimumMoistureContent,
                            paj.Proctor.SpecificGravity,
                            ProctorType = new
                            {
                                paj.Proctor.ProctorType.Id,
                                paj.Proctor.ProctorType.Type
                            },
                            Source = "Reused"
                        }) ?? Enumerable.Empty<object>()
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving job {JobId}", id);
                return StatusCode(500, "Internal server error occurred while retrieving the job");
            }
        }

        // POST: api/jobs
        [HttpPost]
        public async Task<ActionResult<Job>> CreateJob([FromBody] Job job)
        {
            try
            {
                _logger.LogInformation("Creating new job for client: {ClientName}", job.Client.Name);
                
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for job creation");
                    return BadRequest(ModelState);
                }

                _dbContext.Jobs.Add(job);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Successfully created job with ID: {JobId}", job.Id);
                return CreatedAtAction(nameof(GetJob), new { id = job.Id }, job);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating job");
                return StatusCode(500, "Internal server error occurred while creating the job");
            }
        }

        // PUT: api/jobs/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateJob(int id, [FromBody] Job job)
        {
            try
            {
                _logger.LogInformation("Updating job with ID: {JobId}", id);
                
                if (id != job.Id)
                {
                    _logger.LogWarning("ID mismatch: route ID {RouteId} != body ID {BodyId}", id, job.Id);
                    return BadRequest("ID mismatch between route and request body");
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for job update");
                    return BadRequest(ModelState);
                }

                var existingJob = await _dbContext.Jobs.FindAsync(id);
                if (existingJob == null)
                {
                    _logger.LogWarning("Job with ID {JobId} not found for update", id);
                    return NotFound($"Job with ID {id} not found");
                }

                // Update properties
                existingJob.ClientId = job.ClientId;
                existingJob.ProjectName = job.ProjectName;
                existingJob.SiteAddress = job.SiteAddress;
                existingJob.StartDate = job.StartDate;
                existingJob.EndDate = job.EndDate;

                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Successfully updated job with ID: {JobId}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating job {JobId}", id);
                return StatusCode(500, "Internal server error occurred while updating the job");
            }
        }

        // DELETE: api/jobs/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJob(int id)
        {
            try
            {
                _logger.LogInformation("Deleting job with ID: {JobId}", id);
                
                var job = await _dbContext.Jobs
                    .Include(j => j.Reports)
                    .Include(j => j.DistributionLists)
                    .FirstOrDefaultAsync(j => j.Id == id);
                    
                if (job == null)
                {
                    _logger.LogWarning("Job with ID {JobId} not found for deletion", id);
                    return NotFound($"Job with ID {id} not found");
                }

                // Check if job has associated data that would prevent deletion
                if (job.Reports.Any())
                {
                    _logger.LogWarning("Cannot delete job {JobId} - it has {ReportCount} associated reports", 
                        id, job.Reports.Count);
                    return BadRequest($"Cannot delete job - it has {job.Reports.Count} associated reports. Delete reports first.");
                }

                _dbContext.Jobs.Remove(job);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Successfully deleted job with ID: {JobId}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting job {JobId}", id);
                return StatusCode(500, "Internal server error occurred while deleting the job");
            }
        }

        // GET: api/jobs/{id}/distribution-lists
        [HttpGet("{id}/distribution-lists")]
        public async Task<ActionResult<IEnumerable<object>>> GetJobDistributionLists(int id)
        {
            try
            {
                _logger.LogInformation("Retrieving distribution lists for job ID: {JobId}", id);
                
                // First check if the job exists
                var jobExists = await _dbContext.Jobs.AnyAsync(j => j.Id == id);
                if (!jobExists)
                {
                    _logger.LogWarning("Job with ID {JobId} not found", id);
                    return NotFound($"Job with ID {id} not found");
                }

                var distributionLists = await _dbContext.DistributionLists
                    .Where(dl => dl.JobId == id)
                    .Include(dl => dl.DistributionMembers)
                        .ThenInclude(dm => dm.PersonalInfo)
                    .Select(dl => new
                    {
                        dl.Id,
                        dl.Name,
                        dl.Description,
                        MemberCount = dl.DistributionMembers.Count,
                        Members = dl.DistributionMembers.Select(dm => new
                        {
                            dm.PersonalInfo.Id,
                            dm.PersonalInfo.FirstName,
                            dm.PersonalInfo.LastName,
                            dm.PersonalInfo.Email
                        })
                    })
                    .ToListAsync();

                return Ok(distributionLists);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving distribution lists for job {JobId}", id);
                return StatusCode(500, "Internal server error occurred while retrieving distribution lists");
            }
        }

        // GET: api/jobs/{id}/proctors
        [HttpGet("{id}/proctors")]
        public async Task<ActionResult<object>> GetJobProctors(int id)
        {
            try
            {
                _logger.LogInformation("Retrieving proctors for job ID: {JobId}", id);
                
                // First check if the job exists
                var jobExists = await _dbContext.Jobs.AnyAsync(j => j.Id == id);
                if (!jobExists)
                {
                    _logger.LogWarning("Job with ID {JobId} not found", id);
                    return NotFound($"Job with ID {id} not found");
                }

                // Get direct proctors (belonging to this job through LabTest)
                var directProctors = await _dbContext.LabTests
                    .Where(lt => lt.JobId == id)
                    .SelectMany(lt => lt.Proctors)
                    .Include(p => p.ProctorType)
                    .Include(p => p.LabTest)
                    .Select(p => new
                    {
                        p.Id,
                        p.ProctorID,
                        p.MaxDensity,
                        p.CorrectedDensity,
                        p.OptimumMoistureContent,
                        p.SpecificGravity,
                        ProctorType = new
                        {
                            p.ProctorType.Id,
                            p.ProctorType.Type
                        },
                        Source = "Direct",
                        LabTest = new
                        {
                            p.LabTest.Id,
                            p.LabTest.MaterialType,
                            p.LabTest.ReceiveDate
                        }
                    })
                    .ToListAsync();

                // Get reused proctors (from other jobs)
                var reusedProctors = await _dbContext.ProctorAdditionalJobs
                    .Where(paj => paj.JobId == id)
                    .Include(paj => paj.Proctor)
                        .ThenInclude(p => p.ProctorType)
                    .Include(paj => paj.Proctor)
                        .ThenInclude(p => p.LabTest)
                    .Select(paj => new
                    {
                        paj.Proctor.Id,
                        paj.Proctor.ProctorID,
                        paj.Proctor.MaxDensity,
                        paj.Proctor.CorrectedDensity,
                        paj.Proctor.OptimumMoistureContent,
                        paj.Proctor.SpecificGravity,
                        ProctorType = new
                        {
                            paj.Proctor.ProctorType.Id,
                            paj.Proctor.ProctorType.Type
                        },
                        Source = "Reused",
                        OriginalJob = new
                        {
                            paj.Proctor.LabTest.JobId,
                            MaterialType = paj.Proctor.LabTest.MaterialType
                        }
                    })
                    .ToListAsync();

                var result = new
                {
                    DirectProctors = directProctors,
                    ReusedProctors = reusedProctors,
                    TotalCount = directProctors.Count + reusedProctors.Count
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving proctors for job {JobId}", id);
                return StatusCode(500, "Internal server error occurred while retrieving proctors");
            }
        }

        // GET: api/jobs/{id}/project-managers
        [HttpGet("{id}/project-managers")]
        public async Task<ActionResult<object>> GetJobProjectManagers(int id)
        {
            try
            {
                _logger.LogInformation("Retrieving project managers for job ID: {JobId}", id);
                
                // First check if the job exists
                var jobExists = await _dbContext.Jobs.AnyAsync(j => j.Id == id);
                if (!jobExists)
                {
                    _logger.LogWarning("Job with ID {JobId} not found", id);
                    return NotFound($"Job with ID {id} not found");
                }

                var projectManagers = await _dbContext.JobProjectManagers
                    .Where(jpm => jpm.JobId == id)
                    .Include(jpm => jpm.Employee)
                    .OrderByDescending(jpm => jpm.StartDate)
                    .Select(jpm => new
                    {
                        jpm.Id,
                        jpm.StartDate,
                        jpm.EndDate,
                        jpm.IsActive,
                        jpm.Notes,
                        jpm.CreatedDate,
                        jpm.LastModifiedDate,
                        Employee = new
                        {
                            jpm.Employee.Id,
                            FirstName = jpm.Employee.PersonalInfo.FirstName,
                            LastName = jpm.Employee.PersonalInfo.LastName,
                            Email = jpm.Employee.PersonalInfo.Email,
                            PhoneNumber = jpm.Employee.PersonalInfo.PhoneNumber
                        }
                    })
                    .ToListAsync();

                var result = new
                {
                    ActiveContacts = projectManagers.Where(pm => pm.IsActive && pm.EndDate == null),
                    ContactHistory = projectManagers,
                    TotalCount = projectManagers.Count
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving project managers for job {JobId}", id);
                return StatusCode(500, "Internal server error occurred while retrieving project managers");
            }
        }

        // POST: api/jobs/{id}/project-managers
        [HttpPost("{id}/project-managers")]
        public async Task<ActionResult<JobProjectManager>> AddProjectManager(int id, [FromBody] JobProjectManager projectManager)
        {
            try
            {
                _logger.LogInformation("Adding project manager to job ID: {JobId}", id);
                
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for project manager addition");
                    return BadRequest(ModelState);
                }

                // Check if the job exists
                var job = await _dbContext.Jobs.FindAsync(id);
                if (job == null)
                {
                    _logger.LogWarning("Job with ID {JobId} not found", id);
                    return NotFound($"Job with ID {id} not found");
                }

                // Check if the employee exists
                var employee = await _dbContext.GeoPacificEmployees.FindAsync(projectManager.EmployeeId);
                if (employee == null)
                {
                    _logger.LogWarning("Employee with ID {EmployeeId} not found", projectManager.EmployeeId);
                    return BadRequest($"Employee with ID {projectManager.EmployeeId} not found");
                }

                // Set the job ID
                projectManager.JobId = id;
                projectManager.CreatedDate = DateTime.UtcNow;

                _dbContext.JobProjectManagers.Add(projectManager);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Successfully added project manager {EmployeeId} to job {JobId}", 
                    projectManager.EmployeeId, id);
                return CreatedAtAction(nameof(GetJobProjectManagers), new { id }, projectManager);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding project manager to job {JobId}", id);
                return StatusCode(500, "Internal server error occurred while adding project manager");
            }
        }

        // PUT: api/jobs/{id}/project-managers/{projectManagerId}
        [HttpPut("{id}/project-managers/{projectManagerId}")]
        public async Task<IActionResult> UpdateProjectManager(int id, int projectManagerId, [FromBody] JobProjectManager projectManager)
        {
            try
            {
                _logger.LogInformation("Updating project manager {ProjectManagerId} for job {JobId}", projectManagerId, id);
                
                if (projectManagerId != projectManager.Id)
                {
                    return BadRequest("ID mismatch");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var existingProjectManager = await _dbContext.JobProjectManagers
                    .FirstOrDefaultAsync(jpm => jpm.Id == projectManagerId && jpm.JobId == id);
                
                if (existingProjectManager == null)
                {
                    return NotFound($"Project manager not found");
                }

                // Update properties
                existingProjectManager.StartDate = projectManager.StartDate;
                existingProjectManager.EndDate = projectManager.EndDate;
                existingProjectManager.IsActive = projectManager.IsActive;
                existingProjectManager.Notes = projectManager.Notes;
                existingProjectManager.LastModifiedDate = DateTime.UtcNow;

                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Successfully updated project manager {ProjectManagerId} for job {JobId}", 
                    projectManagerId, id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating project manager {ProjectManagerId} for job {JobId}", projectManagerId, id);
                return StatusCode(500, "Internal server error occurred while updating project manager");
            }
        }

        // GET: api/jobs/{id}/site-contacts
        [HttpGet("{id}/site-contacts")]
        public async Task<ActionResult<object>> GetJobSiteContacts(int id)
        {
            try
            {
                _logger.LogInformation("Retrieving site contacts for job ID: {JobId}", id);
                
                // First check if the job exists
                var jobExists = await _dbContext.Jobs.AnyAsync(j => j.Id == id);
                if (!jobExists)
                {
                    _logger.LogWarning("Job with ID {JobId} not found", id);
                    return NotFound($"Job with ID {id} not found");
                }

                var siteContacts = await _dbContext.JobSiteContacts
                    .Where(jsc => jsc.JobId == id)
                    .Include(jsc => jsc.PersonalInfo)
                    .OrderByDescending(jsc => jsc.StartDate)
                    .Select(jsc => new
                    {
                        jsc.Id,
                        jsc.Area,
                        jsc.Company,
                        jsc.Role,
                        jsc.IsPrimary,
                        jsc.StartDate,
                        jsc.EndDate,
                        jsc.IsActive,
                        jsc.Notes,
                        jsc.CreatedDate,
                        jsc.LastModifiedDate,
                        PersonalInfo = new
                        {
                            jsc.PersonalInfo.Id,
                            jsc.PersonalInfo.FirstName,
                            jsc.PersonalInfo.LastName,
                            jsc.PersonalInfo.Email,
                            jsc.PersonalInfo.PhoneNumber
                        }
                    })
                    .ToListAsync();

                var result = new
                {
                    PrimaryContact = siteContacts.FirstOrDefault(sc => sc.IsPrimary && sc.IsActive && sc.EndDate == null),
                    ActiveContacts = siteContacts.Where(sc => sc.IsActive && sc.EndDate == null),
                    ContactHistory = siteContacts,
                    TotalCount = siteContacts.Count
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving site contacts for job {JobId}", id);
                return StatusCode(500, "Internal server error occurred while retrieving site contacts");
            }
        }

        // POST: api/jobs/{id}/site-contacts
        [HttpPost("{id}/site-contacts")]
        public async Task<ActionResult<JobSiteContact>> AddSiteContact(int id, [FromBody] JobSiteContact siteContact)
        {
            try
            {
                _logger.LogInformation("Adding site contact to job ID: {JobId}", id);
                
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for site contact addition");
                    return BadRequest(ModelState);
                }

                // Check if the job exists
                var job = await _dbContext.Jobs.FindAsync(id);
                if (job == null)
                {
                    _logger.LogWarning("Job with ID {JobId} not found", id);
                    return NotFound($"Job with ID {id} not found");
                }

                // Check if the personal info exists
                var personalInfo = await _dbContext.PersonalInfos.FindAsync(siteContact.PersonalInfoId);
                if (personalInfo == null)
                {
                    _logger.LogWarning("Personal info with ID {PersonalInfoId} not found", siteContact.PersonalInfoId);
                    return BadRequest($"Personal info with ID {siteContact.PersonalInfoId} not found");
                }

                // Set the job ID
                siteContact.JobId = id;
                siteContact.CreatedDate = DateTime.UtcNow;

                // If this is a primary site contact, deactivate any existing primary
                if (siteContact.IsPrimary)
                {
                    var existingPrimary = await _dbContext.JobSiteContacts
                        .Where(jsc => jsc.JobId == id && jsc.IsPrimary && jsc.IsActive)
                        .FirstOrDefaultAsync();
                    
                    if (existingPrimary != null)
                    {
                        existingPrimary.IsPrimary = false;
                        existingPrimary.LastModifiedDate = DateTime.UtcNow;
                    }
                }

                _dbContext.JobSiteContacts.Add(siteContact);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Successfully added site contact {PersonalInfoId} to job {JobId}", 
                    siteContact.PersonalInfoId, id);
                return CreatedAtAction(nameof(GetJobSiteContacts), new { id }, siteContact);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding site contact to job {JobId}", id);
                return StatusCode(500, "Internal server error occurred while adding site contact");
            }
        }

        // PUT: api/jobs/{id}/site-contacts/{siteContactId}
        [HttpPut("{id}/site-contacts/{siteContactId}")]
        public async Task<IActionResult> UpdateSiteContact(int id, int siteContactId, [FromBody] JobSiteContact siteContact)
        {
            try
            {
                _logger.LogInformation("Updating site contact {SiteContactId} for job {JobId}", siteContactId, id);
                
                if (siteContactId != siteContact.Id)
                {
                    return BadRequest("ID mismatch");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var existingSiteContact = await _dbContext.JobSiteContacts
                    .FirstOrDefaultAsync(jsc => jsc.Id == siteContactId && jsc.JobId == id);
                
                if (existingSiteContact == null)
                {
                    return NotFound($"Site contact not found");
                }

                // If changing to primary, deactivate any existing primary
                if (siteContact.IsPrimary && !existingSiteContact.IsPrimary)
                {
                    var existingPrimary = await _dbContext.JobSiteContacts
                        .Where(jsc => jsc.JobId == id && jsc.IsPrimary && jsc.IsActive)
                        .FirstOrDefaultAsync();
                    
                    if (existingPrimary != null)
                    {
                        existingPrimary.IsPrimary = false;
                        existingPrimary.LastModifiedDate = DateTime.UtcNow;
                    }
                }

                // Update properties
                existingSiteContact.Area = siteContact.Area;
                existingSiteContact.Company = siteContact.Company;
                existingSiteContact.Role = siteContact.Role;
                existingSiteContact.IsPrimary = siteContact.IsPrimary;
                existingSiteContact.StartDate = siteContact.StartDate;
                existingSiteContact.EndDate = siteContact.EndDate;
                existingSiteContact.IsActive = siteContact.IsActive;
                existingSiteContact.Notes = siteContact.Notes;
                existingSiteContact.LastModifiedDate = DateTime.UtcNow;

                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Successfully updated site contact {SiteContactId} for job {JobId}", 
                    siteContactId, id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating site contact {SiteContactId} for job {JobId}", siteContactId, id);
                return StatusCode(500, "Internal server error occurred while updating site contact");
            }
        }

        // GET: api/jobs/{id}/summary
        [HttpGet("{id}/summary")]
        public async Task<ActionResult<object>> GetJobSummary(int id)
        {
            try
            {
                _logger.LogInformation("Retrieving summary for job ID: {JobId}", id);
                
                var job = await _dbContext.Jobs
                    .Where(j => j.Id == id)
                    .Select(j => new
                    {
                        j.Id,
                        ClientName = j.Client.Name,
                        j.ProjectName,
                        j.SiteAddress,
                        j.StartDate,
                        j.EndDate,
                        ReportCount = j.Reports.Count,
                        DistributionListCount = j.DistributionLists.Count,
                        ContractorCount = j.JobContracts.Count,
                        SitePlanCount = j.SitePlans.Count,
                        LabTestCount = j.LabTests.Count,
                        DirectProctorCount = j.LabTests.SelectMany(lt => lt.Proctors).Count(),
                        ReusedProctorCount = j.ProctorAdditionalJobs.Count,
                        TotalProctorCount = j.LabTests.SelectMany(lt => lt.Proctors).Count() + j.ProctorAdditionalJobs.Count
                    })
                    .FirstOrDefaultAsync();

                if (job == null)
                {
                    _logger.LogWarning("Job with ID {JobId} not found", id);
                    return NotFound($"Job with ID {id} not found");
                }

                return Ok(job);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving job summary {JobId}", id);
                return StatusCode(500, "Internal server error occurred while retrieving job summary");
            }
        }
    }
}
