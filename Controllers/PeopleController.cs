using DensityReportingToolBackend.DTOs.People;
using DensityReportingToolBackend.Services;
using Microsoft.AspNetCore.Mvc;

namespace DensityReportingToolBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PeopleController(IPeopleService peopleService, ILogger<PeopleController> logger) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PersonListFlatDto>>> GetAllPeople()
        {
            var result = await peopleService.GetAllPeopleAsync();
            return Ok(result);
        }

        [HttpGet("employees/{id}")]
        public async Task<ActionResult<GeoPacificEmployeeFlatDto>> GetEmployee(int id)
        {
            var employee = await peopleService.GetEmployeeByIdAsync(id);

            if (employee == null)
            {
                logger.LogWarning("Employee {Id} was requested but not found", id);
                return NotFound();
            }

            return Ok(employee);
        }

        [HttpPost("employees")]
        public async Task<ActionResult<GeoPacificEmployeeReadDto>> CreateEmployee([FromBody] GeoPacificEmployeeCreateDto dto)
        {
            var result = await peopleService.CreateEmployeeAsync(dto);
            return CreatedAtAction(nameof(GetEmployee), new { id = result.Id }, result);
        }

        [HttpPost("contractors")]
        public async Task<ActionResult<PersonalInfoReadDto>> CreateContractor([FromBody] PersonalInfoCreateDto dto)
        {
            var result = await peopleService.CreateContractorAsync(dto);
            return CreatedAtAction(nameof(GetAllPeople), new { id = result.Id }, result);
        }
    }
}
