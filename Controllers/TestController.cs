using DensityReportingToolBackend.Data;
using DensityReportingToolBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DensityReportingToolBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        private readonly ILogger<TestController> _logger;
        private readonly AppDbContext _dbContext;

        public TestController(ILogger<TestController> logger, AppDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        [HttpGet(Name = "GetGeoPacificEmployees")]
        public async Task<IEnumerable<object>> Get()
        {
            // Return all employees from the database with role information
            // Using projection to avoid circular references
            var employees = await _dbContext.GeoPacificEmployees
                .Select(e => new
                {
                    e.Id,
                    e.FirstName,
                    e.LastName,
                    e.Email,
                    e.PhoneNumber,
                    RoleId = e.RoleId,
                    RoleTitle = e.Role.RoleTitle
                })
                .ToListAsync();
            
            return employees;
        }
    }
}
