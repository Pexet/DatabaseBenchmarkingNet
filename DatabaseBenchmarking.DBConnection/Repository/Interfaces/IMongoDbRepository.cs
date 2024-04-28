using DatabaseBenchmarking.DBConnection.Entities.MongoDB;
using MongoDB.Driver;

namespace DatabaseBenchmarking.DBConnection.Repository.Interfaces
{
    public interface IMongoDbRepository<TEntity> where TEntity : BaseDocument
    {
        Task DeleteAllAsync();
        Task DeleteAsync(TEntity entity);
        Task InsertAsync(TEntity entity);
        Task InsertBulkAsync(IEnumerable<TEntity> entities);
        Task<IEnumerable<TEntity>> SelectAllAsync(FilterDefinition<TEntity> filter);
        Task<TEntity> SelectAsync(FilterDefinition<TEntity> filter);
    }
}
