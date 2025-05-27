using EHealthBridgeAPI.Domain.Entities;

namespace EHealthBridgeAPI.Application.Repositories
{
    public interface IUserRepository : IGenericRepository<AppUser>
    {
        Task<AppUser> GetByUsernameAsync(string username);
        Task<AppUser?> GetByEmailAsync(string email);
        Task<AppUser?> GetByResetTokenAsync(string token);
        Task<AppUser?> GetByRefreshTokenAsync(string refreshToken);
    }
}
