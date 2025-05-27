using EHealthBridgeAPI.Domain.Entities;

namespace EHealthBridgeAPI.Application.Repositories
{
    public interface IRoleRepository : IGenericRepository<Role>
    {
        Task<Role> GetByNameAsync(string name);
        Task<IEnumerable<Role>> GetUserRolesAsync(int userId);
        Task<bool> AssignRoleToUserAsync(int userId, int roleId);
        Task<bool> RemoveRoleFromUserAsync(int userId, int roleId);
    }
} 