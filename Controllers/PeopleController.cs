using DensityReportingToolBackend.DTOs.People;
using DensityReportingToolBackend.Infrastructure;
using DensityReportingToolBackend.Infrastructure.Common;
using DensityReportingToolBackend.Services;
using Microsoft.AspNetCore.Mvc;

namespace DensityReportingToolBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PeopleController(IPeopleService peopleService, ILogger<PeopleController> logger) : BaseApiController
    {
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<PersonListFlatDto>>>> GetAllPeople(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await peopleService.GetAllPeopleAsync(pageNumber, pageSize);
            return Success(result, "People retrieved successfully");
        }

        [HttpGet("employees/{id}")]
        public async Task<ActionResult<ApiResponse<GeoPacificEmployeeFlatDto>>> GetEmployee(int id)
        {
            var employee = await peopleService.GetEmployeeByIdAsync(id);

            if (employee == null)
            {
                logger.LogWarning("Employee {Id} was requested but not found", id);
                return Failure<GeoPacificEmployeeFlatDto>($"Employee {id} not found", 404);
            }

            return Success(employee);
        }

        [HttpGet("contractors/{id}")]
        public async Task<ActionResult<ApiResponse<PersonalInfoReadDto>>> GetContractor(int id)
        {
            var contractor = await peopleService.GetContractorByIdAsync(id);

            if (contractor == null)
            {
                logger.LogWarning("Contractor {Id} was requested but not found", id);
                return Failure<PersonalInfoReadDto>($"Contractor {id} not found", 404);
            }

            return Success(contractor);
        }

        [HttpPost("employees")]
        public async Task<ActionResult<ApiResponse<GeoPacificEmployeeReadDto>>> CreateEmployee([FromBody] GeoPacificEmployeeCreateDto dto)
        {
            var result = await peopleService.CreateEmployeeAsync(dto);
            return Created(nameof(GetEmployee), new { id = result.Id }, result);
        }

        [HttpPost("contractors")]
        public async Task<ActionResult<ApiResponse<PersonalInfoReadDto>>> CreateContractor([FromBody] PersonalInfoCreateDto dto)
        {
            var result = await peopleService.CreateContractorAsync(dto);
            return Created(nameof(GetContractor), new { id = result.Id }, result);
        }

        [HttpPut("employees/{id}")]
        public async Task<ActionResult<ApiResponse<GeoPacificEmployeeReadDto>>> UpdateEmployee(int id, [FromBody] GeoPacificEmployeeUpdateDto dto)
        {
            var result = await peopleService.UpdateEmployeeAsync(id, dto);

            if (result == null)
                return Failure<GeoPacificEmployeeReadDto>($"Employee {id} not found", 404);

            return Success(result, "Employee updated successfully");
        }

        [HttpPut("contractors/{id}")]
        public async Task<ActionResult<ApiResponse<PersonalInfoReadDto>>> UpdateContractor(int id, [FromBody] PersonalInfoUpdateDto dto)
        {
            var result = await peopleService.UpdateContractorAsync(id, dto);

            if (result == null)
                return Failure<PersonalInfoReadDto>($"Contractor {id} not found", 404);

            return Success(result, "Contractor updated successfully");
        }
    }
}
