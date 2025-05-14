
using Core.Results;
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

        public async Task<IDataResult<int>> RegisterUserAsync(AppUser user, string rawPassword)
        {
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(rawPassword);
            user.CreatedAt = DateTime.Now;
            user.UpdatedAt = DateTime.Now;

            return await _userService.CreateAsync(user);
        }

        public async Task<IDataResult<AppUser?>> AuthenticateUserAsync(string username, string rawPassword)
        {
            var usersDataResult = await _userService.GetAllAsync();
            if (usersDataResult.Success)
            {
                var user = usersDataResult.Data.FirstOrDefault(u => u.Username == username);

                if (user == null || !BCrypt.Net.BCrypt.Verify(rawPassword, user.PasswordHash))
                    return new ErrorDataResult<AppUser?>();

                return new SuccessDataResult<AppUser?>();
            }
            return new ErrorDataResult<AppUser?>();
        }
    }
}
