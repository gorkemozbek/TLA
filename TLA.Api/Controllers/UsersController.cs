using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TLA.Core.DTOs;
using TLA.Core.DTOs.Response;
using TLA.Core.Interfaces;
using TLA.Infrastructure.Services;

namespace TLA.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<UserDto>>>> GetAll()
        {
            var result = await _userService.GetAllAsync();

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<UserDto>>> GetById(int id)
        {
            if (id <= 0)
                return BadRequest(ApiResponse<UserDto>.ErrorResponse("Invalid user ID"));

            var result = await _userService.GetByIdAsync(id);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        [HttpGet("by-email/{email}")]
        public async Task<ActionResult<ApiResponse<UserDto>>> GetByEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                return BadRequest(ApiResponse<UserDto>.ErrorResponse("Email is required"));

            var result = await _userService.GetByEmailAsync(email);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }
    }
}
