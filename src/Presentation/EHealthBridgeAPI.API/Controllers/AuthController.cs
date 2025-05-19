using EHealthBridgeAPI.Application.Abstractions.Services;
using EHealthBridgeAPI.Application.Abstractions.Token;
using EHealthBridgeAPI.Application.DTOs.Auth;
using Microsoft.AspNetCore.Mvc;

namespace EHealthBridgeAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public readonly ITokenHandler _tokenHandler;
        private readonly IAuthService _authService;

        public AuthController(ITokenHandler tokenHandler, IAuthService authService)
        {
            _tokenHandler = tokenHandler;
            _authService = authService;
        }
        [HttpPost("[action]")]
        public async Task<IActionResult> Login([FromBody] InternalLoginRequestDto internalLoginRequestDto)
        {
            return Ok(await _authService.LoginAsync(internalLoginRequestDto));
        }
    }
}
