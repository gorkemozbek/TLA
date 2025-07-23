using Microsoft.EntityFrameworkCore;
using TLA.Core.Entities;
using TLA.Core.Interfaces;
using TLA.Infrastructure.Data;

namespace TLA.Infrastructure.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly TLADbContext _context;

        public PermissionService(TLADbContext context)
        {
            _context = context;
        }

        public async Task<List<Permission>> GetUserPermissionsAsync(int userId)
        {
            return await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .SelectMany(ur => ur.Role.RolePermissions)
                .Select(rp => rp.Permission)
                .Distinct()
                .ToListAsync();
        }

        public async Task<bool> UserHasPermissionAsync(int userId, string permissionName)
        {
            return await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .SelectMany(ur => ur.Role.RolePermissions)
                .AnyAsync(rp => rp.Permission.Name == permissionName);
        }

        public async Task<List<Permission>> GetAllPermissionsAsync()
        {
            return await _context.Permissions.ToListAsync();
        }

        public async Task<Permission> GetPermissionByNameAsync(string name)
        {
            return await _context.Permissions.FirstOrDefaultAsync(p => p.Name == name);
        }
    }
}

