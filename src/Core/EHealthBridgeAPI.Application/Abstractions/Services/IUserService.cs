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
        Task<IResult> UpdateAsync(int id, UpdateUserRequestDto updateUserRequestDto);
        Task<IResult> RemoveByIdAsync(int id);
        Task<IDataResult<AppUserDto>> GetByEmailAsync(string email);
        Task<IResult> GeneratePasswordResetTokenAsync(string email);
        Task<IResult> ResetPasswordAsync(string token, string newPassword);
    }
}