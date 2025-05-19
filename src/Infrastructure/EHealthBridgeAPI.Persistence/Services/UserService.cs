using Core.Results;
using EHealthBridgeAPI.Application.Abstractions.Services;
using EHealthBridgeAPI.Application.Constant;
using EHealthBridgeAPI.Application.DTOs;
using EHealthBridgeAPI.Application.DTOs.User;
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
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IDataResult<IEnumerable<AppUserDto>>> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();
            List<AppUserDto> result = new List<AppUserDto>();
            foreach (var item in users)
            {
                result.Add(new(item.FirstName, item.LastName, item.Email, item.Username, item.PasswordHash));
            }

            return new SuccessDataResult<IEnumerable<AppUserDto>>(result);
        }

        public async Task<IDataResult<AppUserDto>> GetByUsernameAsync(string username)
        {
            var requestUser = await _userRepository.GetByUsernameAsync(username);
            if (requestUser is null)
            {
                return new ErrorDataResult<AppUserDto>(Messages.UserNotFound);
            }

            var appUserDto = new AppUserDto(requestUser.FirstName, requestUser.LastName, requestUser.Email, requestUser.Username, requestUser.PasswordHash);

            return new SuccessDataResult<AppUserDto>(appUserDto);
        }

        public async Task<IDataResult<AppUserDto>> GetByIdAsync(int id)
        {
            var userById = await _userRepository.GetByIdAsync(id);
            if (userById is null)
            {
                return new ErrorDataResult<AppUserDto>(Messages.UserNotFound);
            }

            return new SuccessDataResult<AppUserDto>(new AppUserDto(userById.Username,userById.LastName,userById.Email,userById.Username,userById.PasswordHash));
        }

        public async Task<IDataResult<int>> CreateAsync(RegisterRequestDto registerRequestDto)
        {
            var requestUser = await _userRepository.GetByUsernameAsync(registerRequestDto.UserName);
            if (requestUser is not null)
            {
                return new ErrorDataResult<int>(Messages.UserAlreadyExists);
            }

            var newUser = new AppUser
            {
                Username = registerRequestDto.UserName,
                Email = registerRequestDto.Email,
                FirstName = registerRequestDto.FirstName,
                LastName = registerRequestDto.LastName,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerRequestDto.Password)
            };

            var createdUser = await _userRepository.InsertAsync(newUser);
            if (createdUser == 0)
            {
                return new ErrorDataResult<int>(Messages.UserNotCreated);
            }
            return new SuccessDataResult<int>(createdUser, Messages.Usercreated);
        }

        public async Task<Result> UpdateAsync(int id, UpdateUserRequestDto updateUserRequestDto)
        {
            var userById = await _userRepository.GetByIdAsync(id);
            if (userById is not null)
            {
                var user = new AppUser
                {
                    Id = id,
                    Email = userById.Email,
                    FirstName = userById.FirstName,
                    LastName = userById.LastName,
                    Username = userById.Username,
                };

                var updatedStatus = await _userRepository.UpdateAsync(user);
                if (updatedStatus)
                {
                    return new SuccessResult(Messages.UserSuccessfullyUpdated);
                }
                return new ErrorResult(Messages.UserNotUpdated);
            }
            return new ErrorResult(Messages.UserNotFound);
        }

        public async Task<Result> RemoveByIdAsync(int id)
        {
            var deletedStatus = await _userRepository.DeleteAsync(id);
            if (!deletedStatus)
            {
                return new ErrorResult(Messages.UserNotDeleted);
            }
            return new SuccessResult(Messages.UserDeleted);
        }
    }
}
