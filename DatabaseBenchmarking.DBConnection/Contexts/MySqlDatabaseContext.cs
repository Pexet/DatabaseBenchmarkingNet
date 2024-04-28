using DatabaseBenchmarking.DBConnection.Extensions;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Z.Dapper.Plus;

namespace DatabaseBenchmarking.DBConnection.Contexts
{
    public class MySqlDatabaseContext : DatabaseContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = ConnectionStringService.MySqlConnectionString;
            ServerVersion? serverVersion = ServerVersion.AutoDetect(connectionString);

            optionsBuilder.UseMySql(connectionString, serverVersion, options => options.SchemaBehavior(MySqlSchemaBehavior.Ignore));
        }
    }
}
