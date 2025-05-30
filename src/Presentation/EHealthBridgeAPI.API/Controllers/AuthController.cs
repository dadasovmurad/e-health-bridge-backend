using EHealthBridgeAPI.Application.Abstractions.Services;
using EHealthBridgeAPI.Application.Abstractions.Token;
using EHealthBridgeAPI.Application.DTOs.Auth;
using Microsoft.AspNetCore.Mvc;

namespace EHealthBridgeAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : BaseApiController
    {
        public readonly ITokenHandler _tokenHandler;
        private readonly IAuthService _authService;
        private readonly IUserService _userService;
        public AuthController(ITokenHandler tokenHandler, IAuthService authService, IUserService userService)
        {
            _tokenHandler = tokenHandler;
            _authService = authService;
            _userService = userService;
        }
        [HttpPost("[action]")]
        public async Task<IActionResult> Login([FromBody] InternalLoginRequestDto internalLoginRequestDto)
        {
            return GetResponseResult(await _authService.LoginAsync(internalLoginRequestDto));
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto internalLoginRequestDto)
        {
            return GetResponseResult(await _authService.RefreshTokenAsync(internalLoginRequestDto.RefreshToken));
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto model)
        {
            return GetResponseResult(await _authService.GeneratePasswordResetTokenAsync(model.Email));
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto model)
        {
            return GetResponseResult(await _authService.ResetPasswordAsync(model.Token, model.NewPassword));
        }
    }
}
