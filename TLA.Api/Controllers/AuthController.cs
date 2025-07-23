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
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<LoginResponse>>> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(ApiResponse<LoginResponse>.ErrorResponse("Validation failed", errors));
            }

            var result = await _authService.LoginAsync(loginDto);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse<UserDto>>> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(ApiResponse<UserDto>.ErrorResponse("Validation failed", errors));
            }

            var result = await _authService.RegisterAsync(registerDto);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("validate-token")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<bool>>> ValidateToken()
        {
            var token = HttpContext.Request.Headers["Authorization"]
                .FirstOrDefault()?.Split(" ").Last();

            if (string.IsNullOrEmpty(token))
                return BadRequest(ApiResponse<bool>.ErrorResponse("Token not provided"));

            var result = await _authService.ValidateTokenAsync(token);
            return Ok(result);
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<UserDto>>> GetCurrentUser()
        {
            var userEmail = User.Identity?.Name;
            if (string.IsNullOrEmpty(userEmail))
                return BadRequest(ApiResponse<UserDto>.ErrorResponse("User not found"));

            // You can expand this to get full user details
            var userDto = new UserDto
            {
                Email = userEmail,
                // Add other properties as needed
            };

            return Ok(ApiResponse<UserDto>.SuccessResponse(userDto));
        }
    }
}