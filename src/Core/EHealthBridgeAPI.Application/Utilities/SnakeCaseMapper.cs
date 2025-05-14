using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using EHealthBridgeAPI.Domain.Entities.Common;

public static class SnakeCaseMapper
{
    public static List<T> MapTo<T>(IEnumerable<dynamic> rows) where T : BaseEntity, new()
    {
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
                    prop.SetValue(entity, Convert.ChangeType(value, prop.PropertyType));
                }
            }

            result.Add(entity);
        }

        return result;
    }
    public static T MapTo<T>(dynamic row) where T : BaseEntity, new()
    {
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
                prop.SetValue(entity, Convert.ChangeType(value, prop.PropertyType));
            }
        }


        return entity;
    }
}
