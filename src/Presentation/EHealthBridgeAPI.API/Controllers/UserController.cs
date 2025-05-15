using Core.Results;
using EHealthBridgeAPI.Application.Abstractions.Services;
using EHealthBridgeAPI.Application.Abstractions.Token;
using EHealthBridgeAPI.Application.Constant;
using EHealthBridgeAPI.Application.DTOs.User;
using EHealthBridgeAPI.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EHealthBridgeAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserManager _userManager;
        private readonly IUserService _userService;
        private readonly ITokenHandler _tokenHandler;
        public UsersController(IUserManager userManager, IUserService userService, ITokenHandler tokenHandler)
        {
            _userManager = userManager;
            _userService = userService;
            _tokenHandler = tokenHandler;
        }
        [HttpGet("test")]
        public async Task<IActionResult> Test()
        {
            return (Ok("Api is working"));
        }

        // Register a new user
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var existingUser = await _userService.GetByEmailOrName(request);

            if (!existingUser.Success)
            {
                return Ok(existingUser);
            }

            // Register user
            var newUser = new AppUser
            {
                Username = request.Username,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName
            };

            var userId = await _userManager.RegisterUserAsync(newUser, request.Password);

            return Ok(userId);
        }

        //// Authenticate user (login)
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _userManager.AuthenticateUserAsync(request.Username, request.Password);

            if (user.Data == null && !user.Success)
                return Unauthorized(Messages.UserNotFound);

            var token = _tokenHandler.CreateAccessToken(3600, user.Data);

            return Ok(token);
        }

        //// Get user by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _userService.GetByIdAsync(id);

            return Ok(user);
        }

        // Update user details
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserRequest request)
        {
            var existingUser = await _userService.GetByIdAsync(id);

            if (!existingUser.Success)
                return NotFound(Messages.UserNotFound);
            var user = existingUser.Data;
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.Username = request.Username;
            user.Email = request.Email;

            var updated = await _userService.UpdateAsync(user);
            return Ok(updated);
        }

        // Delete user
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var deleted = await _userService.RemoveByIdAsync(id);

            return Ok(deleted);
        }
        [Authorize(AuthenticationSchemes = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }
    }
}