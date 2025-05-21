using EHealthBridgeAPI.Application.Abstractions.Services;
using EHealthBridgeAPI.Application.Abstractions.Token;
using EHealthBridgeAPI.Application.DTOs.Auth;
using EHealthBridgeAPI.Persistence.Services;
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


        // 1. Token yaradılması üçün e-poçta əsaslanan endpoint
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto model)
        {
            var result = await _userService.GeneratePasswordResetTokenAsync(model.Email);
            if (!result.IsSuccess)
                return BadRequest(result.Message);

            return Ok(result.Message);
        }

        // 2. Yeni şifrə təyin olunması üçün endpoint
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto model)
        {
            var result = await _userService.ResetPasswordAsync(model.Token, model.NewPassword);
            if (!result.IsSuccess)
                return BadRequest(result.Message);

            return Ok(result.Message);
        }
    }
}
