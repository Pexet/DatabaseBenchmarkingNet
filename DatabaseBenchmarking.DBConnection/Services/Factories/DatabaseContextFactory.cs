using DatabaseBenchmarking.DBConnection.Contexts;

namespace DatabaseBenchmarking.DBConnection.Services.Factories
{
    public abstract class DatabaseContextFactory
    {
        public abstract DatabaseContext GetContext();
    }
}
