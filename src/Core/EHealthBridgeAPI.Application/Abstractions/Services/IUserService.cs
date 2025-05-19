using Core.Results;
using EHealthBridgeAPI.Application.DTOs;
using EHealthBridgeAPI.Application.DTOs.User;
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
        Task<IDataResult<IEnumerable<AppUserDto>>> GetAllAsync();
        Task<IDataResult<AppUserDto>> GetByIdAsync(int id);
        Task<IDataResult<int>> CreateAsync(RegisterRequestDto userRegisterDto);
        Task<IDataResult<AppUserDto>> GetByUsernameAsync(string username);
        Task<Result> UpdateAsync(int id, UpdateUserRequestDto updateUserRequestDto);
        Task<Result> RemoveByIdAsync(int id);
    }
}