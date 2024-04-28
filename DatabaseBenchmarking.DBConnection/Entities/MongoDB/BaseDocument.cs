using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DatabaseBenchmarking.DBConnection.Entities.MongoDB
{
    public class BaseDocument
    {
        [BsonId]
        public ObjectId Id { get; set; }
    }
}
