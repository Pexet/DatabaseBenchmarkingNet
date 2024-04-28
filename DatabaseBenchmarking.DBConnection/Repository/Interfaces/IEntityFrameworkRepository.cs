using DatabaseBenchmarking.DBConnection.Entities;
using System.Linq.Expressions;

namespace DatabaseBenchmarking.DBConnection.Repository.Interfaces;

public interface IEntityFrameworkRepository<TEntity>
    where TEntity : BaseEntity
{
    Task InsertBulkAsync(IEnumerable<TEntity> entities);

    Task DeleteAllAsync();

    Task<TEntity?> SelectAsync(Expression<Func<TEntity, bool>> selector, params Expression<Func<TEntity, object>>[] includeProperties);

    Task<IEnumerable<TEntity>> SelectAllAsync(
        Expression<Func<TEntity, bool>> selector,
        params Expression<Func<TEntity, object>>[] includeProperties);

    Task UpdateAsync(Expression<Func<TEntity, bool>> selector, Expression<Func<TEntity, TEntity>> updateFactory);

    Task<int> CountAsync(Expression<Func<TEntity, bool>> selector, params Expression<Func<TEntity, object>>[] includeProperties);
}
