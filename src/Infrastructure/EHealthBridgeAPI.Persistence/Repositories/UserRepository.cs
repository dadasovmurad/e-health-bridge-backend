using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Dapper;
using EHealthBridgeAPI.Application.Repositories;
using EHealthBridgeAPI.Application.Utilities;
using EHealthBridgeAPI.Domain.Entities;
using EHealthBridgeAPI.Persistence.Contexts.Dapper;
using static Dapper.SqlMapper;

namespace EHealthBridgeAPI.Persistence.Repositories
{
    public class UserRepository : GenericRepository<AppUser>, IUserRepository
    {
        private readonly EHealthBridgeAPIDbContext _context;

        public UserRepository(EHealthBridgeAPIDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<AppUser?> GetByEmailOrUsernameAsync(string emailOrUsername)
        {
            var sql = @"
            SELECT * FROM app_users 
            WHERE email = @EmailOrUsername OR username = @EmailOrUsername";

            // Get the Npgsql connection from EF Core
            using var connection = _context.CreateConnection();

            var user = await connection.QueryFirstOrDefaultAsync<AppUser>(
                sql,
                new { EmailOrUsername = emailOrUsername }
            );

            return SnakeCaseMapper.MapTo<AppUser>(user);
        }
    }
}
