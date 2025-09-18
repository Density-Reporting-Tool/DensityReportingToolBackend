using Microsoft.AspNetCore.Mvc;
using DensityReportingToolBackend.Services;
using DensityReportingToolBackend.Models.DTOs;

namespace DensityReportingToolBackend.Controllers
{
    [ApiController]
    [Route("api/proctors")]
    public class ProctorController : ControllerBase
    {
        private readonly IProctorService _proctorService;
        private readonly ILogger<ProctorController> _logger;

        public ProctorController(IProctorService proctorService, ILogger<ProctorController> logger)
        {
            _proctorService = proctorService;
            _logger = logger;
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

        /// <summary>
        /// Get a specific proctor by ID (used by both roles)
        /// GET /api/proctors/5
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProctorDataResponse>> GetProctor(int id)
        {
            try
            {
                _logger.LogInformation("Getting proctor with ID: {ProctorId}", id);
                
                var proctor = await _proctorService.GetProctorAsync(id);
                
                if (proctor == null)
                {
                    return NotFound(new { message = $"Proctor with ID {id} not found" });
                }
                
                return Ok(proctor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting proctor {ProctorId}", id);
                return StatusCode(500, new { message = "An error occurred while retrieving the proctor" });
            }
        }

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