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
        public async Task<IEnumerable<GeoPacificEmployee>> Get()
        {
            // Return all employees from the database
            return await _dbContext.GeoPacificEmployees
                .Include(e => e.Role)
                .ToListAsync();
        }
    }
}
