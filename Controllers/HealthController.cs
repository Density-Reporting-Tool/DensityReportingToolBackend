using Microsoft.AspNetCore.Mvc;

namespace DensityReportingToolBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { 
                status = "healthy", 
                timestamp = DateTime.UtcNow,
                service = "Density Reporting Tool Backend",
                version = "1.0.0"
            });
        }

        [HttpGet("api/health")]
        public IActionResult ApiHealth()
        {
            return Ok(new { 
                status = "healthy", 
                timestamp = DateTime.UtcNow, 
                endpoint = "api/health" 
            });
        }
    }
}
