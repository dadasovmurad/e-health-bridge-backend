using EHealthBridgeAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EHealthBridgeAPI.Application.Abstractions.Services
{
    public interface IUserService
    {
        Task<IEnumerable<AppUser>> GetAllUsersAsync();
        Task<AppUser?> GetUserByIdAsync(int id);
        Task<int> CreateUserAsync(AppUser user);
        Task<bool> UpdateUserAsync(AppUser user);
        Task<bool> DeleteUserAsync(int id);
    }
}
