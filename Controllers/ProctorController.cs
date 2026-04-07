using DensityReportingToolBackend.DTOs.Labs;
using DensityReportingToolBackend.Infrastructure;
using DensityReportingToolBackend.Infrastructure.Common;
using DensityReportingToolBackend.Services;
using Microsoft.AspNetCore.Mvc;

namespace DensityReportingToolBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProctorController(IProctorService proctorService, ILogger<ProctorController> logger) : BaseApiController
    {
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<ProctorReadDto>>>> GetAllProctors(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            logger.LogInformation("Retrieving paged proctors: Page {PageNumber}, Size {PageSize}", pageNumber, pageSize);
            var result = await proctorService.ListProctorsAsync(pageNumber, pageSize);
            return Success(result, "Proctors retrieved successfully");
        }

        [HttpGet("{proctorId:int}")]
        public async Task<ActionResult<ApiResponse<ProctorReadDto>>> GetProctor(int proctorId)
        {
            var proctor = await proctorService.GetProctorByIdAsync(proctorId);
            if (proctor == null)
                return Failure<ProctorReadDto>($"Proctor with ID {proctorId} not found", 404);
            return Success(proctor);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<ProctorReadDto>>> CreateProctor([FromBody] ProctorCreateDto dto)
        {
            var result = await proctorService.CreateProctorAsync(dto);
            return Created(nameof(GetProctor), new { proctorId = result.Id }, result);
        }

        [HttpPut("{proctorId:int}")]
        public async Task<ActionResult<ApiResponse<ProctorReadDto>>> UpdateProctor(int proctorId, [FromBody] ProctorUpdateDto dto)
        {
            var result = await proctorService.UpdateProctorAsync(proctorId, dto);
            return Success(result, "Proctor updated successfully");
        }

        [HttpGet("search/{jobNumber}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ProctorReadDto>>>> SearchProctors(
            string jobNumber,
            [FromQuery] int limit = 10)
        {
            if (string.IsNullOrWhiteSpace(jobNumber))
                return Failure<IEnumerable<ProctorReadDto>>("Job number search term is required", 400);

            var proctors = await proctorService.SearchProctorsByJobNumberAsync(jobNumber, limit);
            return Success(proctors, $"Found proctors matching '{jobNumber}'");
        }

        [HttpGet("job/{jobNumber}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ProctorReadDto>>>> GetProctorsForJob(string jobNumber)
        {
            var proctors = await proctorService.GetProctorsForJobAsync(jobNumber);
            return Success(proctors);
        }

        [HttpGet("job-id/{jobId:int}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ProctorReadDto>>>> GetProctorsForJobById(int jobId)
        {
            var proctors = await proctorService.GetProctorsForJobByIdAsync(jobId);
            return Success(proctors);
        }
    }
}
