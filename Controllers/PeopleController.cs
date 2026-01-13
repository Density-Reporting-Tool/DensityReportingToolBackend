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

        // Get all people (Employees and Contractors/Contacts)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PersonalInfoReadDto>>> GetAllPeople()
        {
            var result = await _peopleService.GetAllPeopleAsync();
            return Ok(result);
        }

        // Get a specific employee
        [HttpGet("employees/{id}")]
        public async Task<ActionResult<GeoPacificEmployeeReadDto>> GetEmployee(int id)
        {
            var employee = await _peopleService.GetEmployeeByIdAsync(id);
            
            if (employee == null)
                return NotFound();

            return Ok(employee);
        }

        // Create a new GeoPacific employee
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

        // Create a contractor (PersonalInfo with Company)
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


// Get a specific employee
// [HttpGet("employees/{id}")]
// public async Task<ActionResult<object>> GetEmployee(int id)
// {
//     var employee = await _dbContext.GeoPacificEmployees
//         .Include(e => e.PersonalInfo)
//         .Include(e => e.Role)
//         .FirstOrDefaultAsync(e => e.Id == id);

//     if (employee == null)
//         return NotFound();

//                     return new
//         {
//             employee.Id,
//             employee.PersonalInfo.FirstName,
//             employee.PersonalInfo.LastName,
//             employee.PersonalInfo.Email,
//             employee.PersonalInfo.PhoneNumber,
//             RoleId = employee.RoleId,
//             RoleTitle = employee.Role?.RoleTitle,
//             PersonType = "GeoPacific Employee"
//         };
// }



// // Get all people with their type
// [HttpGet]
// public async Task<ActionResult<IEnumerable<object>>> GetAllPeople()
// {
//     var people = await _dbContext.PersonalInfos
//         .Include(p => p.Employee)
//         .ThenInclude(e => e!.Role)
//         .Select(p => new
//         {
//             p.Id,
//             p.FirstName,
//             p.LastName,
//             p.Email,
//             p.PhoneNumber,
//             p.Company,
//             PersonType = p.Employee != null ? "GeoPacific Employee" : "Contact",
//             Role = p.Employee != null && p.Employee.Role != null ? p.Employee.Role.RoleTitle : null
//         })
//         .ToListAsync();

//     return Ok(people);
// }