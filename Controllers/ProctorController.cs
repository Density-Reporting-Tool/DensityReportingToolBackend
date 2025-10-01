using Microsoft.AspNetCore.Mvc;
using DensityReportingToolBackend.Services;
using DensityReportingToolBackend.Models;
using DensityReportingToolBackend.Models.DTOs;
using DensityReportingToolBackend.Data;
using DensityReportingToolBackend.Validators;

namespace DensityReportingToolBackend.Controllers
{
    [ApiController]
    [Route("api/proctors")]
    public class ProctorController : ControllerBase
    {
        private readonly ILogger<ProctorController> _logger;
        private readonly AppDbContext _dbContext;
        private readonly ProctorService _proctorService;

        public ProctorController(ILogger<ProctorController> logger, AppDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
            _proctorService = new ProctorService(dbContext);
        }

        /// <summary>
        /// Get all proctors for dashboard/schedule view
        /// </summary>
        /// <returns>List of all proctors with basic information</returns>
        [HttpGet]
        public async Task<ActionResult<object>> GetAllProctors()
        {
            try
            {
                _logger.LogInformation("Retrieving all proctors for dashboard");
                var proctors = await _proctorService.ListProctors();
                var result = proctors.Select(proctor => new
                {
                    Id = proctor.Id,
                    ProctorId = proctor.ProctorID,
                    JobNumber = proctor.LabTest.Job.JobNumber,
                    MaterialType = proctor.LabTest.MaterialType,
                    ProctorType = proctor.ProctorType.Type,
                    MaxDensity = proctor.MaxDensity,
                    DateTested = proctor.DateTested
                }).ToList();
                _logger.LogInformation("Successfully retrieved {ProctorCount} proctors", result.Count);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all proctors");
                return StatusCode(500, new
                {
                    message = "An error occurred while retrieving proctors"
                });
            }
        }

        /// <summary>
        /// Get proctor information by proctor ID
        /// </summary>
        /// <param name="proctorId">The proctor ID to search for</param>
        /// <returns>Proctor information including job, lab test details, and key relationships</returns>
        [HttpGet("{proctorId}")]
        public async Task<ActionResult<object>> GetProctor(int proctorId)
        {
            try
            {
                _logger.LogInformation("Retrieving proctor with ID: {ProctorId}", proctorId);

                var proctor = await _proctorService.GetProctorById(proctorId);

                if (proctor == null)
                {
                    _logger.LogWarning("Proctor with ID {ProctorId} not found", proctorId);
                    return NotFound(new
                    {
                        message = $"Proctor with ID {proctorId} not found",
                        proctorId = proctorId
                    });
                }

                var result = new
                {
                    Id = proctor.Id,
                    ProctorId = proctor.ProctorID,
                    JobNumber = proctor.LabTest.Job.JobNumber,
                    MaterialType = proctor.LabTest.MaterialType,
                    LabLocation = proctor.LabTest.ImportLocation,
                    ProctorType = proctor.ProctorType.Type,
                    MaxDensity = proctor.MaxDensity,
                    CorrectedDensity = proctor.CorrectedDensity,
                    OptimumMoisture = proctor.OptimumMoistureContent,
                    SpecificGravity = proctor.SpecificGravity,
                    OversizePercentage = proctor.OversizePercentage,
                    DateSampled = proctor.LabTest.ReceiveDate,
                    DateTested = proctor.DateTested,
                    DensityTestsCount = proctor.DensityTests.Count,
                    AdditionalJobsCount = proctor.AdditionalJobs.Count
                };
                _logger.LogInformation("Successfully retrieved proctor {ProctorId}", proctorId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving proctor {ProctorId}", proctorId);
                return StatusCode(500, new
                {
                    message = "An error occurred while retrieving the proctor",
                    proctorId = proctorId
                });
            }
        }

        /// <summary>
        /// Create a new proctor
        /// </summary>
        /// <param name="proctorDto">Proctor creation request with all proctor details</param>
        /// <returns>Created proctor information</returns>
        [HttpPost]
        public async Task<ActionResult<object>> CreateProctor([FromBody] ProctorCreateDto proctorDto)
        {
            try
            {
                _logger.LogInformation("Creating new proctor for job: {JobNumber}", proctorDto.JobNumber);

                // Validate required fields
                var validation = ProctorValidator.Validate(proctorDto);

                if (!validation.IsValid)
                {
                    return BadRequest(new
                    {
                        errors = validation.Errors
                    });
                }

                // Create new proctor
                var newProctor = await _proctorService.CreateProctor(proctorDto);

                _logger.LogInformation("Successfully created proctor with ID: {ProctorId} for job: {JobNumber}",
                    newProctor.Id, proctorDto.JobNumber);

                return CreatedAtAction(nameof(GetProctor), new { proctorId = newProctor.Id }, new
                {
                    Id = newProctor.Id,
                    ProctorId = newProctor.ProctorID,
                    JobNumber = proctorDto.JobNumber,
                    MaterialType = proctorDto.MaterialType,
                    ProctorType = proctorDto.ProctorType
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating proctor for job: {JobNumber}", proctorDto.JobNumber);
                return StatusCode(500, new
                {
                    message = "An error occurred while creating the proctor"
                });
            }
        }

        /// <summary>
        /// Update an existing proctor
        /// </summary>
        /// <param name="proctorId">The proctor ID to update</param>
        /// <param name="proctorDto">Proctor update request</param>
        /// <returns>Updated proctor information</returns>
        [HttpPut("{proctorId}")]
        public async Task<ActionResult<object>> UpdateProctor(int proctorId, [FromBody] ProctorUpdateDto proctorDto)
        {
            try
            {
                _logger.LogInformation("Updating proctor with ID: {ProctorId}", proctorId);

                var validation = ProctorValidator.Validate(proctorDto);
                if (!validation.IsValid)
                    return BadRequest(new { errors = validation.Errors });

                var updatedProctor = await _proctorService.UpdateProctor(proctorId, proctorDto);

                if (updatedProctor == null)
                    return NotFound(new { message = $"Proctor with ID {proctorId} not found" });

                _logger.LogInformation("Successfully updated proctor with ID: {ProctorId}", proctorId);
                return Ok(new
                {
                    Id = updatedProctor.Id,
                    ProctorId = updatedProctor.ProctorID,
                    JobNumber = updatedProctor.LabTest.Job.JobNumber,
                    MaterialType = updatedProctor.LabTest.MaterialType,
                    ProctorType = updatedProctor.ProctorType.Type,
                    MaxDensity = updatedProctor.MaxDensity,
                    DateTested = updatedProctor.DateTested
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating proctor with ID: {ProctorId}", proctorId);
                return StatusCode(500, new
                {
                    message = "An error occurred while updating the proctor"
                });
            }
        }

        /// <summary>
        /// Search proctors by job number
        /// </summary>
        /// <param name="jobNumber">Job number to search for</param>
        /// <param name="limit">Maximum number of results to return</param>
        /// <returns>List of matching proctors</returns>
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<object>>> SearchProctors([FromQuery] string jobNumber, [FromQuery] int limit = 10)
        {
            if (string.IsNullOrWhiteSpace(jobNumber))
                return BadRequest(new { message = "jobNumber query parameter is required" });

            var proctors = await _proctorService.SearchProctorsByJobNumber(jobNumber, limit);

            if (!proctors.Any())
                return NotFound(new { message = "No proctors found" });

            var proctorDtos = proctors.Select(p => new
            {
                Id = p.Id,
                ProctorId = p.ProctorID,
                JobNumber = p.LabTest.Job.JobNumber,
                MaterialType = p.LabTest.MaterialType,
                ProctorType = p.ProctorType.Type,
                MaxDensity = p.MaxDensity,
                DateTested = p.DateTested
            }).ToList();

            return Ok(proctorDtos);
        }

        #region Lab Admin Endpoints

        /// <summary>
        /// Create a new proctor (Lab Admin)
        /// POST /api/proctors/lab-admin
        /// </summary>
        [HttpPost("lab-admin")]
        public async Task<ActionResult<ProctorCreateResponse>> CreateProctor([FromBody] CreateProctorRequest request)
        {
            try
            {
                _logger.LogInformation("Lab Admin creating proctor for job: {JobNumber}", request.JobNumber);
                
                var response = await _proctorService.CreateProctorAsync(request);
                
                return CreatedAtAction(nameof(GetProctor), new { id = response.Id }, response);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid proctor creation request");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating proctor for job {JobNumber}", request.JobNumber);
                return StatusCode(500, new { message = "An error occurred while creating the proctor" });
            }
        }

        /// <summary>
        /// Update an existing proctor (Lab Admin)
        /// PUT /api/proctors/lab-admin/5
        /// </summary>
        [HttpPut("lab-admin/{id:int}")]
        public async Task<ActionResult<ProctorUpdateResponse>> UpdateProctor(int id, [FromBody] UpdateProctorRequest request)
        {
            try
            {
                _logger.LogInformation("Lab Admin updating proctor {ProctorId}", id);
                
                var response = await _proctorService.UpdateProctorAsync(id, request);
                
                if (response == null)
                {
                    return NotFound(new { message = $"Proctor with ID {id} not found" });
                }
                
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid proctor update request for ID {ProctorId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating proctor {ProctorId}", id);
                return StatusCode(500, new { message = "An error occurred while updating the proctor" });
            }
        }

        /// <summary>
        /// Get all proctors for lab admin management
        /// GET /api/proctors/lab-admin?page=1&limit=10&jobNumber=12345
        /// </summary>
        [HttpGet("lab-admin")]
        public async Task<ActionResult<ProctorListResponse>> GetProctorsForLabAdmin(
            [FromQuery] int page = 1, 
            [FromQuery] int limit = 50,
            [FromQuery] string? jobNumber = null)
        {
            try
            {
                _logger.LogInformation("Lab Admin getting proctors - Page: {Page}, Limit: {Limit}, Job: {JobNumber}", 
                    page, limit, jobNumber);
                
                var result = await _proctorService.GetProctorsForLabAdminAsync(page, limit, jobNumber);
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting proctors for lab admin");
                return StatusCode(500, new { message = "An error occurred while retrieving proctors" });
            }
        }

        #endregion

        #region Field Tech Endpoints

        /// <summary>
        /// Get density requirements for field testing
        /// GET /api/proctors/field-tech/5/density-requirements
        /// </summary>
        [HttpGet("field-tech/{id:int}/density-requirements")]
        public async Task<ActionResult<DensityRequirementsResponse>> GetDensityRequirements(int id)
        {
            try
            {
                _logger.LogInformation("Getting density requirements for proctor: {ProctorId}", id);
                
                var requirements = await _proctorService.GetDensityRequirementsAsync(id);
                
                if (requirements == null)
                {
                    return NotFound(new { message = $"Proctor with ID {id} not found" });
                }
                
                return Ok(requirements);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting density requirements for proctor {ProctorId}", id);
                return StatusCode(500, new { message = "An error occurred while retrieving density requirements" });
            }
        }

        #endregion

        #region Shared Endpoints

        // NOTE: Duplicate GetProctor method removed - using the one at line 65 instead

        /// <summary>
        /// Get all proctors for a specific job (shared by both lab admin and field tech)
        /// GET /api/proctors/job/12345
        /// </summary>
        [HttpGet("job/{jobNumber}")]
        public async Task<ActionResult<IEnumerable<ProctorDataResponse>>> GetProctorsForJob(string jobNumber)
        {
            try
            {
                _logger.LogInformation("Getting proctors for job: {JobNumber}", jobNumber);
                
                var proctors = await _proctorService.GetProctorsForJobAsync(jobNumber);
                
                return Ok(proctors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting proctors for job {JobNumber}", jobNumber);
                return StatusCode(500, new { message = "An error occurred while retrieving proctors" });
            }
        }

        #endregion
    }
}