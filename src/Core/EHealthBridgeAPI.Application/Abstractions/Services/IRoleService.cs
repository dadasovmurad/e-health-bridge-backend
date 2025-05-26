using Core.Results;
using EHealthBridgeAPI.Domain.Entities;

namespace EHealthBridgeAPI.Application.Abstractions.Services
{
    public interface IRoleService
    {
        Task<IDataResult<IEnumerable<Role>>> GetAllAsync();
        Task<IDataResult<Role>> GetByIdAsync(int id);
        Task<IDataResult<Role>> GetByNameAsync(string name);
        Task<IDataResult<IEnumerable<Role>>> GetUserRolesAsync(int userId);
        Task<IResult> AssignRoleToUserAsync(int userId, int roleId);
        Task<IResult> RemoveRoleFromUserAsync(int userId, int roleId);
        Task<IDataResult<int>> CreateAsync(string name, string description);
        Task<IResult> UpdateAsync(int id, string name, string description);
        Task<IResult> DeleteAsync(int id);
    }
} 