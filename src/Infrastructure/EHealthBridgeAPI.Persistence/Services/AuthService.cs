using EHealthBridgeAPI.Application.Abstractions.Services;
using EHealthBridgeAPI.Application.Abstractions.Token;
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
        public AuthService(IUserService userService, ITokenHandler tokenHandler,IMapper mapper)
        {
            _userService = userService;
            _tokenHandler = tokenHandler;
            _mapper = mapper;
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
            var newuser = _mapper.Map<AppUser>(user);
            var token = _tokenHandler.CreateAccessToken(3600, newuser);

            return new SuccessDataResult<LoginDto>(new LoginDto(token), Messages.LoginSuccess);
        }
    }
}
