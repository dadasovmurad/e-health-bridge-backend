using EHealthBridgeAPI.Application.Abstractions.Token;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EHealthBridgeAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public readonly ITokenHandler _tokenHandler;

        public AuthController(ITokenHandler tokenHandler)
        {
            _tokenHandler = tokenHandler;
        }
        public IActionResult<
    }
}
