using System.ComponentModel.DataAnnotations.Schema;
using EHealthBridgeAPI.Domain.Entities.Common;
using System.Reflection;
using System;
using Dapper;

namespace EHealthBridgeAPI.Application.Utilities
{
    public static class DapperParamBuilder
    {
        public static DynamicParameters BuildParameters<T>(T entity, bool includeId = false)
        {
            var parameters = new DynamicParameters();

            foreach (var prop in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!includeId && prop.Name == nameof(BaseEntity.Id))
                    continue;

                var columnAttr = prop.GetCustomAttribute<ColumnAttribute>();
                var columnName = columnAttr?.Name;
                var value = prop.GetValue(entity);

                parameters.Add(columnName, value);
            }

            return parameters;
        }
    }
}
