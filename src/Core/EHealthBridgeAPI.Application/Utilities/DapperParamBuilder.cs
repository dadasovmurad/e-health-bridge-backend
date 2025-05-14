using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using EHealthBridgeAPI.Domain.Entities.Common;

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
