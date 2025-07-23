using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TLA.Core.DTOs.Response;
using TLA.Core.Interfaces;

namespace TLA.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AdminController : ControllerBase
    {
        private readonly IRoleService _roleService;
        private readonly IPermissionService _permissionService;

        public AdminController(IRoleService roleService, IPermissionService permissionService)
        {
            _roleService = roleService;
            _permissionService = permissionService;
        }

        [HttpGet("roles")]
        public async Task<ActionResult<ApiResponse<object>>> GetAllRoles()
        {
            var roles = await _roleService.GetAllRolesAsync();
            return Ok(ApiResponse<object>.SuccessResponse(roles));
        }

        [HttpGet("permissions")]
        public async Task<ActionResult<ApiResponse<object>>> GetAllPermissions()
        {
            var permissions = await _permissionService.GetAllPermissionsAsync();
            return Ok(ApiResponse<object>.SuccessResponse(permissions));
        }

        [HttpPost("assign-role")]
        public async Task<ActionResult<ApiResponse<bool>>> AssignRole([FromBody] AssignRoleRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(ApiResponse<bool>.ErrorResponse("Validation failed", errors));
            }

            var result = await _roleService.AssignRoleToUserAsync(request.UserId, request.RoleId);

            if (!result)
                return BadRequest(ApiResponse<bool>.ErrorResponse("Failed to assign role"));

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Role assigned successfully"));
        }
    }

    public class AssignRoleRequest
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
    }
}