using Dapper;
using EHealthBridgeAPI.Application.Repositories;
using EHealthBridgeAPI.Application.Utilities;
using EHealthBridgeAPI.Domain.Entities;
using EHealthBridgeAPI.Persistence.Contexts.Dapper;

namespace EHealthBridgeAPI.Persistence.Repositories
{
    public class RoleRepository : GenericRepository<Role>, IRoleRepository
    {
        private readonly EHealthBridgeAPIDbContext _context;

        public RoleRepository(EHealthBridgeAPIDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Role> GetByNameAsync(string name)
        {
            var sql = @"
                SELECT * FROM roles 
                WHERE name = @name";

            using var connection = _context.CreateConnection();
            var rawRows = await connection.QueryFirstOrDefaultAsync(sql, new { name });
            return SnakeCaseMapper.MapTo<Role>(rawRows);
        }

        public async Task<IEnumerable<Role>> GetUserRolesAsync(int userId)
        {
            var sql = @"
                SELECT r.* FROM roles r
                INNER JOIN user_roles ur ON r.id = ur.role_id
                WHERE ur.user_id = @userId";

            using var connection = _context.CreateConnection();
            var rawRows = await connection.QueryAsync(sql, new { userId });
            return SnakeCaseMapper.MapTo<Role>(rawRows);
        }

        public async Task<bool> AssignRoleToUserAsync(int userId, int roleId)
        {
            var sql = @"
                INSERT INTO user_roles (user_id, role_id)
                VALUES (@userId, @roleId)
                ON CONFLICT (user_id, role_id) DO NOTHING";

            using var connection = _context.CreateConnection();
            try
            {
                await connection.ExecuteAsync(sql, new { userId, roleId });
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RemoveRoleFromUserAsync(int userId, int roleId)
        {
            var sql = @"
                DELETE FROM user_roles
                WHERE user_id = @userId AND role_id = @roleId";

            using var connection = _context.CreateConnection();
            var affected = await connection.ExecuteAsync(sql, new { userId, roleId });
            return affected > 0;
        }
    }
} 