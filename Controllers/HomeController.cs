using Microsoft.AspNetCore.Mvc;

namespace DensityReportingToolBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        public IActionResult Index()
        {
            return Ok(new { 
                message = "Density Reporting Tool Backend API", 
                status = "running", 
                timestamp = DateTime.UtcNow,
                endpoints = new[] { 
                    "/health", 
                    "/api/test/health", 
                    "/api/test",
                    "/swagger"
                },
                documentation = "/swagger"
            });
        }

        [HttpGet("/")]
        public IActionResult Root()
        {
            return Redirect("/home");
        }
    }
}
