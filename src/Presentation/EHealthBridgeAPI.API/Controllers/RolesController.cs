using EHealthBridgeAPI.Application.Abstractions.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EHealthBridgeAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin")] // Only admins can manage roles
    public class RolesController : BaseApiController
    {
        private readonly IRoleService _roleService;

        public RolesController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRoles()
        {
            return GetResponseResult(await _roleService.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRole(int id)
        {
            return GetResponseResult(await _roleService.GetByIdAsync(id));
        }

        [HttpGet("byName/{name}")]
        public async Task<IActionResult> GetRoleByName(string name)
        {
            return GetResponseResult(await _roleService.GetByNameAsync(name));
        }
        [Authorize(Roles = "User",AuthenticationSchemes ="Admin")] // Only admins can manage roles
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserRoles(int userId)
        {
            return GetResponseResult(await _roleService.GetUserRolesAsync(userId));
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequest request)
        {
            return GetResponseResult(await _roleService.CreateAsync(request.Name, request.Description));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRole(int id, [FromBody] UpdateRoleRequest request)
        {
            return GetResponseResult(await _roleService.UpdateAsync(id, request.Name, request.Description));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            return GetResponseResult(await _roleService.DeleteAsync(id));
        }

        [HttpPost("assign")]
        public async Task<IActionResult> AssignRoleToUser([FromBody] AssignRoleRequest request)
        {
            return GetResponseResult(await _roleService.AssignRoleToUserAsync(request.UserId, request.RoleId));
        }

        [HttpDelete("remove")]
        public async Task<IActionResult> RemoveRoleFromUser([FromBody] AssignRoleRequest request)
        {
            return GetResponseResult(await _roleService.RemoveRoleFromUserAsync(request.UserId, request.RoleId));
        }
    }

    public record CreateRoleRequest(string Name, string Description);
    public record UpdateRoleRequest(string Name, string Description);
    public record AssignRoleRequest(int UserId, int RoleId);
} 