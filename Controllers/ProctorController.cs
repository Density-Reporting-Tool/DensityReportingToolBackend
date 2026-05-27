using DensityReportingToolBackend.Infrastructure;
using DensityReportingToolBackend.Models;
using DensityReportingToolBackend.Models.DTOs;
using DensityReportingToolBackend.Services;
using DensityReportingToolBackend.Validators;
using Microsoft.AspNetCore.Mvc;

namespace DensityReportingToolBackend.Controllers
{
    [ApiController]
    [Route("api/proctors")]
    public class ProctorController(IProctorService proctorService, ILogger<ProctorController> logger) : BaseApiController
    {
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<ProctorDataResponse>>>> GetAllProctors()
        {
            logger.LogInformation("Retrieving all proctors");
            var proctors = await proctorService.GetAllProctorsAsync();
            return Success(proctors, "Proctors retrieved successfully");
        }

        [HttpGet("{proctorId:int}")]
        public async Task<ActionResult<ApiResponse<ProctorDataResponse>>> GetProctor(int proctorId)
        {
            var proctor = await proctorService.GetProctorAsync(proctorId);

            if (proctor is null)
            {
                logger.LogWarning("Proctor {ProctorId} was requested but not found", proctorId);
                return Failure<ProctorDataResponse>($"Proctor with ID {proctorId} not found", 404);
            }

            return Success(proctor);
        }

        [HttpGet("search")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ProctorDataResponse>>>> SearchProctors(
            [FromQuery] string jobNumber,
            [FromQuery] int limit = 10)
        {
            if (string.IsNullOrWhiteSpace(jobNumber))
                return Failure<IEnumerable<ProctorDataResponse>>("jobNumber query parameter is required");

            var proctors = await proctorService.SearchProctorsAsync(jobNumber, limit);
            return Success(proctors, $"Found {proctors.Count()} proctors matching '{jobNumber}'");
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<ProctorDataResponse>>> CreateProctor([FromBody] ProctorCreateDto dto)
        {
            var validation = ProctorValidator.Validate(dto);
            if (!validation.IsValid)
            {
                var errors = string.Join("; ", validation.Errors.SelectMany(kv => kv.Value));
                return Failure<ProctorDataResponse>(errors);
            }

            var proctor = await proctorService.CreateProctorAsync(dto);
            return Created(nameof(GetProctor), new { proctorId = proctor.Id }, proctor);
        }

        [HttpPut("{proctorId:int}")]
        public async Task<ActionResult<ApiResponse<ProctorDataResponse>>> UpdateProctor(
            int proctorId,
            [FromBody] ProctorUpdateDto dto)
        {
            var validation = ProctorValidator.Validate(dto);
            if (!validation.IsValid)
            {
                var errors = string.Join("; ", validation.Errors.SelectMany(kv => kv.Value));
                return Failure<ProctorDataResponse>(errors);
            }

            var proctor = await proctorService.UpdateProctorAsync(proctorId, dto);

            if (proctor is null)
                return Failure<ProctorDataResponse>($"Proctor with ID {proctorId} not found", 404);

            return Success(proctor, "Proctor updated successfully");
        }

        #region Lab Admin Endpoints

        [HttpGet("lab-admin")]
        public async Task<ActionResult<ApiResponse<ProctorListResponse>>> GetProctorsForLabAdmin(
            [FromQuery] int page = 1,
            [FromQuery] int limit = 50,
            [FromQuery] string? jobNumber = null)
        {
            logger.LogInformation("Lab Admin getting proctors - Page: {Page}, Limit: {Limit}, Job: {JobNumber}",
                page, limit, jobNumber);

            var result = await proctorService.GetProctorsForLabAdminAsync(page, limit, jobNumber);
            return Success(result, "Proctors retrieved successfully");
        }

        [HttpPost("lab-admin")]
        public async Task<ActionResult<ApiResponse<ProctorDataResponse>>> CreateProctorLabAdmin(
            [FromBody] CreateProctorRequest request)
        {
            logger.LogInformation("Lab Admin creating proctor for job: {JobNumber}", request.JobNumber);

            var response = await proctorService.CreateProctorAsync(request);
            return Created(nameof(GetProctor), new { proctorId = response.Proctor.Id }, response.Proctor);
        }

        [HttpPut("lab-admin/{id:int}")]
        public async Task<ActionResult<ApiResponse<ProctorDataResponse>>> UpdateProctorLabAdmin(
            int id,
            [FromBody] UpdateProctorRequest request)
        {
            logger.LogInformation("Lab Admin updating proctor {ProctorId}", id);

            var response = await proctorService.UpdateProctorAsync(id, request);

            if (response is null)
                return Failure<ProctorDataResponse>($"Proctor with ID {id} not found", 404);

            return Success(response.Proctor, "Proctor updated successfully");
        }

        #endregion

        #region Field Tech Endpoints

        [HttpGet("field-tech/{id:int}/density-requirements")]
        public async Task<ActionResult<ApiResponse<DensityRequirementsResponse>>> GetDensityRequirements(int id)
        {
            var requirements = await proctorService.GetDensityRequirementsAsync(id);

            if (requirements is null)
                return Failure<DensityRequirementsResponse>($"Proctor with ID {id} not found", 404);

            return Success(requirements);
        }

        #endregion

        #region Shared Endpoints

        [HttpGet("job/{jobNumber}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ProctorDataResponse>>>> GetProctorsForJob(string jobNumber)
        {
            var proctors = await proctorService.GetProctorsForJobAsync(jobNumber);
            return Success(proctors, $"Proctors for job {jobNumber} retrieved successfully");
        }

        [HttpGet("job-id/{jobId:int}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ProctorDataResponse>>>> GetProctorsForJobById(int jobId)
        {
            var proctors = await proctorService.GetProctorsForJobByIdAsync(jobId);
            return Success(proctors, $"Proctors for job ID {jobId} retrieved successfully");
        }

        #endregion
    }
}
