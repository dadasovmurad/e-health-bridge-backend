using EHealthBridgeAPI.Application.Abstractions.Services;
using EHealthBridgeAPI.Application.Abstractions.Token;
using EHealthBridgeAPI.Application.Repositories;
using EHealthBridgeAPI.Application.DTOs.User;
using EHealthBridgeAPI.Application.DTOs.Auth;
using EHealthBridgeAPI.Application.Constant;
using EHealthBridgeAPI.Application.DTOs;
using EHealthBridgeAPI.Domain.Entities;
using Core.Results;
using AutoMapper;

namespace EHealthBridgeAPI.Persistence.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserService _userService;
        private readonly ITokenHandler _tokenHandler;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        
        public AuthService(IUserService userService, ITokenHandler tokenHandler, IMapper mapper, IUserRepository userRepository)
        {
            _userService = userService;
            _tokenHandler = tokenHandler;
            _mapper = mapper;
            _userRepository = userRepository;
        }

        public async Task<IDataResult<LoginDto>> LoginAsync(InternalLoginRequestDto internalLoginRequestDto)
        {
            var requestUser = await _userService.GetByUsernameAsync(internalLoginRequestDto.Username);
            if (!requestUser.IsSuccess)
            {
                return new ErrorDataResult<LoginDto>(Messages.LoginFailure);
            }
            var user = requestUser.Data;

            if (!BCrypt.Net.BCrypt.Verify(internalLoginRequestDto.Password, user!.PasswordHash))
            {
                return new ErrorDataResult<LoginDto>(Messages.LoginFailure);
            }

            var newuser = _mapper.Map<AppUser>(user);
            var token = await _tokenHandler.CreateAccessTokenAsync(3600, newuser);
            newuser.PasswordResetTokenExpiry = DateTime.MinValue;
            newuser.RefreshToken = token.RefreshToken;
            newuser.RefreshTokenExpiration = token.Expiration.AddMinutes(5);

            await _userRepository.UpdateAsync(newuser);
            return new SuccessDataResult<LoginDto>(new LoginDto(token), Messages.LoginSuccess);
        }

        public async Task<IResult> GeneratePasswordResetTokenAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
                return new ErrorDataResult<AppUserDto>(Messages.UserNotFound);


            var token = Guid.NewGuid().ToString();
            var expiry = DateTime.UtcNow.AddMinutes(30);

            user.PasswordResetToken = token;
            user.PasswordResetTokenExpiry = expiry;

            await _userRepository.UpdateAsync(user);

            return new SuccessResult(Messages.PasswordResetTokenCreated);
        }

        public async Task<IResult> ResetPasswordAsync(string token, string newPassword)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(newPassword))
                return new ErrorResult(Messages.TokenOrPasswordCannotBeEmpty);

            var user = await _userRepository.GetByResetTokenAsync(token);
            if (user == null)
                return new ErrorResult(Messages.InvalidResetToken);

            if (user.PasswordResetTokenExpiry < DateTime.UtcNow)
                return new ErrorResult(Messages.TokenExpired);

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            user.PasswordResetToken = null;
            user.PasswordResetTokenExpiry = null;

            await _userRepository.UpdateAsync(user);

            return new SuccessResult(Messages.PasswordResetSuccess);
        }

        public async Task<IDataResult<TokenDto>> RefreshTokenAsync(string refreshToken)
        {
            var user = await _userRepository.GetByRefreshTokenAsync(refreshToken);

            if (user == null || user.RefreshTokenExpiration < DateTime.UtcNow)
            {
                return new ErrorDataResult<TokenDto>(Messages.RefreshTokenExpired);
            }

            var token = await _tokenHandler.CreateAccessTokenAsync(3600, user);
            user.RefreshToken = token.RefreshToken;
            user.RefreshTokenExpiration = token.Expiration.AddMinutes(5);
            user.PasswordResetTokenExpiry = DateTime.MinValue;
            await _userRepository.UpdateAsync(user);

            return new SuccessDataResult<TokenDto>(token, "Token yeniləndi.");
        }
    }
}
