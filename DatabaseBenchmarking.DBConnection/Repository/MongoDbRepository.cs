using DatabaseBenchmarking.DBConnection.Entities;
using DatabaseBenchmarking.DBConnection.Entities.MongoDB;
using DatabaseBenchmarking.DBConnection.Extensions;
using DatabaseBenchmarking.DBConnection.Helpers;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DatabaseBenchmarking.DBConnection.Repository
{
    // TODO: Move to repository
    public class MongoDbRepository<TEntity> where TEntity: BaseDocument
    {
        private readonly string _collectionName;
        private readonly IMongoDatabase _mongoDatabase;

        public MongoDbRepository()
        {
            MongoClient mongoClient = new MongoClient(ConnectionStringService.MongoDbConnectionString);
            _mongoDatabase = mongoClient.GetDatabase("admin");
            _collectionName = typeof(TEntity).GetDescription();
        }

        public async Task CreateCollectionIfNotExistsAsync()
        {
            bool collectionExists = _mongoDatabase.ListCollectionNames().ToList().Contains(_collectionName);
            if (!collectionExists)
            {
                await _mongoDatabase.CreateCollectionAsync(_collectionName);
            }
        }

        public async Task<long> CountAsync()
        {
            IMongoCollection<TEntity> collection = _mongoDatabase.GetCollection<TEntity>(_collectionName);
            return await collection.CountDocumentsAsync(Builders<TEntity>.Filter.Empty);
        }

        public async Task DeleteAllAsync()
        {
            FilterDefinition<TEntity>? filter = Builders<TEntity>.Filter.Empty;
            IMongoCollection<TEntity> collection = _mongoDatabase.GetCollection<TEntity>(_collectionName);

            await collection.DeleteManyAsync(filter);
        }

        public async Task DeleteAsync(FilterDefinition<TEntity>? filter)
        {
            IMongoCollection<TEntity> collection = _mongoDatabase.GetCollection<TEntity>(_collectionName);

            await collection.DeleteOneAsync(filter);
        }

        public async Task DeleteManyAsync(FilterDefinition<TEntity>? filter)
        {
            IMongoCollection<TEntity> collection = _mongoDatabase.GetCollection<TEntity>(_collectionName);

            await collection.DeleteManyAsync(filter);
        }

        public async Task InsertAsync(TEntity entity)
        {
            IMongoCollection<TEntity> collection = _mongoDatabase.GetCollection<TEntity>(_collectionName);
            await collection.InsertOneAsync(entity);
        }

        public async Task InsertBulkAsync(IEnumerable<TEntity> entities)
        {
            IMongoCollection<TEntity> collection = _mongoDatabase.GetCollection<TEntity>(_collectionName);

            await collection.InsertManyAsync(entities);
        }

        public async Task<IEnumerable<TEntity>> SelectAllAsync(FilterDefinition<TEntity> filter)
        {
            IMongoCollection<TEntity> collection = _mongoDatabase.GetCollection<TEntity>(_collectionName);

            return await collection.Find(filter).ToListAsync();
        }

        public async Task<IEnumerable<TEntity>> SelectAllLookupAsync(FilterDefinition<BsonDocument> filter, string from, string localField, string foreignField, string _as)
        {
            IMongoCollection<TEntity> collection = _mongoDatabase.GetCollection<TEntity>(_collectionName);

            return await collection
                .Aggregate()
                .Lookup(from, localField, foreignField, _as)
                .Match(filter)
                .As<TEntity>()
                .ToListAsync();
        }

        public async Task<TEntity> SelectAsync(FilterDefinition<TEntity> filter)
        {
            IMongoCollection<TEntity> collection = _mongoDatabase.GetCollection<TEntity>(_collectionName);

            return await collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task UpdateManyAsync(FilterDefinition<TEntity> filter, UpdateDefinition<TEntity> update)
        {
            IMongoCollection<TEntity> collection = _mongoDatabase.GetCollection<TEntity>(_collectionName);

            await collection.UpdateManyAsync(filter, update);
        }
    }
}
