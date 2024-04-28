using DatabaseBenchmarking.DBConnection.Contexts;

namespace DatabaseBenchmarking.DBConnection.Services.Factories
{
    public class SqlServerDatabaseContextFactory : DatabaseContextFactory
    {
        public override DatabaseContext GetContext()
        {
            return new SqlServerDatabaseContext();
        }
    }
}
