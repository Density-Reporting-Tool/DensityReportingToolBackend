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

        // Get all people with their type
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetAllPeople()
        {
            var people = await _dbContext.PersonalInfos
                .Include(p => p.Employee)
                .ThenInclude(e => e!.Role)
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

        // Create a new contact (PersonalInfo with optional Company)
        // This endpoint handles all contacts: regular contacts, contractors, and GeoPacific employees
        [HttpPost]
        public async Task<ActionResult<object>> CreateContact([FromBody] CreateContactRequest request)
        {
            try
            {
                var personalInfo = new PersonalInfo
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber,
                    Company = request.Company // Optional - can be null
                };

                _dbContext.PersonalInfos.Add(personalInfo);
                await _dbContext.SaveChangesAsync();

                // Return the created contact in the format expected by the frontend
                return CreatedAtAction(nameof(GetAllPeople), new { id = personalInfo.Id }, new
                {
                    id = personalInfo.Id,
                    firstName = personalInfo.FirstName,
                    lastName = personalInfo.LastName,
                    email = personalInfo.Email,
                    phoneNumber = personalInfo.PhoneNumber,
                    company = personalInfo.Company,
                    personType = "Contact"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating contact");
                return StatusCode(500, "Internal server error");
            }
        }

    }

    // Request model
    public class CreateContactRequest
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
        public string? Company { get; set; }
    }

}
