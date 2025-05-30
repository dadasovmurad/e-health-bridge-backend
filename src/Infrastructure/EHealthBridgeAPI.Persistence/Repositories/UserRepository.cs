using Dapper;
using EHealthBridgeAPI.Application.Repositories;
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

        public async Task<AppUser?> GetByEmailAsync(string email)
        {
            var sql = @"
            SELECT * FROM app_users 
            WHERE email = @email";
            // Get the Npgsql connection from EF Core
            using var connection = _context.CreateConnection();
            try
            {
                return  await connection.QueryFirstOrDefaultAsync<AppUser>(
                    sql,
                    new { Email = email }
                );
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<AppUser?> GetByRefreshTokenAsync(string refreshToken)
        {
            var sql = @" SELECT * FROM app_users WHERE refresh_token = @refreshToken";

            using var connection = _context.CreateConnection();
            try
            {
                return  await connection.QueryFirstOrDefaultAsync<AppUser>(
                    sql,
                    new { refreshToken }
                );
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<AppUser?> GetByResetTokenAsync(string token)
        {
            var sql = @"
            SELECT * FROM app_users 
            WHERE password_reset_token = @token";

            // Get the Npgsql connection from EF Core
            using var connection = _context.CreateConnection();
            try
            {
               return  await connection.QueryFirstOrDefaultAsync<AppUser>(
                    sql,
                    new { token }
                );
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<AppUser> GetByUsernameAsync(string username)
        {
            var sql = @"
            SELECT * FROM app_users 
            WHERE username = @Username";

            // Get the Npgsql connection from EF Core
            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<AppUser>(
                     sql,
                     new { Username = username }
                 );
        }
    }
}