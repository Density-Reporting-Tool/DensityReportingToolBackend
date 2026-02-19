using DensityReportingToolBackend.Data;
using DensityReportingToolBackend.DTOs.People;
using DensityReportingToolBackend.Models;
using DensityReportingToolBackend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DensityReportingToolBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PeopleController(IPeopleService peopleService, ILogger<PeopleController> logger) : ControllerBase
    {
        private readonly IPeopleService _peopleService = peopleService;
        private readonly ILogger<PeopleController> _logger = logger;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PersonListFlatDto>>> GetAllPeople()
        {
            var result = await _peopleService.GetAllPeopleAsync();
            return Ok(result);
        }

        [HttpGet("employees/{id}")]
        public async Task<ActionResult<GeoPacificEmployeeReadDto>> GetEmployee(int id)
        {
            var employee = await _peopleService.GetEmployeeByIdAsync(id);
            
            if (employee == null)
                return NotFound();

            return Ok(employee);
        }

        [HttpPost("employees")]
        public async Task<ActionResult<GeoPacificEmployeeReadDto>> CreateEmployee([FromBody] CreateEmployeeRequest request)
        {
            try
            {
                var result = await _peopleService.CreateEmployeeAsync(request);
                
                // result.Id comes from the GeoPacificEmployee record
                return CreatedAtAction(nameof(GetEmployee), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating employee");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("contractors")]
        public async Task<ActionResult<PersonalInfoReadDto>> CreateContractor([FromBody] CreateContractorRequest request)
        {
            try
            {
                var result = await _peopleService.CreateContractorAsync(request);
                

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating contractor");
                return StatusCode(500, "Internal server error");
            }
        }
    }

    // Request models
    public class CreateEmployeeRequest
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
        public required int RoleId { get; set; }
        public required string Password { get; set; }
    }

    public class CreateContractorRequest
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Company { get; set; }
    }
}