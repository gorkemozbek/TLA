using TLA.Core.Entities;

namespace TLA.Core.Interfaces
{
    public interface IPermissionService
    {
        Task<List<Permission>> GetUserPermissionsAsync(int userId);
        Task<bool> UserHasPermissionAsync(int userId, string permissionName);
        Task<List<Permission>> GetAllPermissionsAsync();
        Task<Permission> GetPermissionByNameAsync(string name);
    }
}