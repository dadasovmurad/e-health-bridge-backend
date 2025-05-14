using EHealthBridgeAPI.Application.Abstractions.Services;
using EHealthBridgeAPI.Application.Repositories;
using EHealthBridgeAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EHealthBridgeAPI.Persistence.Services
{
    public class UserService : IUserService
    {
        private readonly IGenericRepository<AppUser> _userRepository;

        public UserService(IGenericRepository<AppUser> userRepository)
        {
            _userRepository = userRepository;
        }

        public Task<IEnumerable<AppUser>> GetAllUsersAsync()
            => _userRepository.GetAllAsync();

        public Task<AppUser?> GetUserByIdAsync(int id)
            => _userRepository.GetByIdAsync(id);

        public Task<int> CreateUserAsync(AppUser user)
            => _userRepository.InsertAsync(user);

        public Task<bool> UpdateUserAsync(AppUser user)
            => _userRepository.UpdateAsync(user);

        public Task<bool> DeleteUserAsync(int id)
            => _userRepository.DeleteAsync(id);
    }
}
