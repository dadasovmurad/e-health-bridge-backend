using Core.Results;
using EHealthBridgeAPI.Application.Abstractions.Services;
using EHealthBridgeAPI.Application.Constant;
using EHealthBridgeAPI.Application.Repositories;
using EHealthBridgeAPI.Domain.Entities;

namespace EHealthBridgeAPI.Persistence.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;

        public RoleService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<IDataResult<IEnumerable<Role>>> GetAllAsync()
        {
            var roles = await _roleRepository.GetAllAsync();
            if (roles == null || !roles.Any())
            {
                return new ErrorDataResult<IEnumerable<Role>>("No roles found");
            }
            return new SuccessDataResult<IEnumerable<Role>>(roles);
        }

        public async Task<IDataResult<Role>> GetByIdAsync(int id)
        {
            var role = await _roleRepository.GetByIdAsync(id);
            if (role == null)
            {
                return new ErrorDataResult<Role>("Role not found");
            }
            return new SuccessDataResult<Role>(role);
        }

        public async Task<IDataResult<Role>> GetByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return new ErrorDataResult<Role>("Role name cannot be empty");
            }

            var role = await _roleRepository.GetByNameAsync(name);
            if (role == null)
            {
                return new ErrorDataResult<Role>("Role not found");
            }
            return new SuccessDataResult<Role>(role);
        }

        public async Task<IDataResult<IEnumerable<Role>>> GetUserRolesAsync(int userId)
        {
            var roles = await _roleRepository.GetUserRolesAsync(userId);
            if (roles == null || !roles.Any())
            {
                return new ErrorDataResult<IEnumerable<Role>>("No roles found for this user");
            }
            return new SuccessDataResult<IEnumerable<Role>>(roles);
        }

        public async Task<IResult> AssignRoleToUserAsync(int userId, int roleId)
        {
            var success = await _roleRepository.AssignRoleToUserAsync(userId, roleId);
            if (!success)
            {
                return new ErrorResult("Failed to assign role to user");
            }
            return new SuccessResult("Role assigned successfully");
        }

        public async Task<IResult> RemoveRoleFromUserAsync(int userId, int roleId)
        {
            var success = await _roleRepository.RemoveRoleFromUserAsync(userId, roleId);
            if (!success)
            {
                return new ErrorResult("Failed to remove role from user");
            }
            return new SuccessResult("Role removed successfully");
        }

        public async Task<IDataResult<int>> CreateAsync(string name, string description)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return new ErrorDataResult<int>("Role name cannot be empty");
            }

            var existingRole = await _roleRepository.GetByNameAsync(name);
            if (existingRole != null)
            {
                return new ErrorDataResult<int>("Role already exists");
            }

            var role = new Role
            {
                Name = name,
                Description = description,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var id = await _roleRepository.InsertAsync(role);
            return new SuccessDataResult<int>(id, "Role created successfully");
        }

        public async Task<IResult> UpdateAsync(int id, string name, string description)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return new ErrorResult("Role name cannot be empty");
            }

            var existingRole = await _roleRepository.GetByIdAsync(id);
            if (existingRole == null)
            {
                return new ErrorResult("Role not found");
            }

            existingRole.Name = name;
            existingRole.Description = description;
            existingRole.UpdatedAt = DateTime.UtcNow;

            var success = await _roleRepository.UpdateAsync(existingRole);
            if (!success)
            {
                return new ErrorResult("Failed to update role");
            }
            return new SuccessResult("Role updated successfully");
        }

        public async Task<IResult> DeleteAsync(int id)
        {
            var success = await _roleRepository.DeleteAsync(id);
            if (!success)
            {
                return new ErrorResult("Failed to delete role");
            }
            return new SuccessResult("Role deleted successfully");
        }
    }
} 