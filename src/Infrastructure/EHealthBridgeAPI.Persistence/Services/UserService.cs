using Core.Results;
using EHealthBridgeAPI.Application.Abstractions.Services;
using EHealthBridgeAPI.Application.Constant;
using EHealthBridgeAPI.Application.Repositories;
using EHealthBridgeAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
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

        public async Task<IDataResult<IEnumerable<AppUser>>> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return new SuccessDataResult<IEnumerable<AppUser>>(users);
        }

        public async Task<IDataResult<AppUser?>> GetByIdAsync(int id)
        {
            var userById = await _userRepository.GetByIdAsync(id);

            if (userById != null)
            {
                return new SuccessDataResult<AppUser?>(userById);
            }
            else
            {
                return new ErrorDataResult<AppUser?>(Messages.UserNotFound);
            }

        }

        public async Task<IDataResult<int>> CreateAsync(AppUser user)
        {
            var createdUser = await _userRepository.InsertAsync(user);
            if (createdUser == 0)
            {
                return new ErrorDataResult<int>(Messages.UserNotCreated);
            }
            else
            {
                return new SuccessDataResult<int>(Messages.Usercreated);
            }
        }

        public async Task<Result> UpdateAsync(AppUser user)
        {
            var updatedStatus = await _userRepository.UpdateAsync(user);
            if (!updatedStatus)
            {
                return new ErrorResult(Messages.UserNotUpdated);
            }
            else
            {
                return new SuccessResult(Messages.UserUpdated);
            }
        }

        public async Task<Result> RemoveByIdAsync(int id)
        {
            var deletedStatus = await _userRepository.DeleteAsync(id);
            if (!deletedStatus)
            {
                return new ErrorResult(Messages.UserNotDeleted);
            }
            else
            {
                return new SuccessResult(Messages.UserDeleted);
            }
        }
    }
}
