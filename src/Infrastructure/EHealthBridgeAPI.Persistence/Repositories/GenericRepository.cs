using Dapper;
using EHealthBridgeAPI.Application.Extensions;
using EHealthBridgeAPI.Application.Repositories;
using EHealthBridgeAPI.Application.Utilities;
using EHealthBridgeAPI.Domain.Entities.Common;
using EHealthBridgeAPI.Persistence.Contexts.Dapper;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Reflection;
using System.Text.RegularExpressions;

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
            var param = DapperParamBuilder.BuildParameters(entity);

            var columns = string.Join(", ", _columnNames);
            var parameters = string.Join(", ", _columnNames.Select(c => "@" + c.Trim('"')));

            var sql = $@"
                INSERT INTO {_tableName} ({columns})
                VALUES ({parameters})
                RETURNING id;
            ";

            using var connection = _context.CreateConnection();
            return await connection.ExecuteScalarAsync<int>(sql, param);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            var sql = $"SELECT * FROM {_tableName}";
            using var connection = _context.CreateConnection();

            var rawRows = await connection.QueryAsync(sql); // dynamic
            return SnakeCaseMapper.MapTo<T>(rawRows);
        }

        public async Task<T> GetByIdAsync(int id)
        {
            var sql = $"SELECT * FROM {_tableName} WHERE id = @id";
            using var connection = _context.CreateConnection();
            var rawRows = await connection.QueryFirstOrDefaultAsync(sql, new { id });
            return SnakeCaseMapper.MapTo<T>(rawRows);
        }

        public async Task<bool> UpdateAsync(T entity)
        {
            var param = DapperParamBuilder.BuildParameters(entity, includeId: true);

            var setClause = string.Join(", ", _columnNames.Select(c => $"{c} = @{c.Trim('"')}"));
            var sql = $@"
                UPDATE {_tableName}
                SET {setClause}
                WHERE id = @Id
                ";

            using var connection = _context.CreateConnection();
            try
            {
                var affected = await connection.ExecuteAsync(sql, param);
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
