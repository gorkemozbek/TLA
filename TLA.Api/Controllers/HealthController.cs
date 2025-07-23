using Microsoft.AspNetCore.Mvc;
using TLA.Core.DTOs.Response;

namespace TLA.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public ActionResult<ApiResponse<object>> Get()
        {
            var health = new
            {
                Status = "Healthy",
                Timestamp = DateTime.UtcNow,
                Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"
            };

            return Ok(ApiResponse<object>.SuccessResponse(health, "API is running"));
        }
    }
}