using DatabaseBenchmarking.DBConnection.Contexts;

namespace DatabaseBenchmarking.DBConnection.Services.Factories
{
    public class PostgreSqlDatabaseContextFactory : DatabaseContextFactory
    {
        public override DatabaseContext GetContext()
        {
            return new PostgreSqlDatabaseContext();
        }
    }
}
