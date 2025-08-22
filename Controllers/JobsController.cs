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

        // GET: api/jobs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetJobs()
        {
            try
            {
                _logger.LogInformation("Retrieving all jobs");
                
                var jobs = await _dbContext.Jobs
                    .Include(j => j.ProjectManager)
                    .Include(j => j.DistributionLists)
                    .Include(j => j.JobContracts)
                        .ThenInclude(jc => jc.Contractor)
                            .ThenInclude(c => c.Details)
                    .Include(j => j.LabTests)
                        .ThenInclude(lt => lt.Proctors)
                            .ThenInclude(p => p.ProctorType)
                    .Include(j => j.ProctorAdditionalJobs)
                        .ThenInclude(paj => paj.Proctor)
                            .ThenInclude(p => p.ProctorType)
                    .Select(j => new
                    {
                        j.Id,
                        j.ClientName,
                        j.ProjectName,
                        j.SiteAddress,
                        j.StartDate,
                        j.EndDate,
                        ProjectManager = new
                        {
                            j.ProjectManager.Id,
                            j.ProjectManager.FirstName,
                            j.ProjectManager.LastName,
                            j.ProjectManager.Email
                        },
                        DistributionLists = j.DistributionLists.Select(dl => new
                        {
                            dl.Id,
                            dl.Name,
                            dl.Description,
                            MemberCount = dl.DistributionMembers.Count
                        }),
                        Contractors = j.JobContracts.Select(jc => new
                        {
                            jc.Contractor.Id,
                            jc.Contractor.Details.FirstName,
                            jc.Contractor.Details.LastName,
                            jc.Contractor.Details.Email
                        }),
                        DirectProctors = j.LabTests.SelectMany(lt => lt.Proctors).Select(p => new
                        {
                            p.Id,
                            p.ProctorID,
                            p.MaxDensity,
                            p.OptimumMoistureContent,
                            ProctorType = new
                            {
                                p.ProctorType.Id,
                                p.ProctorType.Type
                            },
                            Source = "Direct" // Belongs directly to this job
                        }),
                        ReusedProctors = j.ProctorAdditionalJobs.Select(paj => new
                        {
                            paj.Proctor.Id,
                            paj.Proctor.ProctorID,
                            paj.Proctor.MaxDensity,
                            paj.Proctor.OptimumMoistureContent,
                            ProctorType = new
                            {
                                paj.Proctor.ProctorType.Id,
                                paj.Proctor.ProctorType.Type
                            },
                            Source = "Reused" // From another job
                        })
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
                    .Include(j => j.ProjectManager)
                    .Include(j => j.DistributionLists)
                        .ThenInclude(dl => dl.DistributionMembers)
                            .ThenInclude(dm => dm.PersonalInfo)
                    .Include(j => j.JobContracts)
                        .ThenInclude(jc => jc.Contractor)
                            .ThenInclude(c => c.Details)
                    .Include(j => j.LabTests)
                        .ThenInclude(lt => lt.Proctors)
                            .ThenInclude(p => p.ProctorType)
                    .Include(j => j.ProctorAdditionalJobs)
                        .ThenInclude(paj => paj.Proctor)
                            .ThenInclude(p => p.ProctorType)
                    .Include(j => j.Reports)
                    .Include(j => j.SitePlans)
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
                    job.ClientName,
                    job.ProjectName,
                    job.SiteAddress,
                    job.StartDate,
                    job.EndDate,
                    ProjectManager = new
                    {
                        job.ProjectManager.Id,
                        job.ProjectManager.FirstName,
                        job.ProjectManager.LastName,
                        job.ProjectManager.Email
                    },
                    DistributionLists = job.DistributionLists.Select(dl => new
                    {
                        dl.Id,
                        dl.Name,
                        dl.Description,
                        Members = dl.DistributionMembers.Select(dm => new
                        {
                            dm.PersonalInfo.Id,
                            dm.PersonalInfo.FirstName,
                            dm.PersonalInfo.LastName,
                            dm.PersonalInfo.Email
                        })
                    }),
                    Contractors = job.JobContracts.Select(jc => new
                    {
                        jc.Contractor.Id,
                        jc.Contractor.Details.FirstName,
                        jc.Contractor.Details.LastName,
                        jc.Contractor.Details.Email
                    }),
                    Reports = job.Reports.Select(r => new
                    {
                        r.Id,
                        r.ReportNumber,
                        r.StartDate,
                        r.SubmitDate,
                        r.DistributeDate
                    }),
                    SitePlans = job.SitePlans.Select(sp => new
                    {
                        sp.Id,
                        sp.SitePlanUrl
                    }),
                    DirectProctors = job.LabTests.SelectMany(lt => lt.Proctors).Select(p => new
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
                    }),
                    ReusedProctors = job.ProctorAdditionalJobs.Select(paj => new
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
                    })
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
                _logger.LogInformation("Creating new job for client: {ClientName}", job.ClientName);
                
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for job creation");
                    return BadRequest(ModelState);
                }

                // Validate that the project manager exists
                var projectManager = await _dbContext.GeoPacificEmployees
                    .FirstOrDefaultAsync(e => e.Id == job.ProjectManagerId);
                
                if (projectManager == null)
                {
                    _logger.LogWarning("Project manager with ID {ProjectManagerId} not found", job.ProjectManagerId);
                    return BadRequest($"Project manager with ID {job.ProjectManagerId} not found");
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

                // Validate that the project manager exists
                var projectManager = await _dbContext.GeoPacificEmployees
                    .FirstOrDefaultAsync(e => e.Id == job.ProjectManagerId);
                
                if (projectManager == null)
                {
                    _logger.LogWarning("Project manager with ID {ProjectManagerId} not found", job.ProjectManagerId);
                    return BadRequest($"Project manager with ID {job.ProjectManagerId} not found");
                }

                // Update properties
                existingJob.ClientName = job.ClientName;
                existingJob.ProjectName = job.ProjectName;
                existingJob.SiteAddress = job.SiteAddress;
                existingJob.StartDate = job.StartDate;
                existingJob.EndDate = job.EndDate;
                existingJob.ProjectManagerId = job.ProjectManagerId;

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
                        j.ClientName,
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
