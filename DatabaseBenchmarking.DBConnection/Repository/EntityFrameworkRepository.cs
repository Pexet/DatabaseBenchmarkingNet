using DatabaseBenchmarking.DBConnection.Contexts;
using DatabaseBenchmarking.DBConnection.Entities;
using DatabaseBenchmarking.DBConnection.Extensions;
using DatabaseBenchmarking.DBConnection.Helpers;
using DatabaseBenchmarking.DBConnection.Repository.Interfaces;
using DatabaseBenchmarking.DBConnection.Services.Factories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Z.EntityFramework.Plus;

namespace DatabaseBenchmarking.DBConnection.Repository;

public class EntityFrameworkRepository<TEntity> : IEntityFrameworkRepository<TEntity>
    where TEntity : BaseEntity
{
    private readonly DatabaseContextFactory databaseContextFactory;
    
    private DatabaseContext dataBaseContext;
    private DbSet<TEntity> entitiesDataSet;

    public EntityFrameworkRepository(DatabaseContextFactory databaseContextFactory)
    {
        dataBaseContext = databaseContextFactory.GetContext();
        entitiesDataSet = dataBaseContext.Set<TEntity>();

        this.databaseContextFactory = databaseContextFactory;
    }

    public async Task InsertBulkAsync(IEnumerable<TEntity> entities)
    {
        foreach (var entity in entities)
        {
            entitiesDataSet.Add(entity);
        }
        await dataBaseContext.SaveChangesAsync();
    }

    public async Task<TEntity?> SelectAsync(Expression<Func<TEntity, bool>> selector, params Expression<Func<TEntity, object>>[] includeProperties)
    {
        return await GetValueWithInclude(includeProperties).Where(selector).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<TEntity>> SelectAllAsync(Expression<Func<TEntity, bool>> selector, params Expression<Func<TEntity, object>>[] includeProperties)
    {
        return await GetValueWithInclude(includeProperties).Where(selector).ToListAsync();
    }

    public async Task<int> CountAsync(Expression<Func<TEntity, bool>> selector, params Expression<Func<TEntity, object>>[] includeProperties)
    {
        return await GetValueWithInclude(includeProperties).Where(selector).CountAsync();
    }

    public async Task UpdateAsync(Expression<Func<TEntity, bool>> selector, Expression<Func<TEntity, TEntity>> updateFactory)
    {
        await entitiesDataSet.Where(selector).UpdateAsync(updateFactory);
    }

    public async Task DeleteAllAsync()
    {
        string tableName = DapperExtensions.GetTableName<TEntity>();
        await dataBaseContext.Database.ExecuteSqlRawAsync($"DELETE FROM {tableName}");
        await dataBaseContext.SaveChangesAsync();
    }

    private IQueryable<TEntity> GetValueWithInclude(
            params Expression<Func<TEntity, object>>[] includeProperties)
    {
        IQueryable<TEntity> querriedEntities = entitiesDataSet.AsNoTracking();

        return includeProperties.Aggregate(
            querriedEntities,
            (current, includeProperty) => current.Include(includeProperty));
    }
}
