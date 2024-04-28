using DatabaseBenchmarking.DBConnection.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Reflection;

namespace DatabaseBenchmarking.DBConnection.Extensions
{
    public static class DapperExtensions
    {
        /// <summary>
        /// Returns Table Name based on Entity [Table] attribute value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string GetTableName<T>() where T : BaseEntity
        {
            Type type = typeof(T);
            TableAttribute? tableAttribute = type.GetCustomAttribute<TableAttribute>();
            if (tableAttribute != null)
            {
                return tableAttribute.Name;
            }

            return type.Name;
        }

        public static IEnumerable<string> GetColumns<T>(bool excludeKey = false)
        {
            Type type = typeof(T);

            return type
                .GetProperties()
                .Where(e => (!excludeKey || !e.IsDefined(typeof(KeyAttribute))) && e.GetCustomAttribute<ColumnAttribute>() != null)
                .Select(e => e.Name);
        }
    }
}
