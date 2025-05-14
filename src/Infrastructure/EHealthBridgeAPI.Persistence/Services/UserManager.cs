
using EHealthBridgeAPI.Application.Abstractions.Services;
using EHealthBridgeAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EHealthBridgeAPI.Persistence.Services
{
    public class UserManager : IUserManager
    {
        private readonly IUserService _userService;

        public UserManager(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<int> RegisterUserAsync(AppUser user, string rawPassword)
        {
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(rawPassword);
            user.CreatedAt = DateTimeOffset.UtcNow;
            user.UpdatedAt = DateTimeOffset.UtcNow;

            return await _userService.CreateUserAsync(user);
        }

        public async Task<AppUser?> AuthenticateUserAsync(string username, string rawPassword)
        {
            var users = await _userService.GetAllUsersAsync();
            var user = users.FirstOrDefault(u => u.Username == username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(rawPassword, user.PasswordHash))
                return null;

            return user;
        }
    }
}
