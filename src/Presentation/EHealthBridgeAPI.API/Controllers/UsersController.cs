using Core.Results;
using EHealthBridgeAPI.Application.Abstractions.Services;
using EHealthBridgeAPI.Application.Abstractions.Token;
using EHealthBridgeAPI.Application.Constant;
using EHealthBridgeAPI.Application.DTOs;
using EHealthBridgeAPI.Application.DTOs.User;
using EHealthBridgeAPI.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EHealthBridgeAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : BaseApiController
    {
        private readonly IUserService _userService;
        private readonly ITokenHandler _tokenHandler;
        public UsersController(IUserService userService, ITokenHandler tokenHandler)
        {
            _userService = userService;
            _tokenHandler = tokenHandler;
        }

        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok("Api is working");
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            return GetResponseResult(await _userService.CreateAsync(request));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            return GetResponseResult(await _userService.GetByIdAsync(id));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            return GetResponseResult(await _userService.RemoveByIdAsync(id));
        }

        [Authorize(AuthenticationSchemes = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            return GetResponseResult(await _userService.GetAllAsync());
        }
    }
}