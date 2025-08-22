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

        // Create a new contractor
        [HttpPost("contractors")]
        public async Task<ActionResult<Contractor>> CreateContractor([FromBody] CreateContractorRequest request)
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

                // Create Contractor
                var contractor = new Contractor
                {
                    PersonalInfoId = personalInfo.Id,
                    CompanyName = request.CompanyName,
                    ClientId = request.ClientId // Optional: link to existing client
                };

                _dbContext.Contractors.Add(contractor);
                await _dbContext.SaveChangesAsync();

                // Return the created contractor with all related data
                var result = await _dbContext.Contractors
                    .Include(c => c.PersonalInfo)
                    .Include(c => c.Client)
                    .FirstOrDefaultAsync(c => c.Id == contractor.Id);

                return CreatedAtAction(nameof(GetContractor), new { id = contractor.Id }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating contractor");
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

        // Get a specific contractor
        [HttpGet("contractors/{id}")]
        public async Task<ActionResult<object>> GetContractor(int id)
        {
            var contractor = await _dbContext.Contractors
                .Include(c => c.PersonalInfo)
                .Include(c => c.Client)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (contractor == null)
                return NotFound();

                            return new
                {
                    contractor.Id,
                    contractor.PersonalInfo.FirstName,
                    contractor.PersonalInfo.LastName,
                    contractor.PersonalInfo.Email,
                    contractor.PersonalInfo.PhoneNumber,
                    contractor.CompanyName,
                    ClientName = contractor.Client != null ? contractor.Client.Name : null,
                    PersonType = "Contractor"
                };
        }

        // Get all people with their type
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetAllPeople()
        {
            var people = await _dbContext.PersonalInfos
                .Include(p => p.Employee)
                .ThenInclude(e => e.Role)
                .Include(p => p.Contractor)
                .ThenInclude(c => c.Client)
                .Select(p => new
                {
                    p.Id,
                    p.FirstName,
                    p.LastName,
                    p.Email,
                    p.PhoneNumber,
                    PersonType = p.Employee != null ? "GeoPacific Employee" : 
                                p.Contractor != null ? "Contractor" : "Unknown",
                    CompanyName = p.Contractor != null ? p.Contractor.CompanyName : null,
                    Role = p.Employee != null && p.Employee.Role != null ? p.Employee.Role.RoleTitle : null,
                    ClientName = p.Contractor != null && p.Contractor.Client != null ? p.Contractor.Client.Name : null
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

    public class CreateContractorRequest
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
        public required string CompanyName { get; set; }
        public int? ClientId { get; set; } // Optional: link to existing client
    }
}
