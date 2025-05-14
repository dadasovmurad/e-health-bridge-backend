using Dapper;
using EHealthBridgeAPI.Application.Extensions;
using EHealthBridgeAPI.Application.Repositories;
using EHealthBridgeAPI.Domain.Entities.Common;
using EHealthBridgeAPI.Persistence.Contexts.Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EHealthBridgeAPI.Persistence.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly EHealthBridgeAPIDbContext _context;
        private readonly string _tableName;
        private readonly List<string> _columnNames;

        public GenericRepository(EHealthBridgeAPIDbContext context)
        {
            _context = context;
            _tableName = typeof(T).Name + "s";
            _columnNames = typeof(T).GetProperties().Where(p => p.Name != nameof(BaseEntity.Id)).Select(p => p.Name.ToSnakeCase()).ToList();
        }
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            // Construct the SQL query to select all records from the table.
            var query = $"SELECT * FROM {_tableName}";

            // Open a database connection.
            using (var connection = _context.CreateConnection())
            {
                // Execute the query asynchronously and retrieve the results.
                var result = await connection.QueryAsync<T>(query);

                // Return the retrieved entities.
                return result;
            }
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            // Construct the SQL query to select a record by its ID from the table.
            var query = $"SELECT * FROM {_tableName} WHERE id = @Id";

            // Open a database connection.
            using (var connection = _context.CreateConnection())
            {
                // Execute the query asynchronously and retrieve the result.
                var result = await connection.QuerySingleOrDefaultAsync<T>(query, new { Id = id });

                // Return the retrieved entity (or null if not found).
                return result;
            }
        }

        public async Task<int> InsertAsync(T model)
        {
            // Construct the SQL query to insert a new record into the table.
            var query = $"INSERT INTO {_tableName} ({string.Join(',', _columnNames)}) VALUES (@{string.Join(", @", _columnNames)});" +
                    // Use SCOPE_IDENTITY() in SQL Server to retrieve the latest generated identity value.
                    // This is used to get the auto-incremented identity value after an INSERT operation.
                    "SELECT CAST(SCOPE_IDENTITY() as int)";

            // Open a database connection.
            using (var connection = _context.CreateConnection())
            {
                // Execute the query asynchronously and retrieve the inserted ID.
                var id = await connection.QueryFirstOrDefaultAsync<int>(query, model);

                // Return the inserted ID.
                return id;
            }
        }

        public async Task<bool> UpdateAsync(T model)
        {
            // Generate SET clause for the SQL query based on column names.
            var setValues = _columnNames.Select(prop => $"{prop} = @{prop}");

            // Construct the SQL query to update the record in the table.
            var query = $"UPDATE {_tableName} SET {string.Join(", ", setValues)} WHERE id = @Id";

            // Open a database connection.
            using (var connection = _context.CreateConnection())
            {
                // Execute the query asynchronously and retrieve the result.
                var result = await connection.ExecuteAsync(query, model);

                // Return true if at least one record was affected; otherwise, return false.
                return result > 0;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            // Construct the SQL query to delete the record from the table.
            var query = $"DELETE FROM {_tableName} WHERE id = @Id";

            // Open a database connection.
            using (var connection = _context.CreateConnection())
            {
                // Execute the query asynchronously and retrieve the result.
                var result = await connection.ExecuteAsync(query, new { Id = id });

                // Return true if at least one record was affected; otherwise, return false.
                return result > 0;
            }
        }
    }
}
