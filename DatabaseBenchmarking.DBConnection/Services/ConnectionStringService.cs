using DatabaseBenchmarking.DBConnection.Entities;
using Microsoft.Extensions.Configuration;

namespace DatabaseBenchmarking.DBConnection.Extensions;

public static class ConnectionStringService
{
    private static IConfigurationRoot environmentConfiguration = new ConfigurationBuilder()
        .AddUserSecrets<BaseEntity>()
        .Build();
    public static string SqlServerConnectionString
        => GetConnectionString("ConnectionStrings:Sql");

    public static string MySqlConnectionString
        => GetConnectionString("ConnectionStrings:MySql");

    public static string PostgreSqlConnectionString
        => GetConnectionString("ConnectionStrings:PostgreSql");

    public static string MongoDbConnectionString
        => GetConnectionString("ConnectionStrings:MongoDB");

    public static string GetConnectionString(string path)
       => environmentConfiguration.GetSection(path).Value!;

}
