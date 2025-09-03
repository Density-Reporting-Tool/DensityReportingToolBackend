using DensityReportingToolBackend.Data;
using DensityReportingToolBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DensityReportingToolBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PeopleController : ControllerBase
    {
        private readonly ILogger<PeopleController> _logger;
        private readonly AppDbContext _dbContext;

        public PeopleController(ILogger<PeopleController> logger, AppDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        // Create a new GeoPacific employee
        [HttpPost("employees")]
        public async Task<ActionResult<GeoPacificEmployee>> CreateEmployee([FromBody] CreateEmployeeRequest request)
        {
            try
            {
                // Create PersonalInfo first
                var personalInfo = new PersonalInfo
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber
                };

                _dbContext.PersonalInfos.Add(personalInfo);
                await _dbContext.SaveChangesAsync();

                // Create GeoPacificEmployee
                var employee = new GeoPacificEmployee
                {
                    PersonalInfoId = personalInfo.Id,
                    RoleId = request.RoleId,
                    Password = request.Password
                };

                _dbContext.GeoPacificEmployees.Add(employee);
                await _dbContext.SaveChangesAsync();

                // Return the created employee with all related data
                var result = await _dbContext.GeoPacificEmployees
                    .Include(e => e.PersonalInfo)
                    .Include(e => e.Role)
                    .FirstOrDefaultAsync(e => e.Id == employee.Id);

                return CreatedAtAction(nameof(GetEmployee), new { id = employee.Id }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating employee");
                return StatusCode(500, "Internal server error");
            }
        }



        // Get a specific employee
        [HttpGet("employees/{id}")]
        public async Task<ActionResult<object>> GetEmployee(int id)
        {
            var employee = await _dbContext.GeoPacificEmployees
                .Include(e => e.PersonalInfo)
                .Include(e => e.Role)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null)
                return NotFound();

                            return new
                {
                    employee.Id,
                    employee.PersonalInfo.FirstName,
                    employee.PersonalInfo.LastName,
                    employee.PersonalInfo.Email,
                    employee.PersonalInfo.PhoneNumber,
                    RoleId = employee.RoleId,
                    RoleTitle = employee.Role?.RoleTitle,
                    PersonType = "GeoPacific Employee"
                };
        }



        // Get all people with their type
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetAllPeople()
        {
            var people = await _dbContext.PersonalInfos
                .Include(p => p.Employee)
                .ThenInclude(e => e.Role)
                .Select(p => new
                {
                    p.Id,
                    p.FirstName,
                    p.LastName,
                    p.Email,
                    p.PhoneNumber,
                    p.Company,
                    PersonType = p.Employee != null ? "GeoPacific Employee" : "Contact",
                    Role = p.Employee != null && p.Employee.Role != null ? p.Employee.Role.RoleTitle : null
                })
                .ToListAsync();

            return Ok(people);
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


}
