using Dapper;
using DatabaseBenchmarking.DBConnection.Entities;
using DatabaseBenchmarking.DBConnection.Extensions;
using DatabaseBenchmarking.DBConnection.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using static Dapper.SqlMapper;
using System.Data;
using Z.Dapper.Plus;

namespace DatabaseBenchmarking.DBConnection.Repository
{
    public class DapperRepository<TEntity> : IDisposable, IDapperRepository<TEntity>
        where TEntity : BaseEntity
    {
        private readonly IDbConnection _connection;

        public DapperRepository(IDbConnection dbConnection)
        {
            _connection = dbConnection;
            DapperPlusManager.Entity<Student>().Table("students");
        }
        public void Dispose()
        {
            _connection.Dispose();
        }


        public async Task DeleteAllAsync()
        {
            string tableName = DapperExtensions.GetTableName<TEntity>();
            string query = $"DELETE FROM {tableName}";
            await _connection.ExecuteAsync(query);
        }

        public async Task DeleteAsync(string? where = null)
        {
            string tableName = DapperExtensions.GetTableName<TEntity>();
            string query = $"DELETE FROM {tableName}";

            if (!string.IsNullOrWhiteSpace(where))
            {
                query += " WHERE " + where;
            }

            await _connection.ExecuteAsync(query);
        }

        public async Task InsertBulkAsync(IEnumerable<TEntity> entities)
        {
            await _connection.BulkActionAsync(x => x.BulkInsert(entities));
        }

        public async Task<IEnumerable<TEntity>> SelectAllAsync(string? where = null, string? join = null)
        {
            string tableName = DapperExtensions.GetTableName<TEntity>();
            string query = $"SELECT * FROM {tableName}";

            if (!string.IsNullOrWhiteSpace(join))
            {
                query += " " + join;
            }

            if (!string.IsNullOrWhiteSpace(where))
            {
                query += " WHERE " + where;
            }

            return await _connection.QueryAsync<TEntity>(query);
        }

        public async Task UpdateAsync(TEntity entity, string columnName, string? where = null)
        {
            string tableName = DapperExtensions.GetTableName<TEntity>();

            string query = $"UPDATE {tableName} SET {columnName} = @{columnName}";

            if (!string.IsNullOrWhiteSpace(where))
            {
                query += " WHERE " + where;
            }

            await _connection.ExecuteAsync(query, entity);
        }
    }
}
