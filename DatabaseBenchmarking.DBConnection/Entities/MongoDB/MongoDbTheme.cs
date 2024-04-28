using System.ComponentModel;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DatabaseBenchmarking.DBConnection.Entities.MongoDB
{
    [Description("themes")]
    public class MongoDbTheme : BaseDocument
    {
        public string Title { get; set; } = null!;

        public string ForegoundColor { get; set; } = null!;

        public string BackgroundColor { get; set; } = null!;

        public string FontName { get; set; } = null!;

        public int FontSize { get; set; }

        public ObjectId StudentId { get; set; }

        [BsonElement]
        public List<MongoDbStudentInserted> Students { get; }
    }
}
