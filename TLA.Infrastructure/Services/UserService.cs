using Microsoft.EntityFrameworkCore;
using TLA.Core.DTOs;
using TLA.Core.DTOs.Response;
using TLA.Core.Entities;
using TLA.Core.Interfaces;
using TLA.Infrastructure.Data;

namespace TLA.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly TLADbContext _context;

        public UserService(TLADbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<UserDto>> GetByIdAsync(int id)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                    .FirstOrDefaultAsync(u => u.Id == id);

                if (user == null)
                {
                    return ApiResponse<UserDto>.ErrorResponse("User not found");
                }

                var userDto = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    Roles = user.UserRoles.Select(ur => ur.Role.RoleName).ToList()
                };

                return ApiResponse<UserDto>.SuccessResponse(userDto);
            }
            catch (Exception ex)
            {
                return ApiResponse<UserDto>.ErrorResponse("Failed to get user", ex.Message);
            }
        }

        public async Task<ApiResponse<UserDto>> GetByEmailAsync(string email)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                    .FirstOrDefaultAsync(u => u.Email == email);

                if (user == null)
                {
                    return ApiResponse<UserDto>.ErrorResponse("User not found");
                }

                var userDto = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    Roles = user.UserRoles.Select(ur => ur.Role.RoleName).ToList()
                };

                return ApiResponse<UserDto>.SuccessResponse(userDto);
            }
            catch (Exception ex)
            {
                return ApiResponse<UserDto>.ErrorResponse("Failed to get user", ex.Message);
            }
        }

        public async Task<ApiResponse<List<UserDto>>> GetAllAsync()
        {
            try
            {
                var users = await _context.Users
                    .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                    .ToListAsync();

                var userDtos = users.Select(user => new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    Roles = user.UserRoles.Select(ur => ur.Role.RoleName).ToList()
                }).ToList();

                return ApiResponse<List<UserDto>>.SuccessResponse(userDtos);
            }
            catch (Exception ex)
            {
                return ApiResponse<List<UserDto>>.ErrorResponse("Failed to get users", ex.Message);
            }
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> UserExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<User> CreateUserAsync(string email, string password)
        {
            var user = new User
            {
                Email = email,
                Password = BCrypt.Net.BCrypt.HashPassword(password)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }
    }
}