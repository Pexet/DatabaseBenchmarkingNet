using System.ComponentModel;

namespace DatabaseBenchmarking.DBConnection.Entities.MongoDB
{
    [Description("students_inserted")]
    public class MongoDbStudentInserted: BaseDocument
    {
        public bool IsNotifiable { get; set; }

        public string EmailAddress { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string PhoneNumber { get; set; } = null!;
    }
}
