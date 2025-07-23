using TLA.Core.Entities;

namespace TLA.Core.Interfaces
{
    public interface IRoleService
    {
        Task<List<Role>> GetUserRolesAsync(int userId);
        Task<bool> AssignRoleToUserAsync(int userId, int roleId);
        Task<Role> GetRoleByNameAsync(string roleName);
        Task<List<Role>> GetAllRolesAsync();
    }
}