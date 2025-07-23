using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TLA.Core.DTOs;
using TLA.Core.DTOs.Response;
using TLA.Core.Entities;
using TLA.Core.Interfaces;
using TLA.Infrastructure.Data;

namespace TLA.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly TLADbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(TLADbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<ApiResponse<LoginResponse>> LoginAsync(LoginDto loginDto)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                    .ThenInclude(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
                    .FirstOrDefaultAsync(u => u.Email == loginDto.Email);

                if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password))
                {
                    return ApiResponse<LoginResponse>.ErrorResponse("Invalid email or password");
                }

                var permissions = await GetUserPermissionsAsync(user.Id);
                var token = GenerateJwtToken(user, permissions);

                var userDto = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    Roles = user.UserRoles.Select(ur => ur.Role.RoleName).ToList(),
                    Permissions = permissions
                };

                var response = new LoginResponse
                {
                    Token = token,
                    User = userDto,
                    ExpiresAt = DateTime.UtcNow.AddDays(7)
                };

                return ApiResponse<LoginResponse>.SuccessResponse(response, "Login successful");
            }
            catch (Exception ex)
            {
                return ApiResponse<LoginResponse>.ErrorResponse("Login failed", ex.Message);
            }
        }

        public async Task<ApiResponse<UserDto>> RegisterAsync(RegisterDto registerDto)
        {
            try
            {
                if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
                {
                    return ApiResponse<UserDto>.ErrorResponse("Email already exists");
                }

                var user = new User
                {
                    Email = registerDto.Email,
                    Password = BCrypt.Net.BCrypt.HashPassword(registerDto.Password)
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Assign default User role
                var userRole = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == "User");
                if (userRole != null)
                {
                    _context.UserRoles.Add(new UserRole { UserId = user.Id, RoleId = userRole.Id });
                    await _context.SaveChangesAsync();
                }

                var userDto = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    Roles = new List<string> { "User" }
                };

                return ApiResponse<UserDto>.SuccessResponse(userDto, "Registration successful");
            }
            catch (Exception ex)
            {
                return ApiResponse<UserDto>.ErrorResponse("Registration failed", ex.Message);
            }
        }

        public async Task<ApiResponse<bool>> ValidateTokenAsync(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return ApiResponse<bool>.SuccessResponse(true, "Token is valid");
            }
            catch
            {
                return ApiResponse<bool>.ErrorResponse("Invalid token");
            }
        }

        public async Task<List<string>> GetUserPermissionsAsync(int userId)
        {
            return await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .SelectMany(ur => ur.Role.RolePermissions)
                .Select(rp => rp.Permission.Name)
                .Distinct()
                .ToListAsync();
        }

        private string GenerateJwtToken(User user, List<string> permissions)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            };

            // Add permissions as claims
            foreach (var permission in permissions)
            {
                claims.Add(new Claim("permission", permission));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}