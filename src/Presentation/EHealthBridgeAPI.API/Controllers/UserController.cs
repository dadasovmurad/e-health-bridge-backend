using EHealthBridgeAPI.Application.Abstractions.Services;
using EHealthBridgeAPI.Application.Abstractions.Token;
using EHealthBridgeAPI.Application.DTOs.User;
using EHealthBridgeAPI.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EHealthBridgeAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserManager _userManager;
        private readonly IUserService _userService;
        private readonly ITokenHandler _tokenHandler;
        public UserController(IUserManager userManager, IUserService userService, ITokenHandler tokenHandler)
        {
            _userManager = userManager;
            _userService = userService;
            _tokenHandler = tokenHandler;
        }

        // Register a new user
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (request == null)
                return BadRequest("Invalid request.");

            // Check if username or email already exists
            var existingUser = await _userService.GetAllUsersAsync();
            if (existingUser.Any(u => u.Username == request.Username))
                return Conflict("Username already exists.");
            if (existingUser.Any(u => u.Email == request.Email))
                return Conflict("Email already exists.");

            // Register user
            var newUser = new AppUser
            {
                Username = request.Username,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName
            };

            var userId = await _userManager.RegisterUserAsync(newUser, request.Password);

            return CreatedAtAction(nameof(GetUser), new { id = userId }, newUser);
        }

        // Authenticate user (login)
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _userManager.AuthenticateUserAsync(request.Username, request.Password);

            if (user == null)
                return Unauthorized("Invalid username or password.");

            var token = _tokenHandler.CreateAccessToken(3600, user);

            return Ok(new { Message = "Login successful", Token = token });
        }

        // Get user by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);

            if (user == null)
                return NotFound("User not found.");

            return Ok(user);
        }

        // Update user details
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserRequest request)
        {
            var existingUser = await _userService.GetUserByIdAsync(id);

            if (existingUser == null)
                return NotFound("User not found.");

            existingUser.FirstName = request.FirstName;
            existingUser.LastName = request.LastName;
            existingUser.Username = request.Username;
            existingUser.Email = request.Email;
            
            var updated = await _userService.UpdateUserAsync(existingUser);

            if (!updated)
                return BadRequest("Failed to update user.");

            return Ok(existingUser);
        }

        // Delete user
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var deleted = await _userService.DeleteUserAsync(id);

            if (!deleted)
                return NotFound("User not found.");

            return NoContent();
        }
        [Authorize(AuthenticationSchemes = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }
    }
}