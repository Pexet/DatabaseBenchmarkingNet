using DatabaseBenchmarking.DBConnection.Entities;

namespace DatabaseBenchmarking.DBConnection.Repository.Interfaces;

public interface IDapperRepository<TEntity>
    where TEntity : BaseEntity
{
    Task InsertBulkAsync(IEnumerable<TEntity> entities);

    Task UpdateAsync(TEntity entity, string columnName, string? where = null);

    Task DeleteAsync(string? where = null);

    Task DeleteAllAsync();

    Task<IEnumerable<TEntity>> SelectAllAsync(string? where = null, string join = null);
}
