using DensityReportingToolBackend.Data;
using DensityReportingToolBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DensityReportingToolBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly ILogger<TestController> _logger;
        private readonly AppDbContext _dbContext;

        public TestController(ILogger<TestController> logger, AppDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        [HttpGet("health")]
        public IActionResult HealthCheck()
        {
            return Ok(new { 
                status = "healthy", 
                timestamp = DateTime.UtcNow,
                message = "Backend is running successfully!"
            });
        }

        [HttpGet("employees")]
        public async Task<IEnumerable<object>> GetEmployees()
        {
            // Return all GeoPacific employees with their personal info and role
            var employees = await _dbContext.GeoPacificEmployees
                .Include(e => e.PersonalInfo)
                .Include(e => e.Role)
                .Select(e => new
                {
                    e.Id,
                    e.PersonalInfo.FirstName,
                    e.PersonalInfo.LastName,
                    e.PersonalInfo.Email,
                    e.PersonalInfo.PhoneNumber,
                    RoleId = e.RoleId,
                    RoleTitle = e.Role.RoleTitle
                })
                .ToListAsync();
            
            return employees;
        }

        [HttpGet("contractors")]
        public async Task<IEnumerable<object>> GetContractors()
        {
            // Return all contractors with their personal info and company
            var contractors = await _dbContext.Contractors
                .Include(c => c.PersonalInfo)
                .Include(c => c.Client)
                .Select(c => new
                {
                    c.Id,
                    c.PersonalInfo.FirstName,
                    c.PersonalInfo.LastName,
                    c.PersonalInfo.Email,
                    c.PersonalInfo.PhoneNumber,
                    CompanyName = c.CompanyName,
                    ClientName = c.Client != null ? c.Client.Name : null
                })
                .ToListAsync();
            
            return contractors;
        }

        [HttpGet("people")]
        public async Task<IEnumerable<object>> GetAllPeople()
        {
            // Return all people with their type (employee or contractor)
            var people = await _dbContext.PersonalInfos
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
                    Role = p.Employee != null ? p.Employee.Role.RoleTitle : null
                })
                .ToListAsync();
            
            return people;
        }
    }

    // Add a separate controller for /api level endpoints
    [ApiController]
    [Route("api")]
    public class ApiController : ControllerBase
    {
        [HttpGet("health")]
        public IActionResult HealthCheck()
        {
            return Ok(new { 
                status = "healthy", 
                timestamp = DateTime.UtcNow,
                message = "Backend is running successfully!"
            });
        }
    }

    // Add a separate controller for root-level endpoints
    [ApiController]
    [Route("")]
    public class RootController : ControllerBase
    {
        [HttpGet("health")]
        public IActionResult HealthCheck()
        {
            return Ok(new { 
                status = "healthy", 
                timestamp = DateTime.UtcNow,
                message = "Backend is running successfully!"
            });
        }

        [HttpGet("")]
        public IActionResult Root()
        {
            return Ok(new { 
                message = "Density Reporting Tool Backend API",
                timestamp = DateTime.UtcNow,
                endpoints = new[] {
                    "/health",
                    "/api/test/health",
                    "/api/test/employees",
                    "/api/test/contractors",
                    "/api/test/people"
                }
            });
        }

        [HttpGet("home")]
        public IActionResult Home()
        {
            return Ok(new { 
                message = "Welcome to Density Reporting Tool Backend",
                timestamp = DateTime.UtcNow
            });
        }

        [HttpGet("status")]
        public IActionResult Status()
        {
            return Ok(new { 
                status = "running", 
                timestamp = DateTime.UtcNow,
                environment = "Development"
            });
        }
    }
}
