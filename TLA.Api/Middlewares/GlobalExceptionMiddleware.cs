using System.Net;
using System.Text.Json;
using TLA.Core.DTOs.Response;

namespace TLA.Api.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var response = new ApiResponse<object>();

            switch (exception)
            {
                case UnauthorizedAccessException:
                    response = ApiResponse<object>.ErrorResponse("Unauthorized access", exception.Message);
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    break;
                case ArgumentException:
                    response = ApiResponse<object>.ErrorResponse("Invalid argument", exception.Message);
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;
                case KeyNotFoundException:
                    response = ApiResponse<object>.ErrorResponse("Resource not found", exception.Message);
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    break;
                default:
                    response = ApiResponse<object>.ErrorResponse("An internal server error occurred", "Please try again later");
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(jsonResponse);
        }
    }
}