using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Results;
using EHealthBridgeAPI.Application.Abstractions.Services;
using EHealthBridgeAPI.Application.Constant;
using EHealthBridgeAPI.Application.DTOs;
using EHealthBridgeAPI.Domain.Entities;
using EHealthBridgeAPI.Application.Exceptions;
using EHealthBridgeAPI.Application.Abstractions.Token;
using EHealthBridgeAPI.Application.DTOs.Auth;


namespace EHealthBridgeAPI.Persistence.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserService _userService;
        private readonly ITokenHandler _tokenHandler;
        public AuthService(IUserService userService, ITokenHandler tokenHandler)
        {
            _userService = userService;
            _tokenHandler = tokenHandler;
        }

        public async Task<IDataResult<LoginDto>> LoginAsync(InternalLoginRequestDto internalLoginRequestDto)
        {
            var requestUser = await _userService.GetByUsernameAsync(internalLoginRequestDto.Username);
            if (!requestUser.IsSuccess)
            {
                return new ErrorDataResult<LoginDto>(Messages.LoginFailure);
            }
            var user = requestUser.Data;

            if (!BCrypt.Net.BCrypt.Verify(internalLoginRequestDto.Password, user!.PasswordHash)) // user is not null (validated earlier in UserService)
            {
                return new ErrorDataResult<LoginDto>(Messages.LoginFailure);
            }

            var token = _tokenHandler.CreateAccessToken(3600, new AppUser(user.Username, user.Email, user.PasswordHash, user.FirstName, user.LastName));

            return new SuccessDataResult<LoginDto>(new LoginDto(token), Messages.LoginSuccess);
        }
    }
}
