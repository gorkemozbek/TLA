using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TLA.Core.DTOs.Response;
using TLA.Core.Interfaces;
using System.Text.Json;

namespace TLA.Api.Middlewares
{
    public class PermissionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<PermissionMiddleware> _logger;

        public PermissionMiddleware(RequestDelegate next, ILogger<PermissionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, IPermissionService permissionService)
        {
            // Skip permission check for auth endpoints and swagger
            var path = context.Request.Path.Value?.ToLower();
            if (path?.Contains("/auth/") == true ||
                path?.Contains("/swagger") == true ||
                path?.Contains("/health") == true ||
                context.Request.Method == "OPTIONS")
            {
                await _next(context);
                return;
            }

            // Check if user is authenticated
            if (!context.User.Identity.IsAuthenticated)
            {
                await _next(context);
                return;
            }

            // Get required permission based on endpoint and method
            var requiredPermission = GetRequiredPermission(context.Request.Path, context.Request.Method);

            if (string.IsNullOrEmpty(requiredPermission))
            {
                await _next(context);
                return;
            }

            // Get user ID from claims
            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
            {
                await UnauthorizedResponse(context, "Invalid user token");
                return;
            }

            // Check if user has required permission
            var hasPermission = await permissionService.UserHasPermissionAsync(userId, requiredPermission);
            if (!hasPermission)
            {
                await UnauthorizedResponse(context, $"Insufficient permissions. Required: {requiredPermission}");
                return;
            }

            await _next(context);
        }

        private string GetRequiredPermission(PathString path, string method)
        {
            var pathValue = path.Value?.ToLower();

            // Define permission mappings based on your endpoints
            if (pathValue?.Contains("/users") == true)
            {
                return method.ToUpper() switch
                {
                    "GET" => "user.read",
                    "POST" => "user.write",
                    "PUT" => "user.write",
                    "DELETE" => "user.delete",
                    _ => null
                };
            }

            if (pathValue?.Contains("/admin") == true)
            {
                return "admin.panel";
            }

            return null; // No specific permission required
        }

        private async Task UnauthorizedResponse(HttpContext context, string message)
        {
            context.Response.StatusCode = 403;
            context.Response.ContentType = "application/json";

            var response = ApiResponse<object>.ErrorResponse("Access denied", message);
            var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(jsonResponse);
        }
    }
}