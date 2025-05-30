using EHealthBridgeAPI.Persistence.Contexts.Dapper;
using System.ComponentModel.DataAnnotations.Schema;
using EHealthBridgeAPI.Application.Repositories;
using EHealthBridgeAPI.Application.Extensions;
using EHealthBridgeAPI.Domain.Entities.Common;
using System.Reflection;
using System.Data;
using Dapper;

namespace EHealthBridgeAPI.Persistence.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity, new()
    {
        private readonly EHealthBridgeAPIDbContext _context;
        private readonly string _tableName;
        private readonly List<string> _columnNames;

        public GenericRepository(EHealthBridgeAPIDbContext context)
        {
            _context = context;

            // Get the table name from [Table] attribute, if present
            var tableAttr = typeof(T).GetCustomAttribute<TableAttribute>();
            _tableName = tableAttr?.Name ?? (typeof(T).Name.ToSnakeCase() + "s");

            // Get column names from [Column] attribute or fallback to snake_case
            _columnNames = typeof(T)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.Name != nameof(BaseEntity.Id))
                .Select(p =>
                {
                    var columnAttr = p.GetCustomAttribute<ColumnAttribute>();
                    var columnName = columnAttr?.Name ?? p.Name.ToSnakeCase();
                    return $"\"{columnName}\"";
                })
                .ToList();
        }

        public async Task<int> InsertAsync(T entity)
        {

            try
            {
                var columns = string.Join(", ", _columnNames);
                var parameters = string.Join(", ", _columnNames.Select(c => "@" + c.Trim('"')));

                var sql = $@"
                INSERT INTO {_tableName} ({columns})
                VALUES ({parameters})
                RETURNING id;
            ";

                using var connection = _context.CreateConnection();
                return await connection.ExecuteScalarAsync<int>(sql, entity);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            var sql = $"SELECT * FROM {_tableName}";
            using var connection = _context.CreateConnection();

           return  await connection.QueryAsync<T>(sql); // dynamic
        }

        public async Task<T> GetByIdAsync(int id)
        {
            var sql = $"SELECT * FROM {_tableName} WHERE id = @id";
            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<T>(sql, new { id });
        }

        public async Task<bool> UpdateAsync(T entity)
        {
            var properties = typeof(T).GetProperties()
                             .Where(p => p.Name != "Id");

            var setClause = string.Join(", ",
      properties.Select(p => $"{p.Name.ToSnakeCase()} = @{p.Name}"));

                    var sql = $@"
                UPDATE {_tableName}
                SET {setClause}
                WHERE id = @Id";

            using var connection = _context.CreateConnection();
            try
            {
                var affected = await connection.ExecuteAsync(sql, entity);
                return affected > 0;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var sql = $"DELETE FROM {_tableName} WHERE id = @id";
            using var connection = _context.CreateConnection();
            var affected = await connection.ExecuteAsync(sql, new { id });
            return affected > 0;
        }
    }
}
