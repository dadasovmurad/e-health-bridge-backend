using EHealthBridgeAPI.Application.Utilities;
using System.Data;
using Dapper;

namespace EHealthBridgeAPI.Application.Extensions
{
    public static class DapperExtensions
    {
        public static Task<int> ExecuteAutoAsync<T>(
            this IDbConnection connection,
            string sql,
            T entity,
            bool includeId = false,
            IDbTransaction transaction = null)
        {
            var param = DapperParamBuilder.BuildParameters(entity, includeId);
            return connection.ExecuteAsync(sql, param, transaction);
        }

        public static Task<TResult> ExecuteScalarAutoAsync<T, TResult>(
            this IDbConnection connection,
            string sql,
            T entity,
            bool includeId = false,
            IDbTransaction transaction = null)
        {
            var param = DapperParamBuilder.BuildParameters(entity, includeId);
            return connection.ExecuteScalarAsync<TResult>(sql, param, transaction);
        }
    }
}
