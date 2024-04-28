using DatabaseBenchmarking.DBConnection.Entities;
using DatabaseBenchmarking.DBConnection.Extensions;
using Microsoft.EntityFrameworkCore;

namespace DatabaseBenchmarking.DBConnection.Contexts;

public class PostgreSqlDatabaseContext : DatabaseContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(ConnectionStringService.PostgreSqlConnectionString);
    }
}
