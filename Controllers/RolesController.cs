using DensityReportingToolBackend.DTOs.People;
using DensityReportingToolBackend.Infrastructure;
using DensityReportingToolBackend.Services;
using Microsoft.AspNetCore.Mvc;

namespace DensityReportingToolBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController(IPeopleService peopleService) : BaseApiController
    {
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<RoleReadDto>>>> GetAllRoles()
        {
            var roles = await peopleService.GetAllRolesAsync();
            return Success(roles, "Roles retrieved successfully");
        }
    }
}
