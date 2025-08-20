using Microsoft.AspNetCore.Mvc;

namespace DensityReportingToolBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        [HttpGet("/")]
        public IActionResult Root()
        {
            return Ok(new { 
                message = "Density Reporting Tool Backend API", 
                status = "running", 
                timestamp = DateTime.UtcNow,
                endpoints = new[] { 
                    "/api/test", 
                    "/api/test/health", 
                    "/swagger" 
                }
            });
        }

        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok(new { 
                status = "healthy", 
                timestamp = DateTime.UtcNow,
                service = "Density Reporting Tool Backend"
            });
        }
    }
}
