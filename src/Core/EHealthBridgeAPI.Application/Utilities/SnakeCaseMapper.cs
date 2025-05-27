using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using EHealthBridgeAPI.Application.DTOs.User;
using EHealthBridgeAPI.Domain.Entities.Common;

public static class SnakeCaseMapper
{
    public static List<T> MapTo<T>(IEnumerable<dynamic> rows) where T : BaseEntity, new()
    {
        if (rows is null) return null;
        var props = typeof(T).GetProperties();
        var result = new List<T>();

        foreach (var row in rows)
        {
            var entity = new T();
            var dict = (IDictionary<string, object>)row;

            foreach (var prop in props)
            {
                var columnAttr = prop.GetCustomAttribute<ColumnAttribute>();
                var columnName = columnAttr?.Name ?? prop.Name;

                if (dict.TryGetValue(columnName, out var value) && value != null && prop.CanWrite)
                {
                    //prop.SetValue(entity, Convert.ChangeType(value, prop.PropertyType));
                    var targetType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                    prop.SetValue(entity, value == null ? null : Convert.ChangeType(value, targetType));
                }
            }

            result.Add(entity);
        }

        return result;
    }
    public static T MapTo<T>(dynamic row) where T : BaseEntity, new()
    {
        if (row is null) return null;

        var props = typeof(T).GetProperties();
        var result = new List<T>();
        var entity = new T();
        var dict = (IDictionary<string, object>)row;

        foreach (var prop in props)
        {
            var columnAttr = prop.GetCustomAttribute<ColumnAttribute>();
            var columnName = columnAttr?.Name ?? prop.Name;

            if (dict.TryGetValue(columnName, out var value) && value != null && prop.CanWrite)
            {
                //prop.SetValue(entity, Convert.ChangeType(value, prop.PropertyType));
                var targetType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                prop.SetValue(entity, value == null ? null : Convert.ChangeType(value, targetType));
            }
        }


        return entity;
    }
}
