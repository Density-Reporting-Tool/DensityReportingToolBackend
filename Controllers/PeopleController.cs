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
            logger.LogInformation("Retrieving paged people list: Page {PageNumber}, Size {PageSize}", pageNumber, pageSize);

            var result = await peopleService.GetAllPeopleAsync(pageNumber, pageSize);
            return Success(result, "People retrieved successfully");
        }

        [HttpGet("search/{query}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<PersonListFlatDto>>>> SearchPeople(
            string query,
            [FromQuery] int limit = 10)
        {
            if (string.IsNullOrWhiteSpace(query))
                return Failure<IEnumerable<PersonListFlatDto>>("Search query is required", 400);

            var people = await peopleService.SearchPeopleAsync(query, limit);

            if (people == null || !people.Any())
                return Success(Enumerable.Empty<PersonListFlatDto>(), "No people matched your search criteria");

            return Success(people, $"Found {people.Count()} people matching '{query}'");
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

            return Success(employee, "Employee retrieved successfully");
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

            return Success(contractor, "Contractor retrieved successfully");
        }

        [HttpPost("employees")]
        public async Task<ActionResult<ApiResponse<GeoPacificEmployeeFlatDto>>> CreateEmployee([FromBody] GeoPacificEmployeeCreateDto dto)
        {
            logger.LogInformation("Creating new employee: {Email}", dto.Email);

            var result = await peopleService.CreateEmployeeAsync(dto);
            return Created(nameof(GetEmployee), new { id = result.Id }, result);
        }

        [HttpPost("contractors")]
        public async Task<ActionResult<ApiResponse<PersonalInfoReadDto>>> CreateContractor([FromBody] PersonalInfoCreateDto dto)
        {
            logger.LogInformation("Creating new contractor: {Email}", dto.Email);

            var result = await peopleService.CreateContractorAsync(dto);
            return Created(nameof(GetContractor), new { id = result.Id }, result);
        }

        [HttpPut("employees/{id}")]
        public async Task<ActionResult<ApiResponse<GeoPacificEmployeeFlatDto>>> UpdateEmployee(int id, [FromBody] GeoPacificEmployeeUpdateDto dto)
        {
            var result = await peopleService.UpdateEmployeeAsync(id, dto);

            if (result == null)
                return Failure<GeoPacificEmployeeFlatDto>($"Employee {id} not found", 404);

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
